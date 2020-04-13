using System;
using System.Threading.Tasks;
using UWPEngine.Shapes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPEngine {
    public sealed partial class MainPage : Page {
        public MainPage() {
            DataContext = new MainPageViewModel();
            InitializeComponent();

            Loaded += Page_Loaded;
        }

        public MainPageViewModel ViewModel => (MainPageViewModel)DataContext;

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            // Registering to the XAML rendering loop.
            ResumeRendering();
        }

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
