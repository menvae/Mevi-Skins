using System;
using System.Runtime.InteropServices;
using System.Text;

namespace RGSkin.Bindings;

public class Converter
{
    static Converter()
    {
        NativeLibraryLoader.EnsureLoaded();
    }

    private static unsafe void ThrowIfError(byte* errorPtr)
    {
        if (errorPtr != null)
        {
            string error = Marshal.PtrToStringUTF8((IntPtr)errorPtr) ?? "Unknown error";
            NativeMethods.free_error(errorPtr);
            throw new Exception(error);
        }
    }

    public static unsafe void ConvertFromOsu(string inputDir, string skinOutputDir, bool exportLayout, string layoutOutputDir)
    {
        byte* errorPtr = null;
        
        fixed (byte* inputPtr = Encoding.UTF8.GetBytes(inputDir + "\0"))
        fixed (byte* skinPtr = Encoding.UTF8.GetBytes(skinOutputDir + "\0"))
        fixed (byte* layoutPtr = Encoding.UTF8.GetBytes(layoutOutputDir + "\0"))
        {
            bool success = NativeMethods.fromOsu(
                inputPtr,
                skinPtr,
                exportLayout, 
                layoutPtr,
                &errorPtr
            );
            
            if (!success)
            {
                ThrowIfError(errorPtr);
                throw new Exception("Failed to convert from osu! skin: Unknown error");
            }
        }
    }
}