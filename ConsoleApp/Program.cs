using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.WriteLine(SampleDll.CallGetInt());
            Debug.WriteLine(SampleDll.CallGetDouble());
            Debug.WriteLine(SampleDll.CallAddInt(1, 2));
            Debug.WriteLine(SampleDll.CallAddDouble(1.2, 3.4));
            Debug.WriteLine(SampleDll.CallStrLenA("あいうえおABC"));
            Debug.WriteLine(SampleDll.CallStrLenW("あいうえおABC"));
            Debug.WriteLine(SampleDll.CallGetStrA());
            Debug.WriteLine(SampleDll.CallGetStrW());
            Debug.WriteLine(SampleDll.CallGetStr2());
            Debug.WriteLine(SampleDll.UpperCase("hello"));
            var data1 = new Data1 { value1 = 1, value2 = 2, };
            Debug.WriteLine(SampleDll.SetData1(data1));
            var data2 = new Data2 { value1 = 2, value2 = "hello", };
            Debug.WriteLine(SampleDll.SetData2(ref data2));
            var data3 = new Data3 { value = 3, values = new int[] { 4, 5, 6 }, };
            Debug.WriteLine(SampleDll.SetData3(ref data3));
        }
    }
}
