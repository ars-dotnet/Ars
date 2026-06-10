using System;
using System.Runtime.InteropServices;

internal static class NativeMethods
{
    private const string DllName = "Configs/CppFunctions.dll";

    // ✅ add
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int add(int a, int b);

    // ✅ process_string (Unicode)
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern int process_string(string input);

    // ✅ move_point
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void move_point(ref MyPoint point, int dx, int dy);
}
