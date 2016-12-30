namespace MiniDumpInCSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    using static NativeMethods;

    internal class Program
    {
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            throw new Exception();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var dumpFileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".dmp";
            using (var file = new FileStream(dumpFileName, FileMode.Create))
            {
                var info = new MinidumpExceptionInformation
                {
                    ThreadId = GetCurrentThreadId(),
                    ExceptionPointers = Marshal.GetExceptionPointers(),
                    ClientPointers = false
                };

                MiniDumpWriteDump(
                    GetCurrentProcess(),
                    GetCurrentProcessId(),
                    file.SafeFileHandle,
                    MinidumpType.WithFullMemory,
                    ref info,
                    IntPtr.Zero,
                    IntPtr.Zero);
            }

            Process.GetCurrentProcess().Kill();
        }
    }
}