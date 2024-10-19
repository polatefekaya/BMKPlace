// src/presentation/pages/PerfCanvas.jsx
import React, { useRef, useEffect, useState, useContext } from 'react';
import { TransformWrapper, TransformComponent, useTransformContext } from 'react-zoom-pan-pinch';
import signalRService from '../../application/services/signalRService';
import useWindowSize from '../../application/hooks/useWindowSize';
import ColorPalette from '../components/colorPalette';
import CanvasComponent from '../components/canvas/CanvasComponent';
import style from "../styles/canvas/Canvas.module.css"
import { RootContext } from '../../domain/context/RootContextProvider';

export default function RTCanvas() {
  const canvasRef = useRef(null);
  const [isConnected, setIsConnected] = useState(false);
  const { width, height } = useWindowSize();
  const context = useContext(RootContext);
  const usableHeight = height - context.constants.topNavBarHeight;
  // Grid parameters
  const gridSize = 200;
  const cellSize = 50;

  // Colors array
  const COLORS = [
    '#FF4500', '#FFA800', '#FFD635', '#00A368',
    '#7EED56', '#2450A4', '#3690EA', '#51E9F4',
    '#811E9F', '#B44AC0', '#FF99AA', '#9C6926',
    '#000000', '#898D90', '#D4D7D9', '#FFFFFF',
  ];

  // Selected color index (implement a color picker to change this)
  const [selectedColorIndex, setSelectedColorIndex] = useState(0);

  // Initialize cell state array
  const cells = useRef([]);

  const transformStateRef = useRef({ scale: 1, positionX: 0, positionY: 0 });


  // Drawing functions
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

    // Vertical lines
    for (let x = 0; x <= gridSize; x++) {
      context.beginPath();
      context.moveTo(x * cellSize, 0);
      context.lineTo(x * cellSize, gridSize * cellSize);
    }

    // Horizontal lines
    for (let y = 0; y <= gridSize; y++) {
      context.beginPath();
      context.moveTo(0, y * cellSize);
      context.lineTo(gridSize * cellSize, y * cellSize);
    }
  };

  const drawPixel = (x, y, colorIndex) => {
    const canvas = canvasRef.current;
    const context = canvas.getContext('2d');

    context.fillStyle = COLORS[colorIndex];
    context.fillRect(x * cellSize, y * cellSize, cellSize, cellSize);

    // Optionally redraw grid lines for the pixel
    // context.strokeStyle = '#ccc';
    // context.lineWidth = 0.001;
    // context.strokeRect(x * cellSize, y * cellSize, cellSize, cellSize);
  };

  useEffect(() => {
    // Initialize cells
    cells.current = Array.from({ length: gridSize }, () =>
      Array.from({ length: gridSize }, () => Math.floor(Math.random() * 15)) // Default to white color index
    );

    // Set canvas dimensions
    const canvas = canvasRef.current;
    if (!canvas) return;
    canvas.width = gridSize * cellSize;
    canvas.height = gridSize * cellSize;
    canvas.style.width = `${gridSize * cellSize}px`;
    canvas.style.height = `${gridSize * cellSize}px`;

    // Draw initial grid
    drawGrid();

    // Start SignalR connection
    signalRService.startConnection().then(() => {
      setIsConnected(true);

      // Fetch initial canvas state from the server
      signalRService.invoke('GetCanvasState');

      // Listen for initial canvas state
      signalRService.on('ReceiveCanvasState', (data) => {
        cells.current = data;
        drawGrid();
      });

      // Listen for pixel updates
      signalRService.on('ReceivePixelUpdate', ({ x, y, colorIndex }) => {
        cells.current[x][y] = colorIndex;
        drawPixel(x, y, colorIndex);
      });
    });

    // Cleanup on unmount
    return () => {
      signalRService.off('ReceiveCanvasState');
      signalRService.off('ReceivePixelUpdate');
    };
  }, []);

  const sendPixelUpdate = (x, y, colorIndex) => {
    signalRService.invoke('SendPixelUpdate', { x, y, colorIndex });
  };

  const handleClick = (e) => {
    e.preventDefault();
    e.stopPropagation();
  
    const { scale, positionX, positionY } = transformStateRef.current;
  
    console.log(`Transformations: positionX=${positionX}, positionY=${positionY}, scale=${scale}`);
  
    const canvas = canvasRef.current;
    const rect = canvas.getBoundingClientRect();
  
    // Get mouse position relative to canvas
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;
  
    // Adjust for transformations
    const contentX = (x - positionX) / scale;
    const contentY = (y - positionY) / scale;
  
    // Calculate grid cell indices
    const cellX = Math.floor(contentX / cellSize);
    const cellY = Math.floor(contentY / cellSize);
  
    console.log(`Clicked cell: (${cellX}, ${cellY})`);
    console.log(`Mouse screen position: (${e.clientX}, ${e.clientY})`);
    console.log(`Canvas position: (${rect.left}, ${rect.top})`);
    console.log(`Mouse canvas position: (${x}, ${y})`);
    console.log(`Content coordinates: (${contentX}, ${contentY})`);
    console.log(`Grid cell indices: (${cellX}, ${cellY})`);
  
    if (
      cellX >= 0 &&
      cellX < gridSize &&
      cellY >= 0 &&
      cellY < gridSize
    ) {
      cells.current[cellX][cellY] = selectedColorIndex;
      drawPixel(cellX, cellY, selectedColorIndex);
      sendPixelUpdate(cellX, cellY, selectedColorIndex);
    }
  };
  
  

  return (
    <div style={{ width: width, height: usableHeight, overflow: 'hidden' }}>

      <TransformWrapper
        initialScale={1}
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
        limitToBounds={false}
        onZoom={(ref) => {
          console.log('onZoom event:', ref);
          transformStateRef.current = {
            scale: ref.state.scale,
            positionX: ref.state.positionX,
            positionY: ref.state.positionY,
          };
        }}
        onPanning={(ref) => {
          console.log('onPanning event:', ref);
          transformStateRef.current = {
            scale: ref.state.scale,
            positionX: ref.state.positionX,
            positionY: ref.state.positionY,
          };
        }}
        onPinching={(ref) => {
          console.log('onPinching event:', ref);
          transformStateRef.current = {
            scale: ref.state.scale,
            positionX: ref.state.positionX,
            positionY: ref.state.positionY,
          };
        }}
      >
        {(transformProps) => {
          console.log('TransformWrapper render prop parameters:', transformProps);
          const { scale, positionX, positionY } = transformProps.instance.transformState;

          const handleState = () => {
            console.log('DEBUG FLAG\n BOUNDS: ', transformProps.instance.bounds, '\n', 'TRANS STATE: ', transformProps.instance.transformState);
          }

          const handleClick = (e) => {
            e.preventDefault();
            e.stopPropagation();

            console.log(`Transformations: positionX=${positionX}, positionY=${positionY}, scale=${scale}`);

            const canvas = canvasRef.current;
            const rect = canvas.getBoundingClientRect();

            // Get mouse position relative to canvas
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            console.log(`Mouse screen position: (${e.clientX}, ${e.clientY})`);
            console.log(`Canvas position: (${rect.left}, ${rect.top})`);
            console.log(`Mouse canvas position: (${x}, ${y})`);

            // Adjust for transformations
            const contentX = (x - positionX) / scale;
            const contentY = (y - positionY) / scale;

            console.log(`Content coordinates: (${contentX}, ${contentY})`);

            // Calculate grid cell indices
            const cellX = Math.floor(contentX / cellSize);
            const cellY = Math.floor(contentY / cellSize);

            console.log(`Grid cell indices: (${cellX}, ${cellY})`);

            if (
              cellX >= 0 &&
              cellX < gridSize &&
              cellY >= 0 &&
              cellY < gridSize
            ) {
              cells.current[cellX][cellY] = selectedColorIndex;
              drawPixel(cellX, cellY, selectedColorIndex);
              sendPixelUpdate(cellX, cellY, selectedColorIndex);
            } else {
              console.warn(`Clicked outside the grid boundaries: (${cellX}, ${cellY})`);
            }
          };
          return(
            <TransformComponent
              wrapperStyle={{
                width: '100%',
                height: '100%',
                touchAction: 'none',
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
              }}
              
            >
                <h1 className={style.canvasHeader}>BMK Place</h1>
              <canvas
                ref={canvasRef}
                style={{
                  border: '1px solid black',
                  display: 'block',
                  width: `${gridSize * cellSize}px`,
                  height: `${gridSize * cellSize}px`,
                }}
                onClick={handleClick}
                
              />
              {/* <CanvasComponent canvasRef={canvasRef} handleClick={handleClick} gridSize={gridSize} cellSize={cellSize}/> */}
            </TransformComponent>)
        }}
      </TransformWrapper>
      {/* Include your ColorPalette component and pass setSelectedColorIndex */}
      <ColorPalette setSelectedColorIndex={setSelectedColorIndex} />
    </div>
  );
}
