// NewCanvas.jsx
import CanvasBase from "../components/canvas/Base";
import Grid from "../components/canvas/Grid";
import ColorPalette from "../components/colorPalette";
import React, { useRef, useState, useEffect } from "react";
import style from "../styles/canvas/Canvas.module.css";

const horizontalBlockCount = 20;
const verticalBlockCount = 20;
const pixelSize = 20;

export default function NewCanvas() {
  const canvasContainerRef = useRef(null);
  const canvasRef = useRef(null);
  const isDragging = useRef(false);
  const lastPos = useRef({ x: 0, y: 0 });

  const [transform, setTransform] = useState({ x: 0, y: 0, scale: 1 });

  // Center the canvas within its parent on mount
  useEffect(() => {
    const canvasContainer = canvasContainerRef.current;
    const canvas = canvasRef.current;
    if (canvasContainer && canvas) {
      const containerRect = canvasContainer.getBoundingClientRect();
      const canvasRect = canvas.getBoundingClientRect();

      // Center the canvas within its parent
      const offsetX = (containerRect.width - canvasRect.width) / 2;
      const offsetY = (containerRect.height - canvasRect.height) / 2;

      canvas.style.position = 'absolute';
      canvas.style.left = `${offsetX}px`;
      canvas.style.top = `${offsetY}px`;
    }
  }, []);

  // Handle starting panning
  const startPan = (e) => {
    e.preventDefault();
    isDragging.current = true;
    const { clientX, clientY } = e.touches ? e.touches[0] : e;
    lastPos.current = { x: clientX, y: clientY };
  };

  // Handle the panning movement
  const panCanvas = (e) => {
    e.preventDefault();
    if (!isDragging.current) return;
    const { clientX, clientY } = e.touches ? e.touches[0] : e;

    const deltaX = clientX - lastPos.current.x;
    const deltaY = clientY - lastPos.current.y;

    lastPos.current = { x: clientX, y: clientY };

    setTransform((prev) => ({
      ...prev,
      x: prev.x + deltaX,
      y: prev.y + deltaY,
    }));
  };

  // Stop panning
  const endPan = (e) => {
    e.preventDefault();
    isDragging.current = false;
  };

  // Handle wheel zoom (desktop)
  const handleWheel = (e) => {
    e.preventDefault();
    const zoomIntensity = 0.1;
    const { offsetX, offsetY } = e.nativeEvent;

    let newScale = transform.scale + (e.deltaY > 0 ? -zoomIntensity : zoomIntensity);
    newScale = Math.max(0.5, Math.min(newScale, 3)); // Clamp the scale between 0.5 and 3

    const scaleFactor = newScale / transform.scale;

    const newX = (offsetX - transform.x) - (offsetX - transform.x) * scaleFactor + transform.x;
    const newY = (offsetY - transform.y) - (offsetY - transform.y) * scaleFactor + transform.y;

    setTransform({
      x: newX,
      y: newY,
      scale: newScale,
    });
  };

  // Handle pinch zoom (touch devices)
  const pinchZoomData = useRef({
    initialDistance: null,
    initialScale: transform.scale,
    initialMidpoint: null,
  });

  const handleTouchMove = (e) => {
    if (e.touches.length === 1 && isDragging.current) {
      panCanvas(e);
    } else if (e.touches.length === 2) {
      e.preventDefault();
      const touch1 = e.touches[0];
      const touch2 = e.touches[1];

      const currentDistance = getDistance(touch1, touch2);
      const midpoint = getMidpoint(touch1, touch2);

      if (pinchZoomData.current.initialDistance === null) {
        // Start pinch zoom
        pinchZoomData.current.initialDistance = currentDistance;
        pinchZoomData.current.initialScale = transform.scale;
        pinchZoomData.current.initialMidpoint = midpoint;
      } else {
        const scaleFactor = currentDistance / pinchZoomData.current.initialDistance;
        let newScale = pinchZoomData.current.initialScale * scaleFactor;
        newScale = Math.max(0.5, Math.min(newScale, 3)); // Clamp the scale

        const canvas = canvasRef.current;
        const rect = canvas.getBoundingClientRect();

        const canvasX = midpoint.x - rect.left;
        const canvasY = midpoint.y - rect.top;

        const scaleChange = newScale / transform.scale;

        const newX = (canvasX - transform.x) - (canvasX - transform.x) * scaleChange + transform.x;
        const newY = (canvasY - transform.y) - (canvasY - transform.y) * scaleChange + transform.y;

        setTransform({
          x: newX,
          y: newY,
          scale: newScale,
        });
      }
    }
  };

  const handleTouchEnd = (e) => {
    if (e.touches.length < 2) {
      pinchZoomData.current.initialDistance = null;
      pinchZoomData.current.initialScale = transform.scale;
      pinchZoomData.current.initialMidpoint = null;
    }
    endPan(e);
  };

  // Utility functions
  const getDistance = (touch1, touch2) => {
    return Math.hypot(
      touch2.clientX - touch1.clientX,
      touch2.clientY - touch1.clientY
    );
  };

  const getMidpoint = (touch1, touch2) => {
    return {
      x: (touch1.clientX + touch2.clientX) / 2,
      y: (touch1.clientY + touch2.clientY) / 2,
    };
  };

  // Apply the combined transform (translation + scaling)
  const transformStyle = {
    transform: `translate(${transform.x}px, ${transform.y}px) scale(${transform.scale})`,
    transformOrigin: '0 0', // Ensure transforms are applied from the top-left corner
    position: 'absolute',
  };

  // Center the grid within the canvas
  const gridContainerStyle = {
    position: 'relative',
    width: `${horizontalBlockCount * pixelSize}px`,
    height: `${verticalBlockCount * pixelSize}px`,
  };

  return (
    <>
      <CanvasBase>
        <div
          className={style.canvasContainer}
          ref={canvasContainerRef}
          style={{ position: 'relative', width: '100%', height: '100%' }}
        >
          <div
            className={style.innerContainer}
            ref={canvasRef}
            style={transformStyle}
            onMouseDown={startPan}
            onMouseMove={panCanvas}
            onMouseUp={endPan}
            onMouseLeave={endPan}
            onWheel={handleWheel}
            onTouchStart={startPan}
            onTouchMove={handleTouchMove}
            onTouchEnd={handleTouchEnd}
          >
            <div style={gridContainerStyle}>
              <Grid
                horizontalBlockCount={horizontalBlockCount}
                verticalBlockCount={verticalBlockCount}
                pixelSize={pixelSize}
              />
            </div>
          </div>
        </div>
      </CanvasBase>
      <ColorPalette />
    </>
  );
}
