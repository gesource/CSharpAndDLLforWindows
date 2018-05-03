using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Data1
    {
        public int value1;
        public int value2;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    public struct Data2
    {
        public int value1;
        [MarshalAs(UnmanagedType.LPStr)]
        public string value2;
    }

    /// <summary>
    /// 配列を含む構造体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Data3
    {
        public int value;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] values;
    }

    internal class NativeMethods
    {
        [DllImport("SampleDll.dll")]
        internal static extern int GetInt();

        [DllImport("SampleDll.dll")]
        internal static extern double GetDouble();

        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int AddInt(int value1, int value2);

        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double AddDouble(double value1, double value2);

        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int StrLenA(string s);

        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal static extern int StrLenW(string s);

        /// <summary>
        /// ANSI文字列をC++から受け取る
        /// </summary>
        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern bool GetStrA(StringBuilder s, int bufsize);

        /// <summary>
        /// UNICODE文字列をC++から受け取る
        /// </summary>
        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        internal static extern bool GetStrW(StringBuilder s, int bufsize);

        /// <summary>
        /// 文字列をC++から受け取る
        /// </summary>
        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal static extern string GetStr2();

        /// <summary>
        /// 文字列のバイト配列を渡して、大文字に変換された結果のバイト配列を受け取る
        /// </summary>
        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern void UpperCase(byte[] src, int srcLength, ref IntPtr dest, ref int destLength);

        /// <summary>
        /// UpperCase()で確保したメモリを解放する
        /// </summary>
        [DllImport("SampleDll.dll")]
        internal static extern void FreeMemory(ref IntPtr ptr);

        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern int SetData1(ref Data1 data1);

        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int SetData2(ref Data2 data2);

        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern int SetData3(ref Data3 data3);
    }

    public class SampleDll
    {
        public static int CallGetInt()
        {
            return NativeMethods.GetInt();
        }
        public static double CallGetDouble()
        {
            return NativeMethods.GetDouble();
        }
        public static int CallAddInt(int value1, int value2)
        {
            return NativeMethods.AddInt(value1, value2);
        }
        public static double CallAddDouble(double value1, double value2)
        {
            return NativeMethods.AddDouble(value1, value2);
        }
        public static int CallStrLenA(string s)
        {
            return NativeMethods.StrLenA(s);
        }
        public static int CallStrLenW(string s)
        {
            return NativeMethods.StrLenW(s);
        }
        public static string CallGetStrA()
        {
            StringBuilder sb = new StringBuilder(256);
            if (NativeMethods.GetStrA(sb, sb.Capacity))
                return sb.ToString();
            else
                return "失敗しました。";
        }

        public static string CallGetStrW()
        {
            StringBuilder sb = new StringBuilder(256);
            if (NativeMethods.GetStrW(sb, sb.Capacity))
                return sb.ToString();
            else
                return "失敗しました。";
        }
        public static string CallGetStr2()
        {
            return NativeMethods.GetStr2();
        }
        /// <summary>
        /// 文字列を渡して、大文字に変換された結果を受け取る
        /// </summary>
        public static string UpperCase(string str)
        {
            byte[] ascii = Encoding.ASCII.GetBytes(str);
            IntPtr dest = IntPtr.Zero;
            int destLength = 0;
            NativeMethods.UpperCase(ascii, ascii.Length, ref dest, ref destLength);
            byte[] result = new byte[destLength];
            Marshal.Copy(dest, result, 0, destLength);
            NativeMethods.FreeMemory(ref dest);

            string up = Encoding.ASCII.GetString(result);
            return up;
        }
        public static int SetData1(Data1 data1)
        {
            return NativeMethods.SetData1(ref data1);
        }
        public static int SetData2(ref Data2 data2)
        {
            return NativeMethods.SetData2(ref data2);
        }
        public static int SetData3(ref Data3 data3)
        {
            return NativeMethods.SetData3(ref data3);
        }
    }
}
