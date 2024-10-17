import React from 'react';
import '../styles/ColorPalette.css';

const COLORS = [
  '#FF4500', '#FFA800', '#FFD635', '#00A368',
  '#7EED56', '#2450A4', '#3690EA', '#51E9F4',
  '#811E9F', '#B44AC0', '#FF99AA', '#9C6926',
  '#000000', '#898D90', '#D4D7D9', '#FFFFFF',
];

function ColorPalette({ colors, selectedColor, onSelectColor }) {
  return (
    <div className="color-palette">
      <div className="color-palette-inner">
        {COLORS.map((color) => (
          <div
            key={color}
            className={`color-swatch ${color === selectedColor ? 'selected' : ''}`}
            style={{ backgroundColor: color }}
            onClick={() => onSelectColor(color)}
          />
        ))}
      </div>
    </div>
  );
}

export default ColorPalette;
