﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using WinQuickLook.Interop;

namespace WinQuickLook
{
    public class KeyboardHook : IDisposable
    {
        public KeyboardHook(Action performAction, Action cancelAction)
        {
            _performAction = performAction;
            _cancelAction = cancelAction;
        }

        private IntPtr _hook;

        private readonly Action _performAction;
        private readonly Action _cancelAction;

        private NativeMethods.LowLevelKeyboardProc _keyboardHookProc;
        
        public void Start()
        {
            if (_hook != IntPtr.Zero)
            {
                return;
            }

            using (var process = Process.GetCurrentProcess())
            using (var module = process.MainModule)
            {
                _keyboardHookProc = KeyboardHookProc;

                _hook = NativeMethods.SetWindowsHookEx(Consts.WH_KEYBOARD_LL, _keyboardHookProc, NativeMethods.GetModuleHandle(module.ModuleName), 0);
            }
        }

        public void Dispose()
        {
            if (_hook != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(_hook);

                _hook = IntPtr.Zero;
            }
        }

        private IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == Consts.HC_ACTION && wParam == (IntPtr)Consts.WM_KEYDOWN)
            {
                var kbdllhook = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

                switch ((Keys)kbdllhook.vkCode)
                {
                    case Keys.Space:
                        if (IsNoModifierKey())
                        {
                            _performAction();
                        }
                        break;

                    case Keys.Escape:
                        if (IsNoModifierKey())
                        {
                            _cancelAction();
                        }
                        break;
                }
            }

            return NativeMethods.CallNextHookEx(_hook, nCode, wParam, lParam);
        }

        private static bool IsNoModifierKey()
        {
            if (NativeMethods.GetAsyncKeyState((int)Keys.ControlKey) != 0)
            {
                return false;
            }

            if (NativeMethods.GetAsyncKeyState((int)Keys.Menu) != 0)
            {
                return false;
            }

            if (NativeMethods.GetAsyncKeyState((int)Keys.ShiftKey) != 0)
            {
                return false;
            }

            if (NativeMethods.GetAsyncKeyState((int)Keys.LWin) != 0 || NativeMethods.GetAsyncKeyState((int)Keys.RWin) != 0)
            {
                return false;
            }

            return true;
        }
    }
}
