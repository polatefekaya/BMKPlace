import { GridPoint, TransformState, CanvasDimensions } from "../../types/canvas";

/**
 * Draws a single cell on the canvas
 */
export function drawCell(
  ctx: CanvasRenderingContext2D,
  x: number,
  y: number,
  color: string,
  cellSize: number,
  strokeStyle: string = "rgba(0, 0, 0, 0.1)"
): void {
  const cellX = x * cellSize;
  const cellY = y * cellSize;
  
  // Fill the cell with the color
  ctx.fillStyle = color;
  ctx.fillRect(cellX, cellY, cellSize, cellSize);
  
  // Draw a subtle border
  ctx.strokeStyle = strokeStyle;
  ctx.lineWidth = 1;
  ctx.strokeRect(cellX, cellY, cellSize, cellSize);
}

/**
 * Draws the grid on the canvas based on the current transform
 */
export function drawGrid(
  ctx: CanvasRenderingContext2D,
  gridData: string[][],
  cellSize: number,
  canvasDimensions: CanvasDimensions,
  transform: TransformState
): void {
  const { width: canvasWidth, height: canvasHeight } = canvasDimensions;
  
  // Clear the canvas first
  ctx.clearRect(0, 0, canvasWidth, canvasHeight);
  
  // Save the current context state
  ctx.save();
  
  // Apply the current transformation
  ctx.translate(transform.translateX, transform.translateY);
  ctx.scale(transform.scale, transform.scale);
  
  // Calculate the visible grid cell range to avoid unnecessary rendering
  // Add a small buffer to prevent pop-in during fast panning
  const buffer = 3;
  
  const visibleStartX = Math.max(0, Math.floor(-transform.translateX / (cellSize * transform.scale)) - buffer);
  const visibleStartY = Math.max(0, Math.floor(-transform.translateY / (cellSize * transform.scale)) - buffer);
  
  const visibleEndX = Math.min(
    gridData[0].length, 
    Math.ceil((canvasWidth - transform.translateX) / (cellSize * transform.scale)) + buffer
  );
  const visibleEndY = Math.min(
    gridData.length,
    Math.ceil((canvasHeight - transform.translateY) / (cellSize * transform.scale)) + buffer
  );
  
  // Draw only the visible cells
  for (let y = visibleStartY; y < visibleEndY; y++) {
    for (let x = visibleStartX; x < visibleEndX; x++) {
      drawCell(ctx, x, y, gridData[y][x], cellSize);
    }
  }
  
  // Restore the context state
  ctx.restore();
}

/**
 * Draws a highlight around the selected cell
 */
export function drawSelectionHighlight(
  ctx: CanvasRenderingContext2D,
  selectedCell: GridPoint | null,
  cellSize: number,
  transform: TransformState
): void {
  if (!selectedCell) return;
  
  // Save the current context state
  ctx.save();
  
  // Apply the current transformation
  ctx.translate(transform.translateX, transform.translateY);
  ctx.scale(transform.scale, transform.scale);
  
  const cellX = selectedCell.x * cellSize;
  const cellY = selectedCell.y * cellSize;
  
  // Draw a highlighted border around the selected cell
  ctx.strokeStyle = "rgba(255, 255, 255, 0.8)";
  ctx.lineWidth = 2;
  ctx.setLineDash([5, 3]); // Create a dashed line pattern
  ctx.strokeRect(cellX, cellY, cellSize, cellSize);
  
  // Draw a second border for better visibility
  ctx.strokeStyle = "rgba(0, 0, 0, 0.8)";
  ctx.lineWidth = 2;
  ctx.setLineDash([3, 5]);
  ctx.strokeRect(cellX, cellY, cellSize, cellSize);
  
  // Restore the context state
  ctx.restore();
}

/**
 * Gets the canvas dimensions considering devicePixelRatio
 */
export function getCanvasDimensions(
  canvas: HTMLCanvasElement,
  containerWidth: number,
  containerHeight: number
): CanvasDimensions {
  const dpr = window.devicePixelRatio || 1;
  canvas.width = containerWidth * dpr;
  canvas.height = containerHeight * dpr;
  
  // Scale the canvas context to match devicePixelRatio
  const ctx = canvas.getContext("2d");
  if (ctx) {
    ctx.scale(dpr, dpr);
  }
  
  // Set the display size of the canvas
  canvas.style.width = `${containerWidth}px`;
  canvas.style.height = `${containerHeight}px`;
  
  return { width: containerWidth, height: containerHeight };
}
