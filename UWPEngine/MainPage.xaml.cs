using System;
using System.Threading.Tasks;
using UWPEngine.Shapes;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPEngine {
    public sealed partial class MainPage : Page {
        public MainPage() {
            DataContext = new MainPageViewModel();
            InitializeComponent();

            Window.Current.VisibilityChanged += Visibility_Changed;
        }

        private void Visibility_Changed(object sender, VisibilityChangedEventArgs e) {
            if (e.Visible) {
                ResumeRendering();
                return;
            }

            PauseRendering();
        }

        public MainPageViewModel ViewModel => (MainPageViewModel)DataContext;

        private async void OpenButton_Click(object sender, RoutedEventArgs e) {
            await PauseRenderingAndDoFunction(async () => await ViewModel.OpenFileAndLoadMesh());
        }

        private void AddCubeButton_Click(object sender, RoutedEventArgs e) {
            ViewModel.Scene.AddMesh(new Cube());
        }

        private async Task PauseRenderingAndDoFunction(Func<Task> function) {
            PauseRendering();
            await function();
            ResumeRendering();
        }

        private void PauseRendering() => CompositionTarget.Rendering -= ViewModel.CompositionTarget_Rendering;
        private void ResumeRendering() => CompositionTarget.Rendering += ViewModel.CompositionTarget_Rendering;
    }
}
