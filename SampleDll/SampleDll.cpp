// SampleDll.cpp : DLL アプリケーション用にエクスポートされる関数を定義します。
//

#include "stdafx.h"
#include <algorithm>
#include <string>
#include <Objbase.h>

extern "C" {
	__declspec(dllexport) int GetInt() { return 123; }
	__declspec(dllexport) double GetDouble() { return 2.34; }
	__declspec(dllexport) int AddInt(int value1, int value2) { return value1 + value2; }
	__declspec(dllexport) double AddDouble(double value1, double value2) { return value1 + value2; }
	// ANSI文字列をC#から受け取り、文字列の長さを返す
	__declspec(dllexport) int StrLenA(const char* c) { return strlen(c); }
	// UNICODE文字列をC#から受け取り、文字列の長さを返す
	__declspec(dllexport) int StrLenW(const wchar_t* c) { return wcslen(c); }
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
	__declspec(dllexport) char* __stdcall GetStr2()
	{
		std::string s = "Hello World";
		auto size = s.size() + sizeof(char);
		char* ret = (char*)::CoTaskMemAlloc(size);
		strcpy_s(ret, size, s.c_str());
		return ret;
	}
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
	// 構造体を受け取る
	struct Data1 {
		int value1;
		int value2;
	};
	__declspec(dllexport) int __stdcall SetData1(const Data1* data)
	{
		return data->value1 + data->value2;
	}
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
