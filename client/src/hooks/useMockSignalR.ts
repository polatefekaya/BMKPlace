import { useState, useRef, useEffect } from "react";
import { PixelData } from "../types/canvas";
import { GRID_WIDTH, GRID_HEIGHT, RATE_LIMIT_MS } from "../config/constants";
import { PALETTE_COLORS } from "../config/colors";

type CellUpdateCallback = (update: PixelData) => void;

export function useMockSignalR() {
  // State for rate limiting
  const [isRateLimited, setIsRateLimited] = useState<boolean>(false);
  const [rateLimitEndTime, setRateLimitEndTime] = useState<number>(0);
  
  // Use ref for the grid state to avoid triggering renders from the hook itself
  const gridStateRef = useRef<string[][]>([]);
  
  // Refs for callback and interval for simulated updates
  const callbackRef = useRef<CellUpdateCallback | null>(null);
  const updateIntervalRef = useRef<NodeJS.Timeout | null>(null);
  
  // Initialize a grid with all white cells
  const initializeEmptyGrid = (): string[][] => {
    const grid: string[][] = [];
    for (let y = 0; y < GRID_HEIGHT; y++) {
      const row: string[] = [];
      for (let x = 0; x < GRID_WIDTH; x++) {
        // All cells are white initially
        row.push(PALETTE_COLORS[0]); // White
      }
      grid.push(row);
    }
    return grid;
  };
  
  // Clean up the update interval on unmount
  useEffect(() => {
    return () => {
      if (updateIntervalRef.current) {
        clearInterval(updateIntervalRef.current);
      }
    };
  }, []);
  
  // Update the rate limit countdown timer if active
  useEffect(() => {
    if (!isRateLimited) return;
    
    const intervalId = setInterval(() => {
      const now = Date.now();
      if (now >= rateLimitEndTime) {
        setIsRateLimited(false);
        clearInterval(intervalId);
      }
    }, 100);
    
    return () => clearInterval(intervalId);
  }, [isRateLimited, rateLimitEndTime]);
  
  /**
   * Simulates fetching the initial grid state
   */
  const getInitialGrid = async (): Promise<string[][]> => {
    // Simulate network delay
    await new Promise(resolve => setTimeout(resolve, 500));
    
    // Initialize the grid if it doesn't exist
    if (gridStateRef.current.length === 0) {
      gridStateRef.current = initializeEmptyGrid();
    }
    
    // Return a deep copy to avoid direct mutation
    return JSON.parse(JSON.stringify(gridStateRef.current));
  };
  
  /**
   * Places a pixel on the canvas and applies rate limiting
   */
  const placePixel = async (x: number, y: number, color: string): Promise<boolean> => {
    // Check if rate limited
    if (isRateLimited) return false;
    
    // Validate coordinates and color
    if (
      x < 0 || x >= GRID_WIDTH ||
      y < 0 || y >= GRID_HEIGHT ||
      !PALETTE_COLORS.includes(color)
    ) {
      return false;
    }
    
    // Update the grid state
    gridStateRef.current[y][x] = color;
    
    // Apply rate limiting
    setIsRateLimited(true);
    const endTime = Date.now() + RATE_LIMIT_MS;
    setRateLimitEndTime(endTime);
    
    // Schedule the reset of rate limiting
    setTimeout(() => {
      setIsRateLimited(false);
    }, RATE_LIMIT_MS);
    
    // If we have a callback registered, notify it about the update
    if (callbackRef.current) {
      callbackRef.current({ x, y, color });
    }
    
    return true;
  };
  
  /**
   * Subscribes to cell updates and returns a cleanup function
   */
  const subscribeToCellUpdates = (callback: CellUpdateCallback): (() => void) => {
    // Store the callback
    callbackRef.current = callback;
    
    // Set up interval for random simulated updates from "other users"
    updateIntervalRef.current = setInterval(() => {
      // Generate a random update
      const randomX = Math.floor(Math.random() * GRID_WIDTH);
      const randomY = Math.floor(Math.random() * GRID_HEIGHT);
      const randomColorIndex = Math.floor(Math.random() * PALETTE_COLORS.length);
      const randomColor = PALETTE_COLORS[randomColorIndex];
      
      // Update the grid state
      gridStateRef.current[randomY][randomX] = randomColor;
      
      // Notify the callback
      if (callbackRef.current) {
        callbackRef.current({
          x: randomX,
          y: randomY,
          color: randomColor
        });
      }
    }, 2000); // Generate a random update every 2 seconds
    
    // Return cleanup function
    return () => {
      callbackRef.current = null;
      if (updateIntervalRef.current) {
        clearInterval(updateIntervalRef.current);
        updateIntervalRef.current = null;
      }
    };
  };
  
  return {
    getInitialGrid,
    placePixel,
    subscribeToCellUpdates,
    isRateLimited,
    rateLimitEndTime
  };
}
