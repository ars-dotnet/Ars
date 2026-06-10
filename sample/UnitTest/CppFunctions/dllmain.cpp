// dllmain.cpp
#include "pch.h"
#include "framework.h"
#include <iostream>
#include <cstring> // For strcpy_s

// 定义一个简单的结构体
struct MyPoint {
    int x;
    int y;
};

// 使用 extern "C" 确保函数名不会被 C++ 编译器修饰
extern "C" {
    // 导出一个简单的加法函数
    CPPFUNCTIONS_API int add(int a, int b) {
        return a + b;
    }

    // 导出一个接收字符串并返回其长度的函数
    // LPWStr (Long Pointer to Wide String) 对应 C# 的 string (Unicode)
    CPPFUNCTIONS_API int process_string(const wchar_t* input) {
        if (input) {
            std::wcout << L"C++ received: " << input << std::endl;
            return wcslen(input);
        }
        return 0;
    }

    // 导出一个通过指针修改结构体的函数
    CPPFUNCTIONS_API void move_point(MyPoint* point, int dx, int dy) {
        if (point) {
            point->x += dx;
            point->y += dy;
        }
    }
}