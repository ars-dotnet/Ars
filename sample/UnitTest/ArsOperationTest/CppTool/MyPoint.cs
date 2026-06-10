// 在你的 C# 文件顶部
using System.Runtime.InteropServices;

// C# 端的结构体，与 C++ 的 MyPoint 对应
[StructLayout(LayoutKind.Sequential)]
public struct MyPoint
{
    public int X;
    public int Y;
}