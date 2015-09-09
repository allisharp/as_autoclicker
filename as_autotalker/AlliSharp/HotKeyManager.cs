
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace AlliSharp
{
    //[Flags]
    //public enum ModifierKeys : uint
    //{
    //    Alt = 1,
    //    Control = 2,
    //    Shift = 4,
    //    Win = 8
    //}

    public interface IHotKeyOwner
    {
        void HotKeyPressed(int id);
    }

    public class HotKeyManager
    {
        private IHotKeyOwner owner;
        private const int WM_HOTKEY = 0x312;
        private HwndSourceHook hook;
        private HwndSource hwndsource;
        //private IntPtr handle;
        private int nextid = 1;
        //private Window window;
    
        //stack overflow http://stackoverflow.com/questions/2450373/set-global-hotkeys-using-c-sharp
        //private class Window : NativeWindow, IDisposable
        //{
        //    private static int WM_HOTKEY = 0x0312;
        //    private IHotKeyOwner owner;
        //    public Window(IHotKeyOwner owner)
        //    {
        //        this.owner = owner;
        //        // create the handle for the window.
        //        this.CreateHandle(new CreateParams());
        //    }

        //    /// <summary>
        //    /// Overridden to get the notifications.
        //    /// </summary>
        //    /// <param name="m"></param>
        //    protected override void WndProc(ref Message m)
        //    {
        //        base.WndProc(ref m);

        //        // check if we got a hot key pressed.
        //        if (m.Msg == WM_HOTKEY)
        //        {
        //            MessageBox.Show("asdasd");
        //            // get the keys.
        //            Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
        //            ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

        //            owner.HotKeyPressed(0);                   
        //        }
        //    }
         
        //    #region IDisposable Members

        //    public void Dispose()
        //    {
        //        this.DestroyHandle();
        //    }

        //    #endregion
        //}

        public HotKeyManager(IHotKeyOwner owner, IntPtr handle)
        {
            this.owner = owner;
            this.hook = new HwndSourceHook(WndProc);
            this.hwndsource = HwndSource.FromHwnd(handle);
            hwndsource.AddHook(hook);
        }

        public int RegisterFKey(int index)
        {
            
            if (index < 1 || index > 12) return 0;
            Keys key = (new Keys[] { Keys.F1, Keys.F2, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8, Keys.F9, Keys.F10, Keys.F11, Keys.F12 })[index - 1];
            NativeMethods.RegisterHotKey(hwndsource.Handle, nextid, 0x4000, (int)key);
            nextid++;
            return nextid - 1;
        }

        public void UnregisterFKey(int id)
        {
            NativeMethods.UnregisterHotKey(hwndsource.Handle, id);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                owner.HotKeyPressed((int)wParam);
            }

            return new IntPtr(0);
        }
    }
}
