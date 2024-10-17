// Grid.jsx
import { PixelModel } from "../../domain/models/pixelModel";
import style from "../../styles/canvas/Grid.module.css";
import Pixel from "./Pixel";

export default function Grid({ horizontalBlockCount, verticalBlockCount, pixelSize }) {
  const totalCount = horizontalBlockCount * verticalBlockCount;
  const pixels = Array.from({ length: totalCount }, () => new PixelModel("#ffffff"));

  const gridStyle = {
    display: "grid",
    gridTemplateColumns: `repeat(${horizontalBlockCount}, ${pixelSize}px)`,
    gridTemplateRows: `repeat(${verticalBlockCount}, ${pixelSize}px)`,
  };

  return (
    <div className={style.container} style={gridStyle}>
      {pixels.map((item, index) => (
        <Pixel key={index} size={pixelSize} backgroundColor={item.color} />
      ))}
    </div>
  );
}
