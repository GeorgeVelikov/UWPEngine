using SharpDX;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWPEngine.Shapes;
using UWPEngine.Utility;
using Windows.Storage;

namespace UWPEngine {
    public class MainPageViewModel : NotifyPropertyChangedBase {
        private Device device;
        private Mesh mesh;

        public MainPageViewModel() {
            Device = new Device {
                Scene = new Scene(),
            };

            mesh = new Cube();

            Scene.Meshes.Add(mesh);
        }

        public Device Device {
            get => device;
            set {
                if (device != value) {
                    device = value;
                    OnPropertyChanged(nameof(Device));
                    OnPropertyChanged(nameof(Scene));
                    OnPropertyChanged(nameof(Camera));
                }
            }
        }

        public Scene Scene => Device.Scene;

        public Camera Camera => Device.Scene.Camera;

        // Rendering loop handler
        public void CompositionTarget_Rendering(object sender, object e) {
            Device.Clear(0, 0, 0, 255);

            // rotating slightly the cube during each frame rendered
            mesh.Rotation = new Vector3(
                mesh.Rotation.X + 0.01f,
                mesh.Rotation.Y + 0.01f,
                mesh.Rotation.Z);

            // Doing the various matrix operations
            Device.Render();
            // Flushing the back buffer into the front buffer
            Device.Present();
        }

        public async Task OpenFileAndLoadMesh() {
            StorageFile file = await FileUtility.BrowseForBabylonFile();

            if (file == null) {
                return;
            }

            IList<Mesh> meshes = await FileUtility.ConvertBabylonFileToMesh(file);

            foreach (Mesh mesh in meshes) {
                Device.Scene.Meshes.Add(mesh);
            }
        }
    }
}
