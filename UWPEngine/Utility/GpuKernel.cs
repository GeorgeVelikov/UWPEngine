using Amplifier.OpenCL;

namespace UWPEngine.Utility {
    internal class GpuKernel : OpenCLFunctions {

        [OpenCLKernel]
        void ClearBackBuffer([Global]byte[] BackBuffer, byte r, byte g, byte b, byte a) {
            int i = get_global_id(0);
            BackBuffer[i] = r;
            BackBuffer[i+1] = g;
            BackBuffer[i+2] = b;
            BackBuffer[i+3] = a;
        }

        [OpenCLKernel]
        void ClearDepthBuffer([Global]float[] DepthBuffer) {
            int i = get_global_id(0);
            DepthBuffer[i] = float.MaxValue;
        }
    }
}
