// NewCanvas.jsx
import React from "react";
import { TransformWrapper, TransformComponent } from "react-zoom-pan-pinch";
import CanvasBase from "../components/canvas/Base";
import Grid from "../components/canvas/Grid";
import ColorPalette from "../components/colorPalette";
import style from "../styles/canvas/Canvas.module.css";

const horizontalBlockCount = 20;
const verticalBlockCount = 20;
const pixelSize = 20;

export default function NewCanvas() {
  const gridWidth = horizontalBlockCount * pixelSize;
  const gridHeight = verticalBlockCount * pixelSize;

  return (
    <>
      <CanvasBase>
        <div
          className={style.canvasContainer}
          style={{
            position: "relative",
            width: "100%",
            height: "100%",
            overflow: "hidden",
          
          }}
        >
          <TransformWrapper
            initialScale={1}
            minScale={0.5}
            maxScale={3}
            centerOnInit={true}
            wheel={{
              step: 0.1,
              wheelEnabled: true,
              touchPadEnabled: true,
            }}
            pinch={{
              step: 0.03,
              disabled: false,
              toggleWheelOnPinch: true, // Added option
              togglePinchOnWheel: true, // Added option
            }}
            doubleClick={{
              disabled: true,
            }}
            panning={{
              disabled: false,
              velocityDisabled: true,
            }}
            preventDefaultTouchmoveEvent={true}
            manipulation={{
              enablePinch: true, // Ensuring pinch gestures are enabled
              enableDoubleClickZoom: false,
              enableWheelZoom: true,
            }}
            onPinchingStart={(e) => {
              // Optional: Debugging or custom behavior
            }}
            onPinching={(e) => {
              // Optional: Debugging or custom behavior
            }}
            onPinchingStop={(e) => {
              // Optional: Debugging or custom behavior
            }}
          >
            {({ zoomIn, zoomOut, resetTransform, centerView, ...rest }) => (
              <>
                <TransformComponent
                  wrapperStyle={{
                    width: "100%",
                    height: "100%",
                  }}
                  contentStyle={{
                    width: "100%",
                    height: "100%",
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center",
                  }}
                >
                  <div
                    style={{
                      width: gridWidth,
                      height: gridHeight,
                    }}
                  >
                    <Grid
                      horizontalBlockCount={horizontalBlockCount}
                      verticalBlockCount={verticalBlockCount}
                      pixelSize={pixelSize}
                    />
                  </div>
                </TransformComponent>
              </>
            )}
          </TransformWrapper>
        </div>
      </CanvasBase>
      <ColorPalette />
    </>
  );
}
