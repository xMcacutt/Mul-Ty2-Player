using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection.Metadata;
using System.CodeDom;
using System.Collections;
using System.Drawing;

namespace MT2PClient
{
    internal class ProcessHandler
    {
        public static IntPtr HProcess;
        public static Process Ty2Process;
        private static nint Ty2ProcessBaseAddress;
        private static bool HasProcess;

        public static bool MemoryWriteDebugLogging = false;
        public static bool MemoryReadDebugLogging = false;
        public static int i = 0;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        internal static unsafe extern bool ReadProcessMemory(
            nint hProcess,
            void* lpBaseAddress,
            void* lpBuffer,
            nuint nSize,
            nuint* lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);

        //Attempts to find the Ty process, returns true if successfully found
        //Automatically opens handle if found
        public static bool FindTyProcess(bool overrideOld = true)
        {
            if (!overrideOld && HasProcess)
                return true;

            Process[] processes = Process.GetProcessesByName("Ty2");

            if (processes.Length == 0)
            {
                //No process found
                return false;
            }
            else if (processes.Length > 1)
            {
                //Multiple found
                Console.WriteLine($"WARNING: Multiple ({processes.Length}) instances of Mul-Ty-Player are open, can and will cause fuckery.");
            }
            Ty2Process = processes.First();
            Ty2Process.Refresh();
            Ty2Process.EnableRaisingEvents = true;

            Ty2Process.Exited += (o, e) =>
            {
                Ty2Process.Close();
                Ty2Process.Dispose();
                HasProcess = false;
            };

            HProcess = OpenProcess(0x1F0FFF, false, Ty2Process.Id);
            Ty2ProcessBaseAddress = Ty2Process.MainModule.BaseAddress;
            HasProcess = true;
            return true;
        }

        public static void WriteData(int address, byte[] bytes, string writeIndicator)
        {
            try
            {
                bool success = WriteProcessMemory(HProcess, address, bytes, bytes.Length, out nint bytesWritten);
                if (MemoryWriteDebugLogging)
                {
                    string message = BitConverter.ToString(bytes) + " to 0x" + address.ToString("X") + " For: " + writeIndicator;
                    string logMsg = (success ? "Successfully wrote " : "Failed to write") + message;
                    Console.WriteLine(logMsg);
                }
                if (!success && !HasProcess)
                {
                    throw new TyClosedException();
                }
            }
            catch (TyClosedException ex)
            {
                throw ex;
            }
        }

        public static unsafe bool TryRead<T>(nint address, out T result, bool addBase)
        where T : unmanaged
        {
            try
            {
                if (!HasProcess)
                {
                        throw new TyClosedException();
                }
                fixed (T* pResult = &result)
                {
                    //string s = GetCallStackAsString();
                    if (addBase) address = Ty2ProcessBaseAddress + address;
                    nuint nSize = (nuint)sizeof(T), nRead;
                    //BasicIoC.LoggerInstance.Write(address.ToString() + " " + s);
                    return ReadProcessMemory(HProcess, (void*)address, pResult, nSize, &nRead)
                        && nRead == nSize;
                }
            }
            catch (TyClosedException ex)
            {
                throw ex;
            }
            catch (Win32Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result = default;
                return false;
            }
        }

        public static unsafe bool TryRead(nint address, int size, out byte[]? result, bool addBase)
        { 
            try
            {
                if (!HasProcess)
                {
                    throw new TyClosedException();
                }
                result = new byte[size];

                fixed (byte* pResult = result)
                {
                    if (addBase) address = Ty2ProcessBaseAddress + address;
                    nuint nRead;
                    if (ReadProcessMemory(HProcess, (void*)address, pResult, (nuint)size, &nRead) && nRead == (nuint)size)
                    {
                        return true;
                    }
                    else return false;
                }
            }
            catch (TyClosedException ex)
            {
                throw ex;
            }
            catch (Win32Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result = default;
                return false;
            }
        }
    }
}