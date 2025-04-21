'use client';

import React, { useState, useEffect, useCallback, useRef, useMemo } from 'react';
import PixelCanvas from '../../components/PixelCanvas/PixelCanvas';
import ColorPalette from '../../components/ColorPalette/ColorPalette';
import { useMockSignalR } from '../../hooks/useMockSignalR';
import { GridPoint, TransformState, CanvasDimensions } from '../../types/canvas';
import { CELL_SIZE, GRID_WIDTH, GRID_HEIGHT } from '../../config/constants';
import { PALETTE_COLORS } from '../../config/colors';
import { clampTransform, calculateZoomedTransform } from '../../utils/transformUtils';
import { useAuth } from '../../contexts/AuthContext';

export default function CanvasPage() {
  // Get authentication state
  const { isAuthenticated } = useAuth();
  
  // Canvas grid state
  const [canvasGridState, setCanvasGridState] = useState<string[][] | null>(null);
  
  // Initialize with a default transform in the center of the grid
  const [transform, setTransform] = useState<TransformState>({
    scale: 1,
    translateX: 0,
    translateY: 0
  });
  
  // Use a ref to avoid lag during panning/zooming
  const transformRef = useRef<TransformState>(transform);
  
  // Update transform ref immediately whenever transform state changes
  // This is crucial for preventing state loss during interactions
  useEffect(() => {
    transformRef.current = transform;
  }, [transform]);
  
  // Store transform in localStorage whenever it changes
  useEffect(() => {
    try {
      localStorage.setItem('canvasTransform', JSON.stringify(transform));
    } catch (e) {
      console.error('Failed to save transform state:', e);
    }
  }, [transform]);
  
  // Load transform from localStorage on initial load
  useEffect(() => {
    try {
      const savedTransform = localStorage.getItem('canvasTransform');
      if (savedTransform) {
        setTransform(JSON.parse(savedTransform));
      }
    } catch (e) {
      console.error('Failed to load saved transform:', e);
    }
  }, []);
  
  // Selection state
  const [selectedCell, setSelectedCell] = useState<GridPoint | null>(null);
  const [selectedColor, setSelectedColor] = useState<string | null>(null);
  
  // Canvas dimensions for transform clamping
  const [canvasDimensions, setCanvasDimensions] = useState<CanvasDimensions>({
    width: 0,
    height: 0
  });
  
  // Initialize the mock SignalR hook
  const mockSignalR = useMockSignalR();
  
  // Ref to keep track of the pixel canvas component for dimension updates
  const canvasContainerRef = useRef<HTMLDivElement>(null);
  
  // Update canvas dimensions when the window resizes
  useEffect(() => {
    const updateDimensions = () => {
      if (canvasContainerRef.current) {
        const { width, height } = canvasContainerRef.current.getBoundingClientRect();
        setCanvasDimensions({ width, height });
      }
    };
    
    updateDimensions();
    
    window.addEventListener('resize', updateDimensions);
    return () => window.removeEventListener('resize', updateDimensions);
  }, []);
  
  // Fetch initial grid data
  useEffect(() => {
    const fetchInitialGrid = async () => {
      try {
        const grid = await mockSignalR.getInitialGrid();
        setCanvasGridState(grid);
        
        // Center the canvas only if no saved transform exists
        // and we have valid canvas dimensions
        if (canvasDimensions.width > 0 && canvasDimensions.height > 0) {
          // Check if we have a saved transform
          const hasSavedTransform = localStorage.getItem('canvasTransform') !== null;
          
          // Only set default transform if we don't have a saved one
          if (!hasSavedTransform) {
            const gridWidthPx = GRID_WIDTH * CELL_SIZE;
            const gridHeightPx = GRID_HEIGHT * CELL_SIZE;
            
            setTransform({
              scale: 1,
              translateX: (canvasDimensions.width - gridWidthPx) / 2,
              translateY: (canvasDimensions.height - gridHeightPx) / 2
            });
          }
        }
      } catch (error) {
        console.error('Failed to fetch initial grid:', error);
      }
    };
    
    fetchInitialGrid();
  }, [mockSignalR, canvasDimensions]);
  
  // Subscribe to real-time updates
  useEffect(() => {
    if (!canvasGridState) return;
    
    const unsubscribe = mockSignalR.subscribeToCellUpdates((update) => {
      setCanvasGridState(prev => {
        if (!prev) return prev;
        
        // Update immutably
        const next = prev.map(row => [...row]);
        next[update.y][update.x] = update.color;
        return next;
      });
    });
    
    return unsubscribe;
  }, [canvasGridState, mockSignalR]);
  
  // Handle cell selection
  const handleCellSelect = useCallback((point: GridPoint) => {
    setSelectedCell(point);
  }, []);
  
  // Handle color selection
  const handleColorSelect = useCallback((color: string) => {
    setSelectedColor(color);
  }, []);
  
  // Handle panning
  const handlePan = useCallback((dx: number, dy: number) => {
    setTransform(prev => {
      const next = {
        ...prev,
        translateX: prev.translateX + dx,
        translateY: prev.translateY + dy
      };
      
      return clampTransform(
        next,
        GRID_WIDTH,
        GRID_HEIGHT,
        CELL_SIZE,
        canvasDimensions.width,
        canvasDimensions.height
      );
    });
  }, [canvasDimensions]);
  
  // Get canvas element for zooming
  const [canvasElement, setCanvasElement] = useState<HTMLCanvasElement | null>(null);
  
  // Handle zooming
  const handleZoom = useCallback((dScale: number, clientX: number, clientY: number) => {
    if (!canvasElement) return;
    
    setTransform(prev => {
      // Calculate the new transform with proper zooming around the pointer
      const next = calculateZoomedTransform(
        prev,
        dScale,
        clientX,
        clientY,
        canvasElement
      );
      
      // Apply clamping to keep within bounds
      return clampTransform(
        next,
        GRID_WIDTH,
        GRID_HEIGHT,
        CELL_SIZE,
        canvasDimensions.width,
        canvasDimensions.height
      );
    });
  }, [canvasDimensions, canvasElement]);
  
  // Place a pixel when both a cell and color are selected and user is authenticated
  useEffect(() => {
    if (
      selectedCell &&
      selectedColor &&
      !mockSignalR.isRateLimited &&
      canvasGridState &&
      isAuthenticated // Only allow pixel placement if user is authenticated
    ) {
      const placePixelAndUpdateState = async () => {
        const success = await mockSignalR.placePixel(
          selectedCell.x,
          selectedCell.y,
          selectedColor
        );
        
        if (success) {
          // Update local state immediately for better UX
          setCanvasGridState(prev => {
            if (!prev) return prev;
            
            const next = prev.map(row => [...row]);
            next[selectedCell.y][selectedCell.x] = selectedColor;
            return next;
          });
          
          // Reset selection after successful placement
          setSelectedCell(null);
        }
      };
      
      placePixelAndUpdateState();
    } else if (selectedCell && selectedColor && !isAuthenticated) {
      // If user tries to place a pixel but is not authenticated, clear the selection
      // and show a message that they need to log in
      setSelectedCell(null);
      console.log('Authentication required to place pixels');
      // In a real app, you'd show a UI notification or redirect to login
    }
  }, [selectedCell, selectedColor, mockSignalR, canvasGridState, isAuthenticated]);
  
  // Add a memo to prevent unnecessary re-renders of the canvas
  const memoizedCanvas = useMemo(() => (
    <PixelCanvas
      gridData={canvasGridState}
      cellSize={CELL_SIZE}
      transform={transform}
      selectedCell={selectedCell}
      onCellSelect={handleCellSelect}
      onPan={handlePan}
      onZoom={handleZoom}
      isInteractionDisabled={mockSignalR.isRateLimited}
      onCanvasReady={setCanvasElement}
    />
  ), [canvasGridState, transform, selectedCell, mockSignalR.isRateLimited, 
      handleCellSelect, handlePan, handleZoom, setCanvasElement]);

  // Add a memo to prevent unnecessary re-renders of the color palette
  const memoizedPalette = useMemo(() => (
    <ColorPalette
      availableColors={PALETTE_COLORS}
      selectedColor={selectedColor}
      onColorSelect={handleColorSelect}
      isRateLimited={mockSignalR.isRateLimited}
      rateLimitEndTime={mockSignalR.rateLimitEndTime}
    />
  ), [selectedColor, mockSignalR.isRateLimited, mockSignalR.rateLimitEndTime, handleColorSelect]);

  // Handle login modal
  const [showAuthModal, setShowAuthModal] = useState(false);
  const [authModalView, setAuthModalView] = useState<'login' | 'register'>('login');
  
  const openLoginModal = () => {
    setAuthModalView('login');
    setShowAuthModal(true);
  };
  
  const closeAuthModal = () => {
    setShowAuthModal(false);
  };
  
  return (
    <main className="flex flex-col w-full h-screen relative overflow-hidden" ref={canvasContainerRef}>
      {memoizedCanvas}
      {memoizedPalette}
      
      <div className="absolute top-10 left-10 bg-white bg-opacity-80 px-3 py-2 rounded-lg text-sm pointer-events-none z-10 shadow-sm sm:text-xs sm:px-2 sm:py-1 sm:top-2 sm:left-2">
        {selectedCell && (
          <div className="font-mono font-bold">
            Selected: ({selectedCell.x}, {selectedCell.y})
          </div>
        )}
      </div>
      
      {/* Show login notification for non-authenticated users */}
      {!isAuthenticated && (
        <div className="absolute bottom-[80px] left-0 right-0 bg-blue-500 bg-opacity-90 text-white py-3 px-5 text-center text-sm z-10 shadow-md animate-[slideUp_0.3s_ease-out_forwards] pointer-events-auto sm:py-2 sm:px-4 sm:text-xs">
          Sign in to place pixels on the canvas
          <button 
            className="bg-white text-blue-600 border-none rounded ml-3 py-1.5 px-3 font-medium cursor-pointer text-sm shadow-sm transition-colors hover:bg-gray-50 sm:py-1 sm:px-2 sm:text-xs" 
            onClick={openLoginModal}
          >
            Sign In
          </button>
        </div>
      )}
      
      {/* Use the AuthModal component */}
      {showAuthModal && (
        <div 
          className="fixed inset-0 bg-black bg-opacity-50 z-50 flex justify-center items-center"
          onClick={closeAuthModal}
        >
          <div 
            className="bg-white rounded-lg p-6 w-full max-w-md mx-4"
            onClick={e => e.stopPropagation()}
          >
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-semibold">
                {authModalView === 'login' ? 'Sign In' : 'Create Account'}
              </h2>
              <button 
                onClick={closeAuthModal}
                className="text-gray-500 hover:text-gray-700 text-2xl font-bold"
              >
                &times;
              </button>
            </div>
            
            {authModalView === 'login' ? (
              <>
                <div className="mb-4">
                  <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="email">
                    Email
                  </label>
                  <input
                    id="email"
                    type="email"
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    placeholder="your@email.com"
                  />
                </div>
                <div className="mb-6">
                  <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="password">
                    Password
                  </label>
                  <input
                    id="password"
                    type="password"
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 mb-3 leading-tight focus:outline-none focus:shadow-outline"
                    placeholder="******************"
                  />
                </div>
                <div className="flex items-center justify-between">
                  <button
                    className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
                    type="button"
                  >
                    Sign In
                  </button>
                  <button
                    className="inline-block align-baseline font-bold text-sm text-blue-500 hover:text-blue-800"
                    onClick={() => setAuthModalView('register')}
                  >
                    Create Account
                  </button>
                </div>
              </>
            ) : (
              <>
                <div className="mb-4">
                  <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="username">
                    Username
                  </label>
                  <input
                    id="username"
                    type="text"
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    placeholder="username"
                  />
                </div>
                <div className="mb-4">
                  <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="reg-email">
                    Email
                  </label>
                  <input
                    id="reg-email"
                    type="email"
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    placeholder="your@email.com"
                  />
                </div>
                <div className="mb-4">
                  <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="reg-password">
                    Password
                  </label>
                  <input
                    id="reg-password"
                    type="password"
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 mb-3 leading-tight focus:outline-none focus:shadow-outline"
                    placeholder="******************"
                  />
                </div>
                <div className="mb-6">
                  <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="confirm-password">
                    Confirm Password
                  </label>
                  <input
                    id="confirm-password"
                    type="password"
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 mb-3 leading-tight focus:outline-none focus:shadow-outline"
                    placeholder="******************"
                  />
                </div>
                <div className="flex items-center justify-between">
                  <button
                    className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
                    type="button"
                  >
                    Register
                  </button>
                  <button
                    className="inline-block align-baseline font-bold text-sm text-blue-500 hover:text-blue-800"
                    onClick={() => setAuthModalView('login')}
                  >
                    Sign In Instead
                  </button>
                </div>
              </>
            )}
          </div>
        </div>
      )}
    </main>
  );
}
