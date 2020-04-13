using SharpDX;
using UWPEngine.Utility;

namespace UWPEngine.Shapes {

    public struct Triangle {
        public int VertexA;
        public int VertexB;
        public int VertexC;
    }

    public class Mesh : NotifyPropertyChangedBase {
        private string name;
        private Vector3 position;
        private Vector3 rotation;

        public Mesh(string name, int verticesCount, int facesCount) {
            Name = name;
            Vertices = new Vector3[verticesCount];
            Triangles = new Triangle[facesCount];
        }

        public Vector3[] Vertices { get; private set; }
        public Triangle[] Triangles { get; set; }

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
