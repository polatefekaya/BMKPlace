// Canvas configuration
export const GRID_WIDTH = 100;
export const GRID_HEIGHT = 100;
export const CELL_SIZE = 20;

// Interaction constraints
export const MIN_ZOOM = 0.5;
export const MAX_ZOOM = 10;
export const RATE_LIMIT_MS = 5000; // 5 seconds cooldown between pixel placements

// Interaction thresholds
export const PAN_THRESHOLD_PX = 5; // Minimum pixels moved to trigger pan vs tap
export const PAN_THRESHOLD_MS = 200; // Maximum ms to consider a quick tap vs hold
