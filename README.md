# C++で作成したDLLをC#で使うサンプルプロジェクト

## 概要

C++で作成したDLLをC#で使うサンプルプロジェクトです。

## 開発環境

* Visual Studio 2017

## 使用言語

* DLL
  * C++

* Console App
  * C#

## サンプルコード

### DLLから返値の数値を受け取る

C/C++から数値を受け取ります。

SampleDll.cpp

    extern "C" {
        __declspec(dllexport) int GetInt() { return 123; }
        __declspec(dllexport) double GetDouble() { return 2.34; }
    }

SampleDll.cs

    internal class NativeMethods
    {
        [DllImport("SampleDll.dll")]
        internal static extern int GetInt();

        [DllImport("SampleDll.dll")]
        internal static extern double GetDouble();
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
    }

### DLLに数値を渡して、返値の数値を受け取る

C#からC/C++に数値を渡して、C/C++からの戻り値をC#で受け取ります。

SampleDll.cpp

    extern "C" {
        __declspec(dllexport) int AddInt(int value1, int value2) { return value1 + value2; }
        __declspec(dllexport) double AddDouble(double value1, double value2) { return value1 + value2; }
    }

SampleDll.cs

    internal class NativeMethods
    {
        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int AddInt(int value1, int value2);

        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double AddDouble(double value1, double value2);
    }

    public class SampleDll
    {
        public static int CallAddInt(int value1, int value2)
        {
            return NativeMethods.AddInt(value1, value2);
        }
        public static double CallAddDouble(double value1, double value2)
        {
            return NativeMethods.AddDouble(value1, value2);
        }
    }

### 引数に文字列を渡す

C#の文字列をC/C++に渡します。  
C/C++が受け取る文字列がANSI文字列とUNICODE文字列のどちらであるか、C#のDllImport()で設定します。

SampleDll.cpp

    extern "C" {
        // ANSI文字列をC#から受け取り、文字列の長さを返す
        __declspec(dllexport) int StrLenA(const char* c) { return strlen(c); }
        // UNICODE文字列をC#から受け取り、文字列の長さを返す
        __declspec(dllexport) int StrLenW(const wchar_t* c) { return wcslen(c); }
    }

SampleDll.cs

    internal class NativeMethods
    {
        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int StrLenA(string s);

        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal static extern int StrLenW(string s);
    }

    public class SampleDll
    {
        public static int CallStrLenA(string s)
        {
            return NativeMethods.StrLenA(s);
        }
        public static int CallStrLenW(string s)
        {
            return NativeMethods.StrLenW(s);
        }
    }

### C#に文字列を渡す

C#からC/C++に文字列のバッファを渡します。  
C/C++は渡されたバッファに文字列を設定します。  
C#はバッファから文字列を復元します。

SampleDll.cpp

    extern "C" {
        // ANSI文字列をC#に渡す
        __declspec(dllexport) bool __stdcall GetStrA(char* buf, size_t bufsize)
        {
            if (bufsize < 5)
                return false;

            // Shift_JISの'あ'
            buf[0] = '\x82';
            buf[1] = '\xa0';
            // Shift_JISの'い'
            buf[2] = '\x82';
            buf[3] = '\xa1';
            // 終端
            buf[4] = '\0';
            return true;
        }
        // UNICODE文字列をC#に渡す
        __declspec(dllexport) bool __stdcall GetStrW(wchar_t* buf, size_t bufsize)
        {
            if (bufsize < 3)
                return false;

            buf[0] = u'あ';
            buf[1] = u'い';
            // 終端
            buf[2] = '\0';
            return true;
        }
    }

SampleDll.cs

    internal class NativeMethods
    {
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
    }

    public class SampleDll
    {
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
    }

Windows限定の方法

SampleDll.cpp

    #include <Objbase.h>

    extern "C" {
        __declspec(dllexport) char* __stdcall GetStr2()
        {
            std::string s = "Hello World";
            auto size = s.size() + sizeof(char);
            char* ret = (char*)::CoTaskMemAlloc(size);
            strcpy_s(ret, size, s.c_str());
            return ret;
        }
    }

SampleDll.cs

    public class SampleDll
    {
        /// <summary>
        /// 文字列をC++から受け取る
        /// </summary>
        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal static extern string GetStr2();
    }

    public class SampleDll
    {
        public static string CallGetStr2()
        {
            return NativeMethods.GetStr2();
        }
    }

### C#からC/C++にバイト配列を渡す。C/C++からC#にバイト配列を返す

C#からC/C++に文字列のバイト配列を渡します。  
C/C++は受け取った文字列を大文字に変換し、変換結果のバイト配列と文字列長を返します。

SampleDll.cpp

    extern "C" {
        // ASCIIの文字列を受け取り大文字にして返す
        __declspec(dllexport) void __stdcall UpperCase(const char* src, const int srcLength, char** dest, int* destlength)
        {
            std::string s(src, srcLength);
            std::string up;
            std::transform(s.begin(), s.end(), std::back_inserter(up), toupper);

            *destlength = up.size();
            *dest = (char*)malloc(*destlength);
            memcpy(*dest, up.c_str(), *destlength);
        }

        // UpperCase()で確保したメモリを解放する
        __declspec(dllexport) void __stdcall FreeMemory(char** ptr)
        {
            free(*ptr);
        }
    }

SampleDll.cs

    public class SampleDll
    {
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
    }

    public class SampleDll
    {
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
    }

### C/C++に構造体を渡す

C#からC/C++に構造体を渡します。  
C/C++は渡された構造体の値を取得します。

SampleDll.cpp

    extern "C" {
        // 構造体を受け取る
        struct Data1 {
            int value1;
            int value2;
        };
        __declspec(dllexport) int __stdcall SetData1(const Data1* data)
        {
            return data->value1 + data->value2;
        }
    }

SampleDll.cs

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Data1
    {
        public int value1;
        public int value2;
    }

    public class SampleDll
    {
        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern int SetData1(ref Data1 data1);
    }

    public class SampleDll
    {
        public static int SetData1(Data1 data1)
        {
            return NativeMethods.SetData1(ref data1);
        }
    }

C#から文字列を含む構造体を受け取ります。

SampleDll.cpp

    extern "C" {
        // 文字列を含む構造体を受け取る
        struct Data2
        {
            int value1;
            char* value2;
        };
        __declspec(dllexport) int __stdcall SetData2(const Data2* data)
        {
            return data->value1 + strlen(data->value2);
        }
    }

SampleDll.cs

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    public struct Data2
    {
        public int value1;
        [MarshalAs(UnmanagedType.LPStr)]
        public string value2;
    }

    public class SampleDll
    {
        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int SetData2(ref Data2 data2);
    }

    public class SampleDll
    {
        public static int SetData2(ref Data2 data2)
        {
            return NativeMethods.SetData2(ref data2);
        }
    }

C#からC/C++に配列を含む構造体を渡します。

SampleDll.cpp

    extern "C" {
        // 配列を含む構造体
        struct Data3 {
            int value;
            int values[3];
        };
        __declspec(dllexport) int __stdcall SetData3(Data3* data)
        {
            int ret = data->value;
            for (int i = 0; i < 3; ++i)
                ret += data->values[i];
            return ret;
        }
    }

SampleDll.cs

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
        [DllImport("SampleDll.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern int SetData3(ref Data3 data3);
    }

    public class SampleDll
    {
        public static int SetData3(ref Data3 data3)
        {
            return NativeMethods.SetData3(ref data3);
        }
    }

