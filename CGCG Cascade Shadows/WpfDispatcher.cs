using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace FR.CascadeShadows;

public static class WpfDispatcher
{
    [DllImport("user32.dll")]
    static extern bool TranslateMessage([In] ref MSG lpMsg);

    [DllImport("user32.dll")]
    static extern IntPtr DispatchMessage([In] ref MSG lpmsg);

    [DllImport("user32.dll")]
    static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

    const uint PM_NOREMOVE = 0x0000;
    const uint PM_REMOVE = 0x0001;
    const uint PM_NOYIELD = 0x0002;

    public static void ProcessMessages()
    {
        while (PeekMessage(out MSG msg, IntPtr.Zero, 0, 0, PM_REMOVE))
        {
            if (ComponentDispatcher.RaiseThreadMessage(ref msg) == false)
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
        }
    }
}