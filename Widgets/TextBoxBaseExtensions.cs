using System.Runtime.InteropServices;

namespace FiniteGroupCalc.Widgets
{
    // TextBox, RichTextBox...etc.
    public static class TextBoxBaseExtensions
    {
        private const int EM_SETRECT = 0xB3;
        private const int EM_SETMARGINS = 0xD3;

        private const int EC_LEFTMARGIN = 0x1;
        private const int EC_RIGHTMARGIN = 0x2;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(Rectangle r)
            {
                Left = r.Left;
                Top = r.Top;
                Right = r.Right;
                Bottom = r.Bottom;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(nint hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(nint hWnd, int msg, int wParam, ref RECT rect);

        public static void SetInnerMargin(this TextBoxBase self, Padding pad)
        {
            if (self.Multiline)
            {
                var r = new Rectangle(
                    pad.Left,
                    pad.Top,
                    self.ClientSize.Width - pad.Left - pad.Right,
                    self.ClientSize.Height - pad.Top - pad.Bottom);
                var nr = new RECT(r);

                SendMessage(self.Handle, EM_SETRECT, 0, ref nr);
            }
            else
            {
                SendMessage(
                    self.Handle,
                    EM_SETMARGINS, EC_LEFTMARGIN | EC_RIGHTMARGIN,
                    pad.Right * 0x10000 + pad.Left);
            }
        }
    }
}