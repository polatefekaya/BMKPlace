import React, { useState, useRef, useEffect, useCallback } from 'react';
import { FixedSizeGrid as Grid } from 'react-window';
import { useGesture } from '@use-gesture/react';
import { useSpring, animated } from 'react-spring';
import ColorPalette from '../components/colorPalette';
import '../styles/Canvas.css';

const GRID_SIZE = 100;
const CELL_SIZE = 20;

function Canvas({ colors }) {
  const [pixels, setPixels] = useState(Array(GRID_SIZE * GRID_SIZE).fill('#FFFFFF'));
  const [selectedColor, setSelectedColor] = useState('#000000');
  const [showPopup, setShowPopup] = useState(false);
  const [clickedPixel, setClickedPixel] = useState(null);

  const [{ x, y, scale }, api] = useSpring(() => ({ x: 0, y: 0, scale: 1 }));
  const canvasRef = useRef(null);

  // Dynamically calculate the canvas size based on window dimensions.
  const [canvasHeight, setCanvasHeight] = useState(window.innerHeight - 120);

  const updateCanvasSize = useCallback(() => {
    setCanvasHeight(window.innerHeight - 120); // Subtract space for palette and navbar
  }, []);

  useEffect(() => {
    window.addEventListener('resize', updateCanvasSize);
    return () => window.removeEventListener('resize', updateCanvasSize);
  }, [updateCanvasSize]);

  // Handle pinch-to-zoom and pan gestures.
  const bind = useGesture({
    onDrag: ({ offset: [ox, oy] }) => api.start({ x: ox, y: oy }),
    onPinch: ({ offset: [s] }) => api.start({ scale: s }),
    onWheel: ({ ctrlKey, delta: [, dy] }) => {
      if (ctrlKey) {
        const newScale = Math.min(Math.max(scale.get() - dy * 0.001, 0.5), 3);
        api.start({ scale: newScale });
      }
    },
  });

  const handlePixelClick = (rowIndex, colIndex) => {
    setClickedPixel({ row: rowIndex, col: colIndex });
    setShowPopup(true);
  };

  const placePixel = () => {
    const { row, col } = clickedPixel;
    const index = row * GRID_SIZE + col;
    const newPixels = [...pixels];
    newPixels[index] = selectedColor;
    setPixels(newPixels);
    setShowPopup(false);
  };

  const Cell = ({ columnIndex, rowIndex, style }) => {
    const index = rowIndex * GRID_SIZE + columnIndex;
    return (
      <div
        className="pixel-cell"
        style={{ ...style, backgroundColor: pixels[index] }}
        onClick={() => handlePixelClick(rowIndex, columnIndex)}
      />
    );
  };

  return (
    <div className="canvas-container">
      <animated.div
        {...bind()}
        style={{
          x,
          y,
          scale,
          width: GRID_SIZE * CELL_SIZE,
          height: GRID_SIZE * CELL_SIZE,
          touchAction: 'none', // Prevent default touch scrolling
        }}
      >
        <Grid
          ref={canvasRef}
          columnCount={GRID_SIZE}
          rowCount={GRID_SIZE}
          columnWidth={CELL_SIZE}
          rowHeight={CELL_SIZE}
          width={window.innerWidth}
          height={canvasHeight}
        >
          {Cell}
        </Grid>
      </animated.div>

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
  );
}

export default Canvas;
