namespace WindowsImports
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// API Declarations for the Windows Shell32 library
    /// </summary>
    public class Shell32
    {
        // ReSharper disable InconsistentNaming
        public const int MAX_PATH = 260;
        // ReSharper restore InconsistentNaming

        [StructLayout(LayoutKind.Sequential)]
        // ReSharper disable InconsistentNaming
        protected struct structSHFILEINFO
        // ReSharper restore InconsistentNaming
        {
            /// <summary>
            /// Handle to the icon that represents the file. 
            /// You are responsible for destroying this 
            /// handle with DestroyIcon when you no longer need it. 
            /// </summary>
            public IntPtr hIcon;
            /// <summary>
            /// Index of the icon image within the system image list. 
            /// </summary>
            public Int16 iIcon;
            /// <summary>
            /// Array of values that indicates the attributes 
            /// of the file object. For information about these 
            /// values, see the IShellFolder::GetAttributesOf method. 
            /// </summary>
            public int dwAttributes;
            /// <summary>
            /// String that contains the name of the file 
            /// as it appears in the Microsoft® Windows® Shell, 
            /// or the path and file name of the file that 
            /// contains the icon representing the file.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szDisplayName;
            /// <summary>
            /// String that describes the type of file.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
    }
}
