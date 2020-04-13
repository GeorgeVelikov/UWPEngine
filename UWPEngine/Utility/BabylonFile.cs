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

        [JsonProperty("positions")]
        public float[] Vertices { get; set; }

        [JsonProperty("indices")]
        public int[] Indices { get; set; }

        public int VerticesCount => (Vertices?.Count() ?? 0) / 3;
        public int IndicesCount => (Indices?.Count() ?? 0) / 3;
    }
}
