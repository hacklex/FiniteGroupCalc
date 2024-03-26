using System.Drawing.Imaging;

namespace FiniteGroupCalc.Widgets
{
    public partial class TransparentLabel : Label
    {
        public TransparentLabel()
        {
            AutoSize = true;
            Visible = false;

            ImageAlign = ContentAlignment.TopLeft;
            Visible = true;

            Resize += TransparentLabelControl_Resize;
            LocationChanged += TransparentLabelControl_LocationChanged;
            TextChanged += TransparentLabelControl_TextChanged;
            ParentChanged += TransparentLabelControl_ParentChanged;
        }

        #region Events
        private void TransparentLabelControl_ParentChanged(object sender, EventArgs e)
        {
            SetTransparent();
            if (Parent != null)
            {
                Parent.ControlAdded += Parent_ControlAdded;
                Parent.ControlRemoved += Parent_ControlRemoved;
            }
        }

        private void Parent_ControlRemoved(object sender, ControlEventArgs e)
        {
            SetTransparent();
        }

        private void Parent_ControlAdded(object sender, ControlEventArgs e)
        {
            if (Bounds.IntersectsWith(e.Control.Bounds))
            {
                SetTransparent();
            }
        }

        private void TransparentLabelControl_TextChanged(object sender, EventArgs e)
        {
            SetTransparent();
        }

        private void TransparentLabelControl_LocationChanged(object sender, EventArgs e)
        {

            SetTransparent();
        }

        private void TransparentLabelControl_Resize(object sender, EventArgs e)
        {
            SetTransparent();
        }
        #endregion

        public void SetTransparent()
        {
            if (Parent != null)
            {
                Visible = false;
                Image = takeComponentScreenShot(Parent);
                Visible = true;
            }
        }

        private Bitmap takeComponentScreenShot(Control control)
        {
            Rectangle rect = control.RectangleToScreen(Bounds);
            if (rect.Width == 0 || rect.Height == 0)
            {
                return null;
            }
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);

            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

            return bmp;
        }

    }
}
