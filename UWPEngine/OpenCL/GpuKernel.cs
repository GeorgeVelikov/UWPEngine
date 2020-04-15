using Amplifier.OpenCL;

namespace UWPEngine.OpenCL {
    internal class GpuKernel : OpenCLFunctions {

        [OpenCLKernel]
        public void ClearBackBuffer([Global]byte[] BackBuffer, byte r, byte g, byte b, byte a) {
            int i = get_global_id(0);

            // TODO: can I improve this?
            if (i % 4 != 0) {
                return;
            }

            BackBuffer[i] = r;
            BackBuffer[i + 1] = g;
            BackBuffer[i + 2] = b;
            BackBuffer[i + 3] = a;
        }

        [OpenCLKernel]
        public void ClearDepthBuffer([Global]float[] DepthBuffer) {
            int i = get_global_id(0);
            DepthBuffer[i] = float.MaxValue;
        }

    }
}
