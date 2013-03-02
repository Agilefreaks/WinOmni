using System;
using System.Runtime.InteropServices;

namespace ClipboardWrapper.Imports
{
	public delegate int WindowProcDelegate(IntPtr hw, IntPtr uMsg, IntPtr wParam, IntPtr lParam);

	/// <summary>
	/// Windows User32 DLL declarations
	/// </summary>
	public class User32
	{
		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr SetClipboardViewer(IntPtr hWnd);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern bool ChangeClipboardChain(
			IntPtr hWndRemove,  // handle to window to remove
			IntPtr hWndNewNext  // handle to next window
			);

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

	}
}
