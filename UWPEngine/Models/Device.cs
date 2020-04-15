using Amplifier;
using SharpDX;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWPEngine.OpenCL;
using UWPEngine.Structs;
using UWPEngine.Utility;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPEngine.Models {
    public class Device : NotifyPropertyChangedBase {
        private Scene scene;

        private OpenCLCompiler compilerGpu;
        private Amplifier.Device physicalGpu;
        private dynamic executionEngine;
        private object vertex;

        public Device() {
            SetupGpu();
        }

        #region Scene Properties

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

        #endregion

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

        public void Clear(byte r, byte g, byte b, byte a) {
            executionEngine.ClearBackBuffer(BackBuffer, r, g, b, a);
            executionEngine.ClearDepthBuffer(DepthBuffer);
        }

        // The main method of the engine that re-compute each vertex projection during each frame
        public void Render() {
            // To understand this part, please read the prerequisites resources
            Matrix viewMatrix = Matrix.LookAtLH(Scene.Camera.Position, Scene.Camera.Target, Vector3.UnitY);

            Matrix projectionMatrix =
                Matrix.PerspectiveFovRH(0.78f, (float)BitmapWidth / BitmapHeight, 0.01f, 1.0f);

            Parallel.ForEach(Scene.Meshes, mesh => {
                // Beware to apply rotation before translation
                Matrix worldMatrix =
                    Matrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z)
                    * Matrix.Translation(mesh.Position);

                Matrix transformMatrix = worldMatrix * viewMatrix * projectionMatrix;

                Parallel.ForEach(mesh.Faces, face => {
                    Vertex vertexA = Project(mesh.Vertices[face.VertexA], transformMatrix, worldMatrix);
                    Vertex vertexB = Project(mesh.Vertices[face.VertexB], transformMatrix, worldMatrix);
                    Vertex vertexC = Project(mesh.Vertices[face.VertexC], transformMatrix, worldMatrix);

                    // White being the most lit area
                    DrawTriangle(vertexA, vertexB, vertexC, Color4.White);
                });
            });
        }

        // Once everything is ready, we can flush the back buffer into the front buffer.
        public void Present() {

            using (Stream stream = Bitmap.PixelBuffer.AsStream()) {
                stream.Write(BackBuffer, 0, BackBuffer.Length);
            }

            // request a redraw of the entire bitmap
            Bitmap.Invalidate();
        }

        #region Rendering methods

        private void ProcessScanLine(ScanLineData data, Vertex vertexA, Vertex vertexB, Vertex vertexC, Vertex vertexD, Color4 color) {
            Vector3 pointA = vertexA.Coordinates;
            Vector3 pointB = vertexB.Coordinates;
            Vector3 pointC = vertexC.Coordinates;
            Vector3 pointD = vertexD.Coordinates;

            // Thanks to current Y, we can compute the gradient to compute others values like
            // the starting X (sx) and ending X (ex) to draw between
            // if pa.Y == pb.Y or pc.Y == pd.Y, gradient is forced to 1
            var gradient1 = pointA.Y != pointB.Y
                ? (data.CurrentY - pointA.Y) / (pointB.Y - pointA.Y)
                : 1;

            var gradient2 = pointC.Y != pointD.Y
                ? (data.CurrentY - pointC.Y) / (pointD.Y - pointC.Y)
                : 1;

            int startingX = (int)MathUtility.Interpolate(pointA.X, pointB.X, gradient1);
            int endingX = (int)MathUtility.Interpolate(pointC.X, pointD.X, gradient2);

            float startingZ = MathUtility.Interpolate(pointA.Z, pointB.Z, gradient1);
            float endingZ = MathUtility.Interpolate(pointC.Z, pointD.Z, gradient2);

            // drawing a line from left (sx) to right (ex)
            for (int currentX = startingX; currentX < endingX; currentX++) {
                float gradient = (currentX - startingX) / (float)(endingX - startingX);

                float z = MathUtility.Interpolate(startingZ, endingZ, gradient);

                DrawPoint(new Vector3(currentX, data.CurrentY, z), color * data.DotA);
            }
        }

        // Project takes some 3D coordinates and transform them in 2D coordinates using the transformation matrix
        private Vertex Project(Vertex vertex, Matrix transMat, Matrix world) {
            // transforming the coordinates into 2D space
            Vector3 point2d = Vector3.TransformCoordinate(vertex.Coordinates, transMat);

            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            float x = point2d.X * BitmapWidth + BitmapWidth / 2.0f;
            float y = -point2d.Y * BitmapHeight + BitmapHeight / 2.0f;

            // transforming the coordinates & the normal to the vertex in the 3D world
            // The transformed coordinates will be based on coordinate system
            return new Vertex {
                Coordinates = new Vector3(x, y, -point2d.Z),
                Normal = Vector3.TransformCoordinate(vertex.Normal, world),
                WorldCoordinates = Vector3.TransformCoordinate(vertex.Coordinates, world),
            };
        }

        // Called to put a pixel on screen at a specific X,Y coordinates
        private void PutPixel(int x, int y, float z, Color4 color) {
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
        private void DrawPoint(Vector3 point, Color4 color) {
            // Clipping what's visible on screen
            if (point.X >= 0
                && point.Y >= 0
                && point.X < BitmapWidth
                && point.Y < BitmapHeight) {

                PutPixel((int)point.X, (int)point.Y, point.Z, color);
            }
        }

        private void DrawTriangle(Vertex vertexA, Vertex vertexB, Vertex vertexC, Color4 color) {
            // Sorting the vertexs in order to always have this order on screen p1, p2 & p3
            // with p1 always up (thus having the Y the lowest possible to be near the top screen)
            // then p2 between p1 & p3
            if (vertexA.Coordinates.Y > vertexB.Coordinates.Y) {
                (vertexA, vertexB) = (vertexB, vertexA);
            }

            if (vertexB.Coordinates.Y > vertexC.Coordinates.Y) {
                (vertexB, vertexC) = (vertexC, vertexB);
            }

            if (vertexA.Coordinates.Y > vertexB.Coordinates.Y) {
                (vertexA, vertexB) = (vertexB, vertexA);
            }

            Vector3 pointA = vertexA.Coordinates;
            Vector3 pointB = vertexB.Coordinates;
            Vector3 pointC = vertexC.Coordinates;

            // Light position
            Vector3 lightPosition = new Vector3(0, 5, 5);

            float lightDotA = MathUtility.ComputeNDotL(vertexA.WorldCoordinates, vertexA.Normal, lightPosition);
            float lightDotB = MathUtility.ComputeNDotL(vertexB.WorldCoordinates, vertexB.Normal, lightPosition);
            float lightDotC = MathUtility.ComputeNDotL(vertexC.WorldCoordinates, vertexC.Normal, lightPosition);
            // computing the cos of the angle between the light vector and the normal vector
            // it will return a value between 0 and 1 that will be used as the intensity of the color
            ScanLineData data = new ScanLineData();

            // Case where triangles are |>
            if (MathUtility.LineSide2D(pointB, pointA, pointC) > 0) {
                for (var y = (int)pointA.Y; y <= (int)pointC.Y; y++) {
                    data.CurrentY = y;

                    if (y < pointB.Y) {
                        data.DotA = lightDotA;
                        data.DotB = lightDotC;
                        data.DotC = lightDotA;
                        data.DotD = lightDotB;
                        ProcessScanLine(data, vertexA, vertexC, vertexA, vertexB, color);
                    } else {
                        data.DotA = lightDotA;
                        data.DotB = lightDotC;
                        data.DotC = lightDotB;
                        data.DotD = lightDotC;
                        ProcessScanLine(data, vertexA, vertexC, vertexB, vertexC, color);
                    }
                }
            }
            // Case where triangles are <|
            else {
                for (var y = (int)pointA.Y; y <= (int)pointC.Y; y++) {
                    data.CurrentY = y;

                    if (y < pointB.Y) {
                        data.DotA = lightDotA;
                        data.DotB = lightDotB;
                        data.DotC = lightDotA;
                        data.DotD = lightDotC;
                        ProcessScanLine(data, vertexA, vertexB, vertexA, vertexC, color);
                    } else {
                        data.DotA = lightDotB;
                        data.DotB = lightDotC;
                        data.DotC = lightDotA;
                        data.DotD = lightDotC;
                        ProcessScanLine(data, vertexB, vertexC, vertexA, vertexC, color);
                    }
                }
            }
        }

        #endregion
    }
}
