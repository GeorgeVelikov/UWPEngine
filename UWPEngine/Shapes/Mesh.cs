using SharpDX;
using UWPEngine.Structs;

namespace UWPEngine.Shapes {

    public class Mesh : NotifyPropertyChangedBase {
        private string name;
        private Vector3 position;
        private Vector3 rotation;

        public Mesh(string name, int verticesCount, int facesCount) {
            Name = name;
            Vertices = new Vector3[verticesCount];
            Faces = new Face[facesCount];
        }

        public Vector3[] Vertices { get; private set; }
        public Face[] Faces { get; set; }

        public string Name {
            get => name;
            set {
                if (!string.IsNullOrWhiteSpace(value) && name != value) {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public Vector3 Position {
            get => position;
            set {
                if (value != null && position != value) {
                    position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        public Vector3 Rotation {
            get => rotation;
            set {
                if (value != null && rotation != value) {
                    rotation = value;
                    OnPropertyChanged(nameof(Rotation));
                }
            }
        }
    }
}
