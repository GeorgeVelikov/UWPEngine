using Newtonsoft.Json;
using System.Linq;

namespace UWPEngine.Utility {
    public class BabylonFile {
        [JsonProperty("meshes")]
        public BabylonMesh[] Meshes { get; set; }
    }

    public class BabylonMesh {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("position")]
        public float[] Position { get; set; }

        [JsonProperty("rotation")]
        public float[] Rotation { get; set; }

        [JsonProperty("positions")]
        public float[] Positions { get; set; }

        [JsonProperty("normals")]
        public float[] Normals { get; set; }

        [JsonProperty("uvs")]
        public float[] UVs { get; set; }

        [JsonProperty("indices")]
        public int[] Faces { get; set; }

        // same as normals count
        public int VerticesCount => (Positions?.Count() ?? 0) / 3;
        public int UVsCount => (UVs?.Count() ?? 0) / 3;
        public int FacesCount => (Faces?.Count() ?? 0) / 3;
    }
}
