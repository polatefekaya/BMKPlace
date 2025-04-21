import { GridPoint, TransformState } from "../types/canvas";

/**
 * Converts client (screen) coordinates to canvas-relative coordinates
 */
export function getCanvasRelativeCoords(
  clientX: number,
  clientY: number,
  canvas: HTMLCanvasElement
): { x: number, y: number } {
  const rect = canvas.getBoundingClientRect();
  return {
    x: clientX - rect.left,
    y: clientY - rect.top
  };
}

/**
 * Converts canvas-relative coordinates to view coordinates (accounting for pan/zoom)
 */
export function getViewCoords(
  canvasX: number,
  canvasY: number,
  transform: TransformState
): { x: number, y: number } {
  return {
    x: (canvasX - transform.translateX) / transform.scale,
    y: (canvasY - transform.translateY) / transform.scale
  };
}

/**
 * Converts view coordinates to grid coordinates
 */
export function getGridCoords(
  viewX: number,
  viewY: number,
  cellSize: number
): GridPoint {
  return {
    x: Math.floor(viewX / cellSize),
    y: Math.floor(viewY / cellSize)
  };
}

/**
 * Combines the above functions to get grid coordinates directly from client coordinates
 */
export function getGridCoordsFromEvent(
  clientX: number,
  clientY: number,
  canvas: HTMLCanvasElement,
  transform: TransformState,
  cellSize: number
): GridPoint {
  const canvasCoords = getCanvasRelativeCoords(clientX, clientY, canvas);
  const viewCoords = getViewCoords(canvasCoords.x, canvasCoords.y, transform);
  return getGridCoords(viewCoords.x, viewCoords.y, cellSize);
}

/**
 * Checks if a grid point is within bounds
 */
export function isGridPointInBounds(
  point: GridPoint,
  gridWidth: number,
  gridHeight: number
): boolean {
  return (
    point.x >= 0 &&
    point.x < gridWidth &&
    point.y >= 0 &&
    point.y < gridHeight
  );
}

/**
 * Calculate the distance between two points
 */
export function getDistance(
  x1: number,
  y1: number,
  x2: number,
  y2: number
): number {
  return Math.sqrt(Math.pow(x2 - x1, 2) + Math.pow(y2 - y1, 2));
}
