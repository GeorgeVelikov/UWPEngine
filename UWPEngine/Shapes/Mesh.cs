using SharpDX;
using UWPEngine.Structs;

namespace UWPEngine.Shapes {

    public class Mesh : NotifyPropertyChangedBase {
        private string name;
        private Vector3 position;
        private Vector3 rotation;
        private Vector3[] vertices;
        private Face[] faces;

        public Mesh(string name, int verticesCount, int facesCount) {
            Name = name;
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Vertices = new Vector3[verticesCount];
            Faces = new Face[facesCount];
        }

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

        public Vector3[] Vertices {
            get => vertices;
            set {
                if (vertices != value) {
                    vertices = value;
                    OnPropertyChanged(nameof(Vertices));
                }
            }
        }

        public Face[] Faces {
            get => faces;
            set {
                if (faces != value) {
                    faces = value;
                    OnPropertyChanged(nameof(Faces));
                }
            }
        }
    }
}
