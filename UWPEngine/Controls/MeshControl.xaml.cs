using UWPEngine.Shapes;
using Windows.UI.Xaml.Controls;

namespace UWPEngine.Controls {
    public sealed partial class MeshControl : UserControl {
        public MeshControl() {
            this.InitializeComponent();
        }

        public Mesh ViewModel => (Mesh)DataContext;

    }
}
