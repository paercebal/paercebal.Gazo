using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace paercebal.Gazo.Utils
{
    public class SafeIntPtrHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [SecurityCritical]
        public SafeIntPtrHandle(IntPtr preexistingHandle)
            : base(true)
        {
            SetHandle(preexistingHandle);
        }

        [SecurityCritical]
        public SafeIntPtrHandle(IntPtr preexistingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            SetHandle(preexistingHandle);
        }

        public IntPtr Handle
        {
            get { return this.handle; }
        }

        protected override bool ReleaseHandle()
        {
            if (handle != IntPtr.Zero)
                Marshal.FreeHGlobal(handle);
            return true;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
    }
}
