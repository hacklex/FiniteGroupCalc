using System.ComponentModel;
using System.Reflection;

namespace FiniteGroupCalc.Widgets
{
    /// <summary>
    /// A NumericUpDown that allows setting the inner margins and the text alignment.
    /// </summary>
    public class NumericUpDownEx : NumericUpDown
    {
        private Padding inMargin = Padding.Empty;
        /// <summary>
        /// Gets or sets the inner margins. The Left and Right only.
        /// The Top and Bottom margins are ignored.
        /// </summary>
        [DefaultValue(typeof(Padding), "0, 0, 0, 0")]
        public Padding InnerMargin
        {
            get => inMargin;
            set
            {
                if (inMargin == value) return;
                
                inMargin = value;
                if (typeof(NumericUpDown).GetField("_upDownEdit", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.GetValue(this) is not TextBox udEdit) return;                
                udEdit.Multiline = true;
                udEdit.Height = 30;
                SetInnerMargins();
            }
        }

        /// <inheritdoc cref="TextBox.TextAlign"/>
        new public HorizontalAlignment TextAlign
        {
            get => base.TextAlign;
            set
            {
                if (base.TextAlign != value)
                {
                    base.TextAlign = value;
                    SetInnerMargins();
                }
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SetInnerMargins();
        }

        private void SetInnerMargins()
        {
            if (Controls[1] is TextBox tb)
                tb.SetInnerMargin(InnerMargin);
        }
    }
}