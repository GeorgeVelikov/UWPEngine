using SharpDX;
using System;

namespace UWPEngine.Utility {
    public static class MathUtility {
        public static float Clamp(float value, float min = 0, float max = 1) {
            return Math.Max(min, Math.Min(value, max));
        }

        public static float Interpolate(float min, float max, float gradient) {
            return min + (max - min) * Clamp(gradient);
        }

        public static float CrossProduct2D(float x1, float y1, float x2, float y2) {
            return x1 * y2 - x2 * y1;
        }

        // positive for |> triangles and negative for <|
        public static float LineSide2D(Vector3 point, Vector3 lineFrom, Vector3 lineTo) {
            return CrossProduct2D(
                x1: point.X - lineFrom.X,
                y1: point.Y - lineFrom.Y,
                x2: lineTo.X - lineFrom.X,
                y2: lineTo.Y - lineFrom.Y);
        }
    }
}
