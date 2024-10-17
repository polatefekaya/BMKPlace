// Grid.jsx
import { PixelModel } from "../../domain/models/pixelModel";
import style from "../../styles/canvas/Grid.module.css";
import Pixel from "./Pixel";


function getRandomElement(arr) {
    if (arr.length === 0) return undefined; // Handle empty array
    const randomIndex = Math.floor(Math.random() * arr.length);
    return arr[randomIndex];
  }

export default function Grid({ horizontalBlockCount, verticalBlockCount, pixelSize, borderSize }) {
  const totalCount = horizontalBlockCount * verticalBlockCount;
  
  const pixels = Array.from({ length: totalCount }, () => new PixelModel("#FFFFFF"));

    const hSize = (horizontalBlockCount * pixelSize) + (borderSize * 2);
    const vSize = (verticalBlockCount * pixelSize) + (borderSize * 2);

  const gridStyle = {
    display: "grid",
    gridTemplateColumns: `repeat(${horizontalBlockCount}, ${pixelSize}px)`,
    gridTemplateRows: `repeat(${verticalBlockCount}, ${pixelSize}px)`,
  };

  const outerGridStyle = {
    border: `${borderSize}px solid black`,
    height: `${vSize}`,
    width: `${hSize}`,
  }

  return (
    <div className={style.outerGrid} style={outerGridStyle}>
        <div className={style.container} style={gridStyle}>
        {pixels.map((item, index) => (
            <Pixel key={index} size={pixelSize} backgroundColor={item.color} />
        ))}
        </div>
    </div>
  );
}
