using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RGSkin.Bindings
{
    internal static class NativeLibraryLoader
    {
        private static bool _isLoaded = false;
        private static readonly object _lock = new object();

        public static void EnsureLoaded()
        {
            if (_isLoaded) return;

            lock (_lock)
            {
                if (_isLoaded) return;

                string libraryName;
                string resourceName;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    libraryName = "rgskin.dll";
                    resourceName = "rgskin.dll";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    libraryName = "librgskin.so";
                    resourceName = "librgskin.so";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    libraryName = "librgskin.dylib";
                    resourceName = "librgskin.dylib";
                }
                else
                {
                    throw new PlatformNotSupportedException("Unsupported platform");
                }

                string tempDir = Path.Combine(Path.GetTempPath(), "rgskin_native", Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                string libraryPath = Path.Combine(tempDir, libraryName);

                var assembly = Assembly.GetExecutingAssembly();
                using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream == null)
                    {
                        throw new FileNotFoundException($"Embedded resource '{resourceName}' not found");
                    }

                    using (var fileStream = File.Create(libraryPath))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }

                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    try
                    {
                        var process = System.Diagnostics.Process.Start("chmod", $"+x \"{libraryPath}\"");
                        process?.WaitForExit();
                    }
                    catch
                    {
                    }
                }

                NativeLibrary.SetDllImportResolver(assembly, (libraryName, assembly, searchPath) =>
                {
                    if (libraryName == "rgskin")
                    {
                        return NativeLibrary.Load(libraryPath);
                    }
                    return IntPtr.Zero;
                });

                _isLoaded = true;
            }
        }
    }
}