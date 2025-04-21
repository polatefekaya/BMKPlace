import React, { useRef, useEffect, useState, useCallback } from 'react';
import { GridPoint, TransformState, CanvasDimensions } from '../../types/canvas';
import { drawGrid, drawSelectionHighlight, getCanvasDimensions } from './canvasUtils';
import { useCanvasInteraction } from '../../hooks/useCanvasInteraction';
import styles from './PixelCanvas.module.css';

interface PixelCanvasProps {
  gridData: string[][] | null;
  cellSize: number;
  transform: TransformState;
  selectedCell: GridPoint | null;
  onCellSelect: (point: GridPoint) => void;
  onPan: (dx: number, dy: number) => void;
  onZoom: (dScale: number, clientX: number, clientY: number) => void;
  isInteractionDisabled?: boolean;
  onCanvasReady?: (canvas: HTMLCanvasElement) => void;
}

const PixelCanvas: React.FC<PixelCanvasProps> = ({
  gridData,
  cellSize,
  transform,
  selectedCell,
  onCellSelect,
  onPan,
  onZoom,
  isInteractionDisabled = false,
  onCanvasReady
}) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const [canvasDimensions, setCanvasDimensions] = useState<CanvasDimensions>({ width: 0, height: 0 });
  
  // Use ref to track the actual canvas element for use in other functions
  const canvasElementRef = useRef<HTMLCanvasElement | null>(null);
  
  // Initialize the canvas interaction hook
  useCanvasInteraction({
    canvasRef,
    cellSize,
    transform,
    isInteractionDisabled,
    onCellSelect,
    onPan,
    onZoom
  });
  
  // Handle canvas resizing
  const updateCanvasDimensions = useCallback(() => {
    if (!containerRef.current || !canvasRef.current) return;
    
    const container = containerRef.current;
    const { width, height } = container.getBoundingClientRect();
    
    const dimensions = getCanvasDimensions(
      canvasRef.current,
      width,
      height
    );
    
    setCanvasDimensions(dimensions);
  }, []);
  
  // Set up resize observer for the container
  useEffect(() => {
    if (!containerRef.current) return;
    
    updateCanvasDimensions();
    
    const resizeObserver = new ResizeObserver(updateCanvasDimensions);
    resizeObserver.observe(containerRef.current);
    
    return () => {
      resizeObserver.disconnect();
    };
  }, [updateCanvasDimensions]);
  
  // Store the canvas reference when it changes and notify parent
  useEffect(() => {
    if (canvasRef.current) {
      canvasElementRef.current = canvasRef.current;
      if (onCanvasReady) {
        onCanvasReady(canvasRef.current);
      }
    }
  }, [canvasRef.current, onCanvasReady]);

  // Set up animation frame for rendering
  const renderRef = useRef<number | null>(null);
  
  // Draw the canvas using requestAnimationFrame for smooth rendering
  const renderCanvas = useCallback(() => {
    if (!canvasRef.current || !gridData || canvasDimensions.width === 0) return;
    
    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    
    if (!ctx) {
      console.error('Canvas context not available');
      return;
    }
    
    // Draw the grid
    drawGrid(ctx, gridData, cellSize, canvasDimensions, transform);
    
    // Draw the selection highlight if any
    drawSelectionHighlight(ctx, selectedCell, cellSize, transform);
  }, [gridData, transform, selectedCell, cellSize, canvasDimensions]);
  
  // Set up animation loop for smooth rendering
  useEffect(() => {
    // Cancel any existing animation frame
    if (renderRef.current !== null) {
      cancelAnimationFrame(renderRef.current);
    }
    
    // Render immediately once
    renderCanvas();
    
    // Set up animation loop for any future changes
    const animate = () => {
      renderCanvas();
      renderRef.current = requestAnimationFrame(animate);
    };
    
    // Start animation loop
    renderRef.current = requestAnimationFrame(animate);
    
    // Clean up animation frame on unmount
    return () => {
      if (renderRef.current !== null) {
        cancelAnimationFrame(renderRef.current);
      }
    };
  }, [renderCanvas]);
  
  // Set initial cursor style when component mounts
  useEffect(() => {
    if (canvasRef.current) {
      canvasRef.current.style.cursor = 'grab';
    }
  }, []);
  
  return (
    <div ref={containerRef} className={styles.canvasContainer}>
      <canvas
        ref={canvasRef}
        className={styles.canvas}
        tabIndex={0} // Make canvas focusable for keyboard events
        aria-label="Pixel canvas"
        style={{ touchAction: 'none' }} // Disable browser handling of all touch actions
      />
      
      {!gridData && (
        <div className={styles.loadingOverlay}>
          <div className={styles.loadingSpinner}></div>
          <div>Loading canvas...</div>
        </div>
      )}
    </div>
  );
};

export default PixelCanvas;
