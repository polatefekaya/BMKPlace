import CanvasBase from "../components/canvas/Base";
import Grid from "../components/canvas/Grid";
import ColorPalette from "../components/colorPalette";

export default function NewCanvas(){
    return (
        <>
            <CanvasBase>
            <Grid horizontalBlockCount={10} verticalBlockCount={10} pixelSize={10}>

            </Grid>
            </CanvasBase>
            <ColorPalette/>
        </>
    );
}