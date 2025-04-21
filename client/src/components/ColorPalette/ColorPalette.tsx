import React, { useMemo, useState, useEffect } from 'react';
import styles from './ColorPalette.module.css';

interface ColorPaletteProps {
  availableColors: readonly string[];
  selectedColor: string | null;
  onColorSelect: (color: string) => void;
  isRateLimited: boolean;
  rateLimitEndTime?: number;
}

const ColorPalette: React.FC<ColorPaletteProps> = ({
  availableColors,
  selectedColor,
  onColorSelect,
  isRateLimited,
  rateLimitEndTime
}) => {
  // Calculate time remaining for rate limit
  const timeRemaining = useMemo(() => {
    if (!isRateLimited || !rateLimitEndTime) return 0;
    return Math.max(0, Math.ceil((rateLimitEndTime - Date.now()) / 1000));
  }, [isRateLimited, rateLimitEndTime]);

  // State to track if we're on a mobile device
  const [isMobile, setIsMobile] = useState(false);

  // Check if we're on a mobile device
  useEffect(() => {
    const checkMobile = () => {
      setIsMobile(window.innerWidth <= 768);
    };
    
    // Initial check
    checkMobile();
    
    // Add event listener for window resize
    window.addEventListener('resize', checkMobile);
    
    // Cleanup
    return () => window.removeEventListener('resize', checkMobile);
  }, []);

  return (
    <div className={styles.paletteContainer}>
      {isRateLimited && (
        <div className={styles.cooldownMessage}>
          Wait {timeRemaining}s to place next pixel
        </div>
      )}
      
      <div className={styles.colorsGrid}>
        {availableColors.map((color) => (
          <button
            key={color}
            className={`${styles.colorButton} ${selectedColor === color ? styles.selected : ''}`}
            style={{ backgroundColor: color }}
            onClick={() => onColorSelect(color)}
            disabled={isRateLimited}
            aria-label={`Select ${color} color`}
            aria-pressed={selectedColor === color}
            title={color}
          />
        ))}
      </div>
    </div>
  );
};

export default ColorPalette;
