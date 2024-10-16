import { useContext } from "react";
import useWindowSize from "../../../application/hooks/useWindowSize";
import { RootContext } from "../../domain/context/RootContextProvider";
import "../../styles/Canvas.css";

export default function CanvasBase({children}){
    const {width, height} = useWindowSize();
    const context = useContext(RootContext);

    const usableHeight = height - context.constants.topNavBarHeight;
    return (
        <>
        <div className="canvas" style={{width: width, height: usableHeight, border: "10px solid black"}}>
            {children}
        </div>
        </>
    );
}
