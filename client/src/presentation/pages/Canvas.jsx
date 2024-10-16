import React, { useState, useRef, useEffect } from 'react';
import ColorPalette from '../components/colorPalette';
import '../styles/Canvas.css';

const GRID_SIZE = 100; // 100x100 grid
const CELL_SIZE = 20;  // Each pixel is 20x20px

export default function Canvas({ colors }) {
  const [pixels, setPixels] = useState(Array(GRID_SIZE * GRID_SIZE).fill('#FFFFFF'));
  const [selectedColor, setSelectedColor] = useState('#000000');
  const [showPopup, setShowPopup] = useState(false);
  const [clickedPixel, setClickedPixel] = useState(null);
  const canvasRef = useRef(null); // Reference to the canvas element
  const isDragging = useRef(false); // Track if dragging is happening
  const lastPos = useRef({ x: 0, y: 0 }); // Track the last cursor position

  // Handle pixel click only if not dragging
  const handlePixelClick = (row, col) => {
    if (isDragging.current) return; // Prevent clicks while dragging
    setClickedPixel({ row, col });
    setShowPopup(true);
  };

  // Place a pixel and update the state
  const placePixel = () => {
    const { row, col } = clickedPixel;
    const index = row * GRID_SIZE + col;
    const newPixels = [...pixels];
    newPixels[index] = selectedColor;
    setPixels(newPixels);
    setShowPopup(false);
  };

  // Start panning
  const startPan = (e) => {
    isDragging.current = true;
    const { clientX, clientY } = e.touches ? e.touches[0] : e;
    lastPos.current = { x: clientX, y: clientY };
  };

  // Handle the panning movement
  const panCanvas = (e) => {
    if (!isDragging.current) return;
    const { clientX, clientY } = e.touches ? e.touches[0] : e;

    const deltaX = clientX - lastPos.current.x;
    const deltaY = clientY - lastPos.current.y;

    lastPos.current = { x: clientX, y: clientY };

    const canvas = canvasRef.current;
    const style = window.getComputedStyle(canvas);
    const matrix = new DOMMatrixReadOnly(style.transform);

    canvas.style.transform = `translate(${matrix.m41 + deltaX}px, ${matrix.m42 + deltaY}px)`;
  };

  // Stop panning
  const endPan = () => {
    isDragging.current = false;
  };

  return (
    <div className="canvas">
      <div
        className="canvas-container"
        onMouseDown={startPan}
        onMouseMove={panCanvas}
        onMouseUp={endPan}
        onTouchStart={startPan}
        onTouchMove={panCanvas}
        onTouchEnd={endPan}
      >
        <div
          ref={canvasRef}
          className="pixel-grid"
          style={{
            width: GRID_SIZE * CELL_SIZE,
            height: GRID_SIZE * CELL_SIZE,
            transform: 'translate(0px, 0px)',
          }}
        >
          {pixels.map((color, index) => {
            const row = Math.floor(index / GRID_SIZE);
            const col = index % GRID_SIZE;
            return (
              <div
                key={index}
                className="pixel"
                style={{ backgroundColor: color }}
                onClick={() => handlePixelClick(row, col)}
              />
            );
          })}
        </div>

        <ColorPalette
          colors={colors}
          selectedColor={selectedColor}
          onSelectColor={setSelectedColor}
        />

        {showPopup && (
          <div className="place-popup">
            <button onClick={placePixel}>Place</button>
          </div>
        )}
      </div>
    </div>
  );
}

