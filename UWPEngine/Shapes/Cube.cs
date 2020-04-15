using SharpDX;
using UWPEngine.Structs;

namespace UWPEngine.Shapes {
    public class Cube : Mesh {
        public Cube()
            : base(nameof(Cube), 8, 12) {
            /* normals
                0,  1,  0,
                0,  1,  0
                0,  1,  0
                0,  0,  -1
                0,  0,  -1
                0,  0,  -1
                -1,  0,  0
                -1,  0,  0
                -1,  0,  0
                0,  -1,  0
                0,  -1,  0
                0,  -1,  0
                1,  0,  0
                1,  0,  0
                1,  0,  0
                0,  0,  1
                0,  0,  1
                0,  0,  1
                0,  1,  0
                0,  0,  -1
                0,  0,  -1
                -1,  0,  0
                0,  -1,  0
                0,  -1,  0
                1,  0,  0
                1,  0,  0
                1,  0,  0
                0,  0,  1
                0,  0,  1
                0,  0,  1
            */

            /* uvs
                0.625,  0,  0.375,
                0.25,  0.375,  0,
                0.625,  0.25,  0.375,
                0.5,  0.375,  0.25,
                0.625,  0.5,  0.375,
                0.75,  0.375,  0.5,
                0.625,  0.75,  0.375,
                1,  0.375,  0.75,
                0.375,  0.5,  0.125,
                0.75,  0.125,  0.5,
                0.875,  0.5,  0.625,
                0.75,  0.625,  0.5,
                0.625,  0.25,  0.625,
                0.25,  0.625,  0.5,
                0.625,  0.75,  0.625,
                1,  0.375,  1,
                0.375,  0.5,  0.375,
                0.75,  0.125,  0.75,
                0.875,  0.5,  0.875,
                0.75,  0.625,  0.75
            */

            /* positions
                -1,  1,  1,
                1,  1,  -1,
                1,  1,  1,
                1,  1,  -1,
                -1,  -1,  -1,
                1,  -1,  -1,
                -1,  1,  -1,
                -1,  -1,  1,
                -1,  -1,  -1,
                1,  -1,  1,
                -1,  -1,  -1,
                -1,  -1,  1,
                1,  1,  1,
                1,  -1,  -1,
                1,  -1,  1,
                -1,  1,  1,
                1,  -1,  1,
                -1,  -1,  1,
                -1,  1,  -1,
                1,  1,  -1,
                -1,  1,  -1,
                -1,  1,  1,
                1,  -1,  -1,
                -1,  -1,  -1,
                1,  1,  1,
                1,  1,  -1,
                1,  -1,  -1,
                -1,  1,  1,
                1,  1,  1,
                1,  -1,  1
             */

            Vertices[0] = new Vector3(-1, 1, 1);
            Vertices[1] = new Vector3(1, 1, 1);
            Vertices[2] = new Vector3(-1, -1, 1);
            Vertices[3] = new Vector3(1, -1, 1);
            Vertices[4] = new Vector3(-1, 1, -1);
            Vertices[5] = new Vector3(1, 1, -1);
            Vertices[6] = new Vector3(1, -1, -1);
            Vertices[7] = new Vector3(-1, -1, -1);

            Faces[0] = new Face { VertexA = 0, VertexB = 1, VertexC = 2 };
            Faces[1] = new Face { VertexA = 1, VertexB = 2, VertexC = 3 };
            Faces[2] = new Face { VertexA = 1, VertexB = 3, VertexC = 6 };
            Faces[3] = new Face { VertexA = 1, VertexB = 5, VertexC = 6 };
            Faces[4] = new Face { VertexA = 0, VertexB = 1, VertexC = 4 };
            Faces[5] = new Face { VertexA = 1, VertexB = 4, VertexC = 5 };
            Faces[6] = new Face { VertexA = 2, VertexB = 3, VertexC = 7 };
            Faces[7] = new Face { VertexA = 3, VertexB = 6, VertexC = 7 };
            Faces[8] = new Face { VertexA = 0, VertexB = 2, VertexC = 7 };
            Faces[9] = new Face { VertexA = 0, VertexB = 4, VertexC = 7 };
            Faces[10] = new Face { VertexA = 4, VertexB = 5, VertexC = 6 };
            Faces[11] = new Face { VertexA = 4, VertexB = 6, VertexC = 7 };
        }
    }
}
