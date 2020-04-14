using System;
using UWPEngine.Models;
using UWPEngine.Shapes;
using Windows.UI.Xaml.Controls;

namespace UWPEngine.Controls {
    public sealed partial class MeshControlList : UserControl {

        public MeshControlList() {
            this.InitializeComponent();
            MeshControl.RemoveMeshEvent += MeshControl_RemoveMeshEvent;
        }

        private void MeshControl_RemoveMeshEvent(object sender, EventArgs e) {
            if (!(sender is Mesh mesh)) {
                return;
            }

            ViewModel.RemoveMesh(mesh);
        }

        public Scene ViewModel => (Scene)DataContext;
    }
}
