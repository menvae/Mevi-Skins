using System.Text;

namespace RGSkin.Bindings
{
    public class Converter
    {
        static Converter()
        {
            NativeLibraryLoader.EnsureLoaded();
        }

        public static void ConvertFromOsu(string inputDir, string outputDir)
        {
            unsafe
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(inputDir + "\0");
                byte[] outputBytes = Encoding.UTF8.GetBytes(outputDir + "\0");

                fixed (byte* inputPtr = inputBytes)
                fixed (byte* outputPtr = outputBytes)
                {
                    NativeMethods.fromOsu(inputPtr, outputPtr);
                }
            }
        }
    }
}