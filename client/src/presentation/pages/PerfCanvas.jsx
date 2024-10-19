// CanvasGrid.jsx
import React, { useRef, useEffect } from 'react';
import { TransformWrapper, TransformComponent } from 'react-zoom-pan-pinch';

export default function CanvasGrid() {
  const canvasRef = useRef(null);

  // Grid parameters
  const gridSize = 200;
  const cellSize = 50; // Adjusted to keep the canvas size manageable

  // Colors array
  const COLORS = [
    '#FF4500', '#FFA800', '#FFD635', '#00A368',
    '#7EED56', '#2450A4', '#3690EA', '#51E9F4',
    '#811E9F', '#B44AC0', '#FF99AA', '#9C6926',
    '#000000', '#898D90', '#D4D7D9', '#FFFFFF',
  ];

  // Initialize cell state array with random colors
  const cells = useRef([]);

  // Drawing function
  const drawGrid = () => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    const context = canvas.getContext('2d');
    context.clearRect(0, 0, canvas.width, canvas.height);

    // Draw filled cells
    for (let x = 0; x < gridSize; x++) {
      for (let y = 0; y < gridSize; y++) {
        const colorIndex = cells.current[x][y];
        if (typeof colorIndex === 'number') {
          context.fillStyle = COLORS[colorIndex];
          context.fillRect(
            x * cellSize,
            y * cellSize,
            cellSize,
            cellSize
          );
        }
      }
    }

    // Draw grid lines
    context.strokeStyle = '#ccc';
    context.lineWidth = 0.01; // Adjusted line width for better visibility

    // Vertical lines
    for (let x = 0; x <= gridSize; x++) {
      context.beginPath();
      context.moveTo(x * cellSize, 0);
      context.lineTo(x * cellSize, gridSize * cellSize);
      context.stroke();
    }

    // Horizontal lines
    for (let y = 0; y <= gridSize; y++) {
      context.beginPath();
      context.moveTo(0, y * cellSize);
      context.lineTo(gridSize * cellSize, y * cellSize);
      context.stroke();
    }
  };

  // Initialize cells and set canvas dimensions
  useEffect(() => {
    // Initialize the cells array
    const initCells = [];
    for (let x = 0; x < gridSize; x++) {
      initCells[x] = [];
      for (let y = 0; y < gridSize; y++) {
        // Assign a random color index to each cell
        initCells[x][y] = Math.floor(Math.random() * COLORS.length);
      }
    }
    cells.current = initCells;

    // Set canvas dimensions
    const canvas = canvasRef.current;
    if (!canvas) return;
    canvas.width = gridSize * cellSize;
    canvas.height = gridSize * cellSize;

    // Set canvas display size
    canvas.style.width = `${gridSize * cellSize}px`;
    canvas.style.height = `${gridSize * cellSize}px`;

    // Draw the grid after initializing
    drawGrid();
  }, []); // Empty dependency array ensures this runs once on mount

  return (
    <div style={{ width: '100%', height: '100%', overflow: 'hidden' }}>
      <TransformWrapper
        initialScale={0.5}
        initialPositionX={0}
        initialPositionY={0}
        minScale={0.1}
        maxScale={10}
        centerOnInit={false}
        wheel={{
          wheelEnabled: true,
          touchPadEnabled: true,
          step: 0.1,
        }}
        pinch={{
          disabled: false,
        }}
        panning={{
          disabled: false,
          velocityDisabled: true,
        }}
        doubleClick={{
          disabled: true,
        }}
        alignmentAnimation={{
          sizeX: 0,
          sizeY: 0,
        }}
        limitToBounds={false} // Allow panning beyond the bounds
      >
        {({ scale, positionX, positionY }) => {
          // Define the onClick handler here, where `scale`, `positionX`, and `positionY` are accessible
          const handleClick = (e) => {
            const canvas = canvasRef.current;
            const rect = canvas.getBoundingClientRect();

            // Adjust for transformations
            const mouseX = (e.clientX - rect.left - positionX) / scale;
            const mouseY = (e.clientY - rect.top - positionY) / scale;

            const cellX = Math.floor(mouseX / cellSize);
            const cellY = Math.floor(mouseY / cellSize);

            if (
              cellX >= 0 &&
              cellX < gridSize &&
              cellY >= 0 &&
              cellY < gridSize
            ) {
              // Assign a new random color index to the cell
              cells.current[cellX][cellY] = Math.floor(Math.random() * COLORS.length);
              drawGrid(); // Redraw the grid to reflect changes
            }
          };

          return (
            <TransformComponent
              wrapperStyle={{
                width: '100%',
                height: '100%',
                touchAction: 'none', // Prevent default touch actions
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
              }}
            >
              <canvas
                ref={canvasRef}
                style={{
                  border: '1px solid black',
                  display: 'block',
                  width: `${gridSize * cellSize}px`,
                  height: `${gridSize * cellSize}px`,
                }}
                onClick={handleClick} // Pass the handler here
              />
            </TransformComponent>
          );
        }}
      </TransformWrapper>
    </div>
  );
}
