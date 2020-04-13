using System.Collections.ObjectModel;
using UWPEngine.Shapes;
using UWPEngine.Utility;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPEngine {
    public class Scene : NotifyPropertyChangedBase {
        private readonly byte[] backBuffer;

        private ObservableCollection<Mesh> meshes;
        private WriteableBitmap bitmap;

        public Scene() {
            Meshes = new ObservableCollection<Mesh>();
            Bitmap = new WriteableBitmap(720, 480);

            // the back buffer size is equal to the number of pixels to draw
            // on screen (width*height) * 4 (R,G,B & Alpha values).
            backBuffer = new byte[Bitmap.PixelWidth * Bitmap.PixelHeight * 4];
        }

        public byte[] BackBuffer => backBuffer;

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
