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
                Mesh mesh = new Mesh(babylonMesh.Name, babylonMesh.VerticesCount, babylonMesh.FacesCount);

                // Filling the Vertices array of our mesh first
                for (int i = 0; i < babylonMesh.VerticesCount; i++) {
                    int xIndex = i * 3;
                    int yIndex = i * 3 + 1;
                    int zIndex = i * 3 + 2;

                    mesh.Vertices[i].Coordinates = new Vector3 {
                        X = babylonMesh.Positions[xIndex],
                        Y = babylonMesh.Positions[yIndex],
                        Z = babylonMesh.Positions[zIndex],
                    };

                    mesh.Vertices[i].Normal = new Vector3 {
                        X = babylonMesh.Normals[xIndex],
                        Y = babylonMesh.Normals[yIndex],
                        Z = babylonMesh.Normals[zIndex],
                    };
                }

                for (int i = 0; i < babylonMesh.FacesCount; i++) {
                    // filling the Faces array
                    mesh.Faces[i] = new Face {
                        VertexA = babylonMesh.Faces[i * 3],
                        VertexB = babylonMesh.Faces[i * 3 + 1],
                        VertexC = babylonMesh.Faces[i * 3 + 2],
                    };
                }

                // Getting the position you've set in Blender
                float[] position = babylonMesh.Position;
                float[] rotation = babylonMesh.Rotation;
                float[] scale = babylonMesh.Scale;

                mesh.Position = new Vector3(position[0], position[1], position[2]);
                mesh.Rotation = new Vector3(rotation[0], rotation[1], rotation[2]);
                mesh.Scale = new Vector3(scale[0], scale[1], scale[2]);

                meshes.Add(mesh);
            }

            return meshes;
        }
    }
}
