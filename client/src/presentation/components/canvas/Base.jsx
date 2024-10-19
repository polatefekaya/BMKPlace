import { useContext } from "react";
import useWindowSize from "../../../application/hooks/useWindowSize";
import { RootContext } from "../../../domain/context/RootContextProvider";
import style from "../../styles/canvas/Canvas.module.css";

export default function CanvasBase({children}){
    const {width, height} = useWindowSize();
    const context = useContext(RootContext);

    const usableHeight = height - context.constants.topNavBarHeight;
    return (
        <>
        <div className={style.container} style={{width: width, height: usableHeight}}>
            {children}
        </div>
        </>
    );
}
