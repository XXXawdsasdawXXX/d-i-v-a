using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowService : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWng, int nIndex, uint dwNewLong);

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    [DllImport("Dwmapi.dll",SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWind, IntPtr hWindInsertAfter, int x, int y, int cx, int cy, uint uFlag);


    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    
    private const int GWL_EX_STYLE = -20;

    private const uint WS_EX_LAYERED = 0x00080000;
    private const uint WS_EX_TRANSPARENT = 0x00000020;

    void Start()
    {
#if !UNITY_EDITOR_
        // MessageBox(new IntPtr(0), "Good to see you", "Lulu", 0);
        var activeWindow = GetActiveWindow();
        
        MARGINS margins = new MARGINS()
        {
            cxLeftWidth = -1,
        };
        
        DwmExtendFrameIntoClientArea(activeWindow, ref margins);
        SetWindowLong(activeWindow, GWL_EX_STYLE,WS_EX_LAYERED | WS_EX_TRANSPARENT);
        SetWindowPos(activeWindow, HWND_TOPMOST, 0, 0, 0, 0,0);
#endif
        Application.runInBackground = true;
    }

    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cxTopHeight;
        public int cxBottomHeight;
    }
}