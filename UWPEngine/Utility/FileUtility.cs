using Newtonsoft.Json;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWPEngine.Shapes;
using UWPEngine.Structs;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace UWPEngine.Utility {
    public static class FileUtility {
        public static async Task<StorageFile> BrowseForBabylonFile() {

            FileOpenPicker filePicker = new FileOpenPicker {
                SuggestedStartLocation = PickerLocationId.Desktop,
            };

            filePicker.FileTypeFilter.Add(".babylon");

            StorageFile file = await filePicker.PickSingleFileAsync();

            return file;
        }

        public static async Task<IList<Mesh>> ConvertBabylonFileToMesh(StorageFile file) {
            IList<Mesh> meshes = new List<Mesh>();

            string data = await FileIO.ReadTextAsync(file);

            BabylonFile babylonFile = JsonConvert.DeserializeObject<BabylonFile>(data);

            foreach (BabylonMesh babylonMesh in babylonFile.Meshes) {
                Mesh mesh = new Mesh(babylonMesh.Name, babylonMesh.VerticesCount, babylonMesh.IndicesCount);

                // Filling the Vertices array of our mesh first
                for (int i = 0; i < babylonMesh.VerticesCount; i++) {
                    mesh.Vertices[i] = new Vector3(
                        x: babylonMesh.Vertices[i * 3],
                        y: babylonMesh.Vertices[i * 3 + 1],
                        z: babylonMesh.Vertices[i * 3 + 2]
                    );
                }

                // Then filling the Faces array
                for (int i = 0; i < babylonMesh.IndicesCount; i++) {
                    mesh.Faces[i] = new Face {
                        VertexA = babylonMesh.Indices[i * 3],
                        VertexB = babylonMesh.Indices[i * 3 + 1],
                        VertexC = babylonMesh.Indices[i * 3 + 2],
                    };
                }

                // Getting the position you've set in Blender
                var position = babylonMesh.Position;
                mesh.Position = new Vector3(position[0], position[1], position[2]);

                meshes.Add(mesh);
            }

            return meshes;
        }
    }
}
