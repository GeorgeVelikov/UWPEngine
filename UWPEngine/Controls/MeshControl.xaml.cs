using System;
using UWPEngine.Shapes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPEngine.Controls {
    public sealed partial class MeshControl : UserControl {
        // TODO: improve on this as using an event for this just doesn't feel right
        public static event EventHandler RemoveMeshEvent;

        public MeshControl() {
            this.InitializeComponent();
        }

        public Mesh ViewModel => (Mesh)DataContext;

        private void RemoveButton_Click(object sender, RoutedEventArgs e) {
            RemoveMeshEvent?.Invoke(ViewModel, new EventArgs());
        }
    }
}
