import style from "../../styles/canvas/Pixel.module.css"

export default function Pixel({size}) {
    return (
        <>
            <div className={style.container} style={{height: size, width: size}}>
            
            </div>
        </>
    );
}