import { TransformState } from "../types/canvas";
import { MIN_ZOOM, MAX_ZOOM } from "../config/constants";
import { getCanvasRelativeCoords, getViewCoords } from "./coordinateUtils";

/**
 * Clamps transform values to prevent panning outside of valid bounds
 * and zooming beyond min/max limits
 */
export function clampTransform(
  transform: TransformState,
  gridWidth: number,
  gridHeight: number,
  cellSize: number,
  canvasWidth: number,
  canvasHeight: number
): TransformState {
  // Clamp scale within min/max zoom limits
  const clampedScale = Math.max(MIN_ZOOM, Math.min(MAX_ZOOM, transform.scale));
  
  // Calculate the scaled grid dimensions
  const scaledGridWidth = gridWidth * cellSize * clampedScale;
  const scaledGridHeight = gridHeight * cellSize * clampedScale;
  
  // Calculate the maximum allowable translation to keep the grid visible
  // If the scaled grid is smaller than the canvas, center it
  // Otherwise, prevent panning beyond the edges
  // Add some padding to allow for smooth panning
  const padding = 50; // Added padding in pixels
  
  const maxTranslateX = Math.max(padding, (canvasWidth - scaledGridWidth) / 2 + padding);
  const minTranslateX = Math.min(-padding, canvasWidth - scaledGridWidth + (canvasWidth - scaledGridWidth) / 2 - padding);
  
  const maxTranslateY = Math.max(padding, (canvasHeight - scaledGridHeight) / 2 + padding);
  const minTranslateY = Math.min(-padding, canvasHeight - scaledGridHeight + (canvasHeight - scaledGridHeight) / 2 - padding);
  
  // Apply the constraints
  const clampedTranslateX = Math.max(minTranslateX, Math.min(maxTranslateX, transform.translateX));
  const clampedTranslateY = Math.max(minTranslateY, Math.min(maxTranslateY, transform.translateY));
  
  return {
    scale: clampedScale,
    translateX: clampedTranslateX,
    translateY: clampedTranslateY
  };
}

/**
 * Calculates new transform when zooming, ensuring the zoom is centered on the pointer
 */
export function calculateZoomedTransform(
  prevTransform: TransformState,
  zoomFactor: number,
  clientX: number,
  clientY: number,
  canvasElement: HTMLCanvasElement
): TransformState {
  // Get canvas-relative coordinates of the pointer
  const canvasRelativeCoords = getCanvasRelativeCoords(clientX, clientY, canvasElement);
  
  // Get the view coordinates of the pointer before zoom
  const viewCoordsBefore = getViewCoords(
    canvasRelativeCoords.x,
    canvasRelativeCoords.y,
    prevTransform
  );
  
  // Calculate new scale (clamping will be applied by the caller)
  const newScale = prevTransform.scale * zoomFactor;
  
  // Calculate new translations that keep the pointer position fixed in the view
  const newTranslateX = canvasRelativeCoords.x - viewCoordsBefore.x * newScale;
  const newTranslateY = canvasRelativeCoords.y - viewCoordsBefore.y * newScale;
  
  return {
    scale: newScale,
    translateX: newTranslateX,
    translateY: newTranslateY
  };
}
