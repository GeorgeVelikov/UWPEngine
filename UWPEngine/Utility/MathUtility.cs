using System;

namespace UWPEngine.Utility {
    public static class MathUtility {
        public static float Clamp(float value, float min = 0, float max = 1) {
            return Math.Max(min, Math.Min(value, max));
        }

        public static float Interpolate(float min, float max, float gradient) {
            return min + (max - min) * Clamp(gradient);
        }
    }
}
