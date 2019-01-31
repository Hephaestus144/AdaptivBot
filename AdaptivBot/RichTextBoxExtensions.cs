using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace AdaptivBot
{
    public static class RichTextBoxExtensions
    {
        public static void AppendText(
            this RichTextBox box,
            string text,
            Color color,
            Font font)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = color;
            box.SelectionFont = font;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
}
