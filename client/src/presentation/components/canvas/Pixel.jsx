import style from "../../styles/canvas/Pixel.module.css"

export default function Pixel({size, backgroundColor}) {
    return (
        <>
            <div className={style.container} style={{height: size, width: size, backgroundColor: backgroundColor}}>
            
            </div>
        </>
    );
}