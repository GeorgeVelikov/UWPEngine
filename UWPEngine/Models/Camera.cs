using SharpDX;

namespace UWPEngine.Models {
    public class Camera : NotifyPropertyChangedBase {
        private Vector3 position;
        private Vector3 target;

        public Camera() {
            Position = new Vector3(0, 0, 10.0f);
            Target = Vector3.Zero;
        }

        public Vector3 Position {
            get => position;
            set {
                if (position != value) {
                    position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        public Vector3 Target {
            get => target;
            set {
                if (target != value) {
                    target = value;
                    OnPropertyChanged(nameof(Target));
                }
            }
        }
    }
}
