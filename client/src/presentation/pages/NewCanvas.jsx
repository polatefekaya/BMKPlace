// NewCanvas.jsx
import React from "react";
import { TransformWrapper, TransformComponent } from "react-zoom-pan-pinch";
import CanvasBase from "../components/canvas/Base";
import Grid from "../components/canvas/Grid";
import ColorPalette from "../components/colorPalette";
import style from "../styles/canvas/Canvas.module.css";

const horizontalBlockCount = 100;
const verticalBlockCount = 100;
const pixelSize = 10;

const BORDER_SIZE = 2;

export default function NewCanvas() {
  const gridWidth = (horizontalBlockCount * pixelSize) + BORDER_SIZE * 2;
  const gridHeight = (verticalBlockCount * pixelSize) + BORDER_SIZE * 2;

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
            minScale={0.1}
            maxScale={5}
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
                    <h6 className={style.header}>BMK PLACE</h6>
                    <Grid
                      horizontalBlockCount={horizontalBlockCount}
                      verticalBlockCount={verticalBlockCount}
                      pixelSize={pixelSize}
                      borderSize={BORDER_SIZE}
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
