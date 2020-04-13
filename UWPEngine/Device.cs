using SharpDX;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPEngine.Shapes;
using UWPEngine.Utility;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPEngine {
    public class Device : NotifyPropertyChangedBase {
        private Scene scene;

        public Device() {
        }

        public Scene Scene {
            get => scene;
            set {
                if (value?.Bitmap == null || (!value?.BackBuffer?.Any() ?? true) ) {
                    return;
                }

                if (scene != value) {
                    scene = value;
                    OnPropertyChanged(nameof(Scene));
                    OnPropertyChanged(nameof(BackBuffer));
                    OnPropertyChanged(nameof(Bitmap));
                }
            }
        }

        public WriteableBitmap Bitmap => Scene.Bitmap;

        public byte[] BackBuffer => Scene.BackBuffer;


        public void Clear(byte r, byte g, byte b, byte a) {
            for (var index = 0; index < BackBuffer.Length; index += 4) {
                // BGRA is used by Windows instead by RGBA in HTML5
                BackBuffer[index] = b;
                BackBuffer[index + 1] = g;
                BackBuffer[index + 2] = r;
                BackBuffer[index + 3] = a;
            }
        }

        // Once everything is ready, we can flush the back buffer
        // into the front buffer.
        public void Present() {

            using (Stream stream = Bitmap.PixelBuffer.AsStream()) {
                stream.Write(BackBuffer, 0, BackBuffer.Length);
            }

            // request a redraw of the entire bitmap
            Bitmap.Invalidate();
        }

        // Called to put a pixel on screen at a specific X,Y coordinates
        public void PutPixel(int x, int y, Color4 color) {
            // As we have a 1-D Array for our back buffer
            // we need to know the equivalent cell in 1-D based
            // on the 2D coordinates on screen
            int index = (x + y * Bitmap.PixelWidth) * 4;

            BackBuffer[index] = (byte)(color.Blue * 255);
            BackBuffer[index + 1] = (byte)(color.Green * 255);
            BackBuffer[index + 2] = (byte)(color.Red * 255);
            BackBuffer[index + 3] = (byte)(color.Alpha * 255);
        }

        // Project takes some 3D coordinates and transform them
        // in 2D coordinates using the transformation matrix
        public Vector2 Project(Vector3 coord, Matrix transMat) {
            // transforming the coordinates
            Vector3 point = Vector3.TransformCoordinate(coord, transMat);
            // The transformed coordinates will be based on coordinate system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            float x = point.X * Bitmap.PixelWidth + Bitmap.PixelWidth / 2.0f;
            float y = -point.Y * Bitmap.PixelHeight + Bitmap.PixelHeight / 2.0f;

            return (new Vector2(x, y));
        }

        // DrawPoint calls PutPixel but does the clipping operation before
        public void DrawPoint(Vector2 point) {
            // Clipping what's visible on screen
            if (point.X >= 0
                && point.Y >= 0
                && point.X < Bitmap.PixelWidth
                && point.Y < Bitmap.PixelHeight) {

                PutPixel((int)point.X, (int)point.Y, new Color4(1.0f, 1.0f, 1.0f, 1.0f));
            }
        }

        public void DrawLine(Vector2 pointFrom, Vector2 pointTo) {
            // Bresenham’s line algorithm

            int fromX = (int)pointFrom.X;
            int fromY = (int)pointFrom.Y;
            int toX = (int)pointTo.X;
            int toY = (int)pointTo.Y;

            int dx = Math.Abs(toX - fromX);
            int dy = Math.Abs(toY - fromY);
            int sx = (fromX < toX) ? 1 : -1;
            int sy = (fromY < toY) ? 1 : -1;
            int err = dx - dy;

            while (true) {
                DrawPoint(new Vector2(fromX, fromY));

                if ((fromX == toX) && (fromY == toY)) {
                    break;
                }

                int e2 = 2 * err;

                if (e2 > -dy) {
                    err -= dy;
                    fromX += sx;
                }

                if (e2 < dx) {
                    err += dx;
                    fromY += sy;
                }
            }
        }

        // The main method of the engine that re-compute each vertex projection
        // during each frame
        public void Render() {
            // To understand this part, please read the prerequisites resources
            Matrix viewMatrix = Matrix.LookAtLH(Scene.Camera.Position, Scene.Camera.Target, Vector3.UnitY);

            Matrix projectionMatrix =
                Matrix.PerspectiveFovRH(0.78f, (float)Bitmap.PixelWidth / Bitmap.PixelHeight, 0.01f, 1.0f);

            foreach (Mesh mesh in Scene.Meshes) {
                // Beware to apply rotation before translation
                Matrix worldMatrix =
                    Matrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z)
                    * Matrix.Translation(mesh.Position);

                Matrix transformMatrix = worldMatrix * viewMatrix * projectionMatrix;

                foreach (var face in mesh.Triangles) {
                    Vector3 vertexA = mesh.Vertices[face.VertexA];
                    Vector3 vertexB = mesh.Vertices[face.VertexB];
                    Vector3 vertexC = mesh.Vertices[face.VertexC];

                    Vector2 pointA = Project(vertexA, transformMatrix);
                    Vector2 pointB = Project(vertexB, transformMatrix);
                    Vector2 pointC = Project(vertexC, transformMatrix);

                    DrawLine(pointA, pointB);
                    DrawLine(pointB, pointC);
                    DrawLine(pointC, pointA);
                }
            }
        }
    }
}
