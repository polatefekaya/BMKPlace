import { useRef, useEffect, MutableRefObject } from "react";
import { GridPoint, TransformState, PanPoint, PointerData } from "../types/canvas";
import { getGridCoordsFromEvent, getDistance } from "../utils/coordinateUtils";
import { PAN_THRESHOLD_PX, PAN_THRESHOLD_MS } from "../config/constants";

interface UseCanvasInteractionProps {
  canvasRef: MutableRefObject<HTMLCanvasElement | null>;
  cellSize: number;
  transform: TransformState;
  isInteractionDisabled?: boolean;
  onCellSelect: (point: GridPoint) => void;
  onPan: (dx: number, dy: number) => void;
  onZoom: (dScale: number, clientX: number, clientY: number) => void;
}

export function useCanvasInteraction({
  canvasRef,
  cellSize,
  transform,
  isInteractionDisabled = false,
  onCellSelect,
  onPan,
  onZoom
}: UseCanvasInteractionProps) {
  // Refs to track interaction state without triggering re-renders
  const isPanningRef = useRef<boolean>(false);
  const panStartPointRef = useRef<PanPoint | null>(null);
  
  // For multi-touch gestures
  const isPinchingRef = useRef<boolean>(false);
  const pinchStartDistanceRef = useRef<number>(0);
  const pointerCacheRef = useRef<Map<number, PointerData>>(new Map());
  
  // Cleanup function reference
  const cleanupRef = useRef<(() => void) | null>(null);
  
  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    
    // Set touch-action to none to prevent default browser behaviors
    canvas.style.touchAction = "none";
    
    // Pointer event handlers for pan and tap
    const handlePointerDown = (event: PointerEvent) => {
      if (isInteractionDisabled) return;
      
      // Capture the pointer to ensure we get all events
      if (canvas.hasPointerCapture(event.pointerId) === false) {
        canvas.setPointerCapture(event.pointerId);
      }
      
      // Store the pointer data
      pointerCacheRef.current.set(event.pointerId, {
        id: event.pointerId,
        x: event.clientX,
        y: event.clientY
      });
      
      // If this is the first pointer, store it as potential pan start
      if (pointerCacheRef.current.size === 1) {
        isPanningRef.current = false;
        panStartPointRef.current = {
          x: event.clientX,
          y: event.clientY,
          time: Date.now()
        };
      } 
      // If this is the second pointer, start pinch
      else if (pointerCacheRef.current.size === 2) {
        const pointers = Array.from(pointerCacheRef.current.values());
        const distance = getDistance(
          pointers[0].x, pointers[0].y,
          pointers[1].x, pointers[1].y
        );
        isPinchingRef.current = true;
        pinchStartDistanceRef.current = distance;
        
        // Cancel any ongoing pan when starting a pinch
        isPanningRef.current = false;
      }
      
      event.preventDefault();
    };
    
    const handlePointerMove = (event: PointerEvent) => {
      if (isInteractionDisabled) return;
      
      // Update the pointer data
      if (pointerCacheRef.current.has(event.pointerId)) {
        pointerCacheRef.current.set(event.pointerId, {
          id: event.pointerId,
          x: event.clientX,
          y: event.clientY
        });
      }
      
      // Handle pinch-to-zoom with two pointers
      if (isPinchingRef.current && pointerCacheRef.current.size === 2) {
        const pointers = Array.from(pointerCacheRef.current.values());
        const currentDistance = getDistance(
          pointers[0].x, pointers[0].y, 
          pointers[1].x, pointers[1].y
        );
        
        // Calculate zoom factor based on pinch distance change
        if (pinchStartDistanceRef.current > 0) {
          const zoomFactor = currentDistance / pinchStartDistanceRef.current;
          
          // Only apply zoom if the change is significant
          if (Math.abs(zoomFactor - 1) > 0.01) {
            // Calculate pinch center
            const centerX = (pointers[0].x + pointers[1].x) / 2;
            const centerY = (pointers[0].y + pointers[1].y) / 2;
            
            // Directly apply zoom with the pinch center as the focal point
            // without using requestAnimationFrame to ensure immediate update
            onZoom(zoomFactor, centerX, centerY);
            
            // Reset the start distance for continuous zooming
            pinchStartDistanceRef.current = currentDistance;
          }
        }
        
        event.preventDefault();
        return;
      }
      
      // Handle panning with one pointer
      if (pointerCacheRef.current.size === 1 && panStartPointRef.current) {
        const deltaX = event.clientX - panStartPointRef.current.x;
        const deltaY = event.clientY - panStartPointRef.current.y;
        const deltaTime = Date.now() - panStartPointRef.current.time;
        
        // Determine if we've moved enough to consider this a pan
        if (!isPanningRef.current && 
            (Math.abs(deltaX) > PAN_THRESHOLD_PX || 
             Math.abs(deltaY) > PAN_THRESHOLD_PX || 
             deltaTime > PAN_THRESHOLD_MS)) {
          isPanningRef.current = true;
          
          // Change cursor to indicate grabbing
          if (canvas) {
            canvas.style.cursor = 'grabbing';
          }
        }
        
        // If we're panning, apply the translation
        if (isPanningRef.current) {
          // Directly apply the pan without going through requestAnimationFrame
          // to ensure immediate state update
          onPan(deltaX, deltaY);
          
          // Update the start point for continuous panning
          panStartPointRef.current = {
            x: event.clientX,
            y: event.clientY,
            time: Date.now()
          };
          
          event.preventDefault();
        }
      }
    };
    
    const handlePointerUp = (event: PointerEvent) => {
      if (isInteractionDisabled) return;
      
      // Release pointer capture
      if (canvas.hasPointerCapture(event.pointerId)) {
        canvas.releasePointerCapture(event.pointerId);
      }
      
      // Reset cursor to grab
      if (canvas) {
        canvas.style.cursor = 'grab';
      }
      
      // If this was a tap (not a pan), select a cell
      if (pointerCacheRef.current.size === 1 && 
          !isPanningRef.current && 
          panStartPointRef.current) {
        const gridPoint = getGridCoordsFromEvent(
          event.clientX,
          event.clientY,
          canvas,
          transform,
          cellSize
        );
        onCellSelect(gridPoint);
      }
      
      // Remove this pointer from the cache
      pointerCacheRef.current.delete(event.pointerId);
      
      // Reset pinching state if no more pointers
      if (pointerCacheRef.current.size < 2) {
        isPinchingRef.current = false;
      }
      
      // Reset panning state if no more pointers
      if (pointerCacheRef.current.size === 0) {
        isPanningRef.current = false;
        panStartPointRef.current = null;
      }
      
      event.preventDefault();
    };
    
    const handlePointerCancel = (event: PointerEvent) => {
      // Clean up on cancel
      pointerCacheRef.current.delete(event.pointerId);
      
      if (pointerCacheRef.current.size < 2) {
        isPinchingRef.current = false;
      }
      
      if (pointerCacheRef.current.size === 0) {
        isPanningRef.current = false;
        panStartPointRef.current = null;
      }
      
      event.preventDefault();
    };
    
    // Handle wheel events for zoom
    const handleWheel = (event: WheelEvent) => {
      if (isInteractionDisabled) return;
      
      event.preventDefault();
      
      // Determine zoom direction and factor
      const delta = -Math.sign(event.deltaY);
      // Use a smaller zoom factor for smoother zooming
      const zoomFactor = delta > 0 ? 1.05 : 0.95;
      
      // Directly apply the zoom without requestAnimationFrame
      // to ensure immediate state update
      onZoom(zoomFactor, event.clientX, event.clientY);
    };
    
    // Prevent context menu on right-click
    const handleContextMenu = (event: MouseEvent) => {
      event.preventDefault();
      return false;
    };
    
    // Attach event listeners
    canvas.addEventListener('pointerdown', handlePointerDown);
    canvas.addEventListener('pointermove', handlePointerMove);
    canvas.addEventListener('pointerup', handlePointerUp);
    canvas.addEventListener('pointercancel', handlePointerCancel);
    canvas.addEventListener('wheel', handleWheel);
    canvas.addEventListener('contextmenu', handleContextMenu);
    
    // Store cleanup function
    cleanupRef.current = () => {
      canvas.removeEventListener('pointerdown', handlePointerDown);
      canvas.removeEventListener('pointermove', handlePointerMove);
      canvas.removeEventListener('pointerup', handlePointerUp);
      canvas.removeEventListener('pointercancel', handlePointerCancel);
      canvas.removeEventListener('wheel', handleWheel);
      canvas.removeEventListener('contextmenu', handleContextMenu);
    };
    
    // Cleanup on unmount
    return () => {
      if (cleanupRef.current) {
        cleanupRef.current();
      }
    };
  }, [canvasRef, cellSize, transform, isInteractionDisabled, onCellSelect, onPan, onZoom]);
}
