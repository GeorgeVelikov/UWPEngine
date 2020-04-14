using System.Collections.ObjectModel;
using UWPEngine.Shapes;
using UWPEngine.Utility;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPEngine {
    public class Scene : NotifyPropertyChangedBase {
        private readonly byte[] backBuffer;
        private readonly float[] depthBuffer;
        private readonly object[] lockBuffer;

        private Camera camera;
        private ObservableCollection<Mesh> meshes;
        private WriteableBitmap bitmap;

        public Scene() {
            Camera = new Camera();
            Meshes = new ObservableCollection<Mesh>();
            Bitmap = new WriteableBitmap(720, 480);
            BitmapWidth = bitmap.PixelWidth;
            BitmapHeight = bitmap.PixelHeight;

            // the back buffer size is equal to the number of pixels to draw
            // on screen (width*height) * 4 (R,G,B & Alpha values).
            backBuffer = new byte[Bitmap.PixelWidth * Bitmap.PixelHeight * 4];
            depthBuffer = new float[Bitmap.PixelWidth * Bitmap.PixelHeight];
            lockBuffer = new object[Bitmap.PixelWidth * Bitmap.PixelHeight];

            for (var i = 0; i < lockBuffer.Length; i++) {
                lockBuffer[i] = new object();
            }
        }

        public byte[] BackBuffer => backBuffer;

        public float[] DepthBuffer => depthBuffer;

        public object[] LockBuffer => lockBuffer;

        public int BitmapWidth { get; set; }
        public int BitmapHeight { get; set; }

        public Camera Camera {
            get => camera;
            set {
                if (camera != value) {
                    camera = value;
                    OnPropertyChanged(nameof(Camera));
                }
            }
        }

        public ObservableCollection<Mesh> Meshes {
            get => meshes;
            set {
                if (meshes != value) {
                    meshes = value;
                    OnPropertyChanged(nameof(Meshes));
                }
            }
        }

        public WriteableBitmap Bitmap {
            get => bitmap;
            set {
                if (bitmap != value) {
                    bitmap = value;
                    OnPropertyChanged(nameof(Bitmap));
                }
            }
        }

        public void AddMesh(Mesh mesh) => Meshes.Add(mesh);
        public void RemoveMesh(Mesh mesh) => Meshes.Remove(mesh);
        public void ClearScene() => Meshes.Clear();
    }
}
