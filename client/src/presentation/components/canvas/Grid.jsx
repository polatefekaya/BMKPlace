import { PixelModel } from "../../domain/models/pixelModel";
import style from "../../styles//canvas/Grid.module.css"
import Pixel from "./Pixel";

export default function Grid({horizontalBlockCount, verticalBlockCount, pixelSize}){
    const totalCount = horizontalBlockCount * verticalBlockCount;
    const pixels = Array(totalCount).fill(new PixelModel());
return (
    <>
        <div className={style.container}>
        <p>{totalCount}</p>
            {pixels.map((item, index) => (
                <div key={index}
                     
                >
                <Pixel size={pixelSize}></Pixel>
                </div>
            ))}
        </div>
    </>
);
}