﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WinQuickLook.Interop
{
    public static class NativeMethods
    {
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        public static extern uint GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int hookType, LowLevelKeyboardProc hookDelegate, IntPtr module, uint threadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hook);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("shell32.dll")]
        public static extern void SHCreateItemFromParsingName([In, MarshalAs(UnmanagedType.LPWStr)] string pszPath, [In] IntPtr pbc, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [Out, MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] out IShellItem ppv);
        
        [DllImport("shlwapi.dll")]
        public static extern void StrRetToBuf([In] ref STRRET pstr, [In] IntPtr pidl, StringBuilder pszBuf, uint cchBuf);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern uint GetGUIThreadInfo(uint dwthreadid, ref GUITHREADINFO lpguithreadinfo);
    }
}