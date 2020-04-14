using Amplifier;
using SharpDX;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWPEngine.Shapes;
using UWPEngine.Utility;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPEngine {
    public class Device : NotifyPropertyChangedBase {
        private Scene scene;

        private OpenCLCompiler compilerGpu;
        private Amplifier.Device physicalGpu;
        private dynamic executionEngine;

        public Device() {
            SetupGpu();
        }

        private void SetupGpu() {
            compilerGpu = new OpenCLCompiler();

            physicalGpu = compilerGpu.Devices
                .Where(d => d.Type == DeviceType.GPU)
                .FirstOrDefault();

            if (physicalGpu == null) {
                return;
            }

            compilerGpu.UseDevice(physicalGpu.ID);

            compilerGpu.CompileKernel(typeof(GpuKernel));

            executionEngine = compilerGpu.GetExec();
        }

        public Scene Scene {
            get => scene;
            set {
                if (value?.Bitmap == null || (!value?.BackBuffer?.Any() ?? true) ) {
                    return;
                }

                if (scene != value) {
                    scene = value;
                    OnPropertyChanged(null);
                }
            }
        }

        public WriteableBitmap Bitmap => Scene.Bitmap;

        public int BitmapWidth => Scene.BitmapWidth;

        public int BitmapHeight => Scene.BitmapHeight;

        public byte[] BackBuffer => Scene.BackBuffer;

        public float[] DepthBuffer => Scene.DepthBuffer;

        public object[] LockBuffer => Scene.LockBuffer;

        public void Clear(byte r, byte g, byte b, byte a) {
            executionEngine.ClearBackBuffer(BackBuffer, r, g, b, a);
            executionEngine.ClearDepthBuffer(DepthBuffer);
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

        void ProcessScanLine(int y, Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD, Color4 color) {
            // Thanks to current Y, we can compute the gradient to compute others values like
            // the starting X (sx) and ending X (ex) to draw between
            // if pa.Y == pb.Y or pc.Y == pd.Y, gradient is forced to 1
            var gradient1 = pointA.Y != pointB.Y
                ? (y - pointA.Y) / (pointB.Y - pointA.Y)
                : 1;

            var gradient2 = pointC.Y != pointD.Y
                ? (y - pointC.Y) / (pointD.Y - pointC.Y)
                : 1;

            int startingX = (int)MathUtility.Interpolate(pointA.X, pointB.X, gradient1);
            int endingX = (int)MathUtility.Interpolate(pointC.X, pointD.X, gradient2);

            float startingZ = MathUtility.Interpolate(pointA.Z, pointB.Z, gradient1);
            float endingZ = MathUtility.Interpolate(pointC.Z, pointD.Z, gradient2);

            // drawing a line from left (sx) to right (ex)
            for (int currentX = startingX; currentX < endingX; currentX++) {
                float gradient = (currentX - startingX) / (float)(endingX - startingX);

                float z = MathUtility.Interpolate(startingZ, endingZ, gradient);

                DrawPoint(new Vector3(currentX, y, z), color);
            }
        }

        // Project takes some 3D coordinates and transform them in 2D coordinates using the transformation matrix
        public Vector3 Project(Vector3 coord, Matrix transMat) {
            // transforming the coordinates
            Vector3 point = Vector3.TransformCoordinate(coord, transMat);
            // The transformed coordinates will be based on coordinate system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            float x = point.X * BitmapWidth + BitmapWidth / 2.0f;
            float y = -point.Y * BitmapHeight + BitmapHeight / 2.0f;

            return (new Vector3(x, y, point.Z));
        }

        // Called to put a pixel on screen at a specific X,Y coordinates
        public void PutPixel(int x, int y, float z, Color4 color) {
            // As we have a 1-D Array for our back buffer
            // we need to know the equivalent cell in 1-D based
            // on the 2D coordinates on screen
            int depthBufferIndex = x + y * BitmapWidth;
            int backBufferIndex = depthBufferIndex * 4;

            lock (LockBuffer[depthBufferIndex]) {

                if (DepthBuffer[depthBufferIndex] < z) {
                    return;
                }

                DepthBuffer[depthBufferIndex] = z;

                BackBuffer[backBufferIndex] = (byte)(color.Blue * 255);
                BackBuffer[backBufferIndex + 1] = (byte)(color.Green * 255);
                BackBuffer[backBufferIndex + 2] = (byte)(color.Red * 255);
                BackBuffer[backBufferIndex + 3] = (byte)(color.Alpha * 255);
            }
        }

        // DrawPoint calls PutPixel but does the clipping operation before
        public void DrawPoint(Vector3 point, Color4 color) {
            // Clipping what's visible on screen
            if (point.X >= 0
                && point.Y >= 0
                && point.X < BitmapWidth
                && point.Y < BitmapHeight) {

                PutPixel((int)point.X, (int)point.Y, point.Z, color);
            }
        }

        //public void DrawLine(Vector2 pointFrom, Vector2 pointTo, Color4 color) {
        //    // Bresenham’s line algorithm
        //    int fromX = (int)pointFrom.X;
        //    int fromY = (int)pointFrom.Y;
        //    int toX = (int)pointTo.X;
        //    int toY = (int)pointTo.Y;

        //    int dx = Math.Abs(toX - fromX);
        //    int dy = Math.Abs(toY - fromY);
        //    int sx = (fromX < toX) ? 1 : -1;
        //    int sy = (fromY < toY) ? 1 : -1;
        //    int err = dx - dy;

        //    while (true) {
        //        DrawPoint(new Vector2(fromX, fromY), color);

        //        if ((fromX == toX) && (fromY == toY)) {
        //            break;
        //        }

        //        int e2 = 2 * err;

        //        if (e2 > -dy) {
        //            err -= dy;
        //            fromX += sx;
        //        }

        //        if (e2 < dx) {
        //            err += dx;
        //            fromY += sy;
        //        }
        //    }
        //}

        public void DrawTriangle(Vector3 pointA, Vector3 pointB, Vector3 pointC, Color4 color) {
            // Sorting the points in order to always have this order on screen p1, p2 & p3
            // with p1 always up (thus having the Y the lowest possible to be near the top screen)
            // then p2 between p1 & p3
            if (pointA.Y > pointB.Y) {
                (pointA, pointB) = (pointB, pointA);
            }

            if (pointB.Y > pointC.Y) {
                (pointB, pointC) = (pointC, pointB);
            }

            if (pointA.Y > pointB.Y) {
                (pointA, pointB) = (pointB, pointA);
            }

            // Case where triangles are |>
            if (MathUtility.LineSide2D(pointB, pointA, pointC) > 0) {
                for (var y = (int)pointA.Y; y <= (int)pointC.Y; y++) {
                    if (y < pointB.Y) {
                        ProcessScanLine(y, pointA, pointC, pointA, pointB, color);
                    } else {
                        ProcessScanLine(y, pointA, pointC, pointB, pointC, color);
                    }
                }
            }
            // Case where triangles are <|
            else {
                for (var y = (int)pointA.Y; y <= (int)pointC.Y; y++) {
                    if (y < pointB.Y) {
                        ProcessScanLine(y, pointA, pointB, pointA, pointC, color);
                    } else {
                        ProcessScanLine(y, pointB, pointC, pointA, pointC, color);
                    }
                }
            }
        }

        // The main method of the engine that re-compute each vertex projection during each frame
        public void Render() {
            // To understand this part, please read the prerequisites resources
            Matrix viewMatrix = Matrix.LookAtLH(Scene.Camera.Position, Scene.Camera.Target, Vector3.UnitY);

            Matrix projectionMatrix =
                Matrix.PerspectiveFovRH(0.78f, (float)BitmapWidth / BitmapHeight, 0.01f, 1.0f);

            foreach (Mesh mesh in Scene.Meshes) {
                // Beware to apply rotation before translation
                Matrix worldMatrix =
                    Matrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z)
                    * Matrix.Translation(mesh.Position);

                Matrix transformMatrix = worldMatrix * viewMatrix * projectionMatrix;

                int meshTrianglesCount = mesh.Triangles.Length;

                Parallel.For(0, mesh.Triangles.Length, faceIndex => {
                    Triangle face = mesh.Triangles[faceIndex];

                    Vector3 vertexA = mesh.Vertices[face.VertexA];
                    Vector3 vertexB = mesh.Vertices[face.VertexB];
                    Vector3 vertexC = mesh.Vertices[face.VertexC];

                    Vector3 pointA = Project(vertexA, transformMatrix);
                    Vector3 pointB = Project(vertexB, transformMatrix);
                    Vector3 pointC = Project(vertexC, transformMatrix);

                    float color = 0.25f + (faceIndex++ % meshTrianglesCount) * 0.75f / meshTrianglesCount;
                    DrawTriangle(pointA, pointB, pointC, new Color4(color, color, color, 1));
                });
            }
        }
    }
}
