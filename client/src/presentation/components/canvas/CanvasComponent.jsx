import { useTransformContext } from 'react-zoom-pan-pinch';

export default function CanvasComponent({ canvasRef, handleClick, gridSize, cellSize }) {
  const { scale, positionX, positionY } = useTransformContext();

  const onClick = (e) => {
    handleClick(e, { scale, positionX, positionY });
  };

  return (
    <canvas
      ref={canvasRef}
      style={{
        border: '1px solid black',
        display: 'block',
        width: `${gridSize * cellSize}px`,
        height: `${gridSize * cellSize}px`,
      }}
      onClick={onClick}
    />
  );
}
