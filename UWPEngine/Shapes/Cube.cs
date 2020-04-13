using SharpDX;

namespace UWPEngine.Shapes {
    public class Cube : Mesh {
        public Cube()
            : base(nameof(Cube), 8, 12) {
            Vertices[0] = new Vector3(-1, 1, 1);
            Vertices[1] = new Vector3(1, 1, 1);
            Vertices[2] = new Vector3(-1, -1, 1);
            Vertices[3] = new Vector3(1, -1, 1);
            Vertices[4] = new Vector3(-1, 1, -1);
            Vertices[5] = new Vector3(1, 1, -1);
            Vertices[6] = new Vector3(1, -1, -1);
            Vertices[7] = new Vector3(-1, -1, -1);

            Triangles[0] = new Triangle { VertexA = 0, VertexB = 1, VertexC = 2 };
            Triangles[1] = new Triangle { VertexA = 1, VertexB = 2, VertexC = 3 };
            Triangles[2] = new Triangle { VertexA = 1, VertexB = 3, VertexC = 6 };
            Triangles[3] = new Triangle { VertexA = 1, VertexB = 5, VertexC = 6 };
            Triangles[4] = new Triangle { VertexA = 0, VertexB = 1, VertexC = 4 };
            Triangles[5] = new Triangle { VertexA = 1, VertexB = 4, VertexC = 5 };
            Triangles[6] = new Triangle { VertexA = 2, VertexB = 3, VertexC = 7 };
            Triangles[7] = new Triangle { VertexA = 3, VertexB = 6, VertexC = 7 };
            Triangles[8] = new Triangle { VertexA = 0, VertexB = 2, VertexC = 7 };
            Triangles[9] = new Triangle { VertexA = 0, VertexB = 4, VertexC = 7 };
            Triangles[10] = new Triangle { VertexA = 4, VertexB = 5, VertexC = 6 };
            Triangles[11] = new Triangle { VertexA = 4, VertexB = 6, VertexC = 7 };
        }
    }
}
