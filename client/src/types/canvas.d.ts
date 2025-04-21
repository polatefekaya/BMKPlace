export interface GridPoint {
  x: number;
  y: number;
}

export interface TransformState {
  scale: number;
  translateX: number;
  translateY: number;
}

export interface PixelData {
  x: number;
  y: number;
  color: string;
}

export interface CanvasDimensions {
  width: number;
  height: number;
}

export interface PanPoint {
  x: number;
  y: number;
  time: number;
}

export interface PointerData {
  id: number;
  x: number;
  y: number;
}
