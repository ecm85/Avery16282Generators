using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator
{
    public static class TextSharpHelpers
    {
        public static void DrawRectangle(PdfContentByte content, Rectangle rectangle)
        {
            content.Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, rectangle.Height);
            content.FillStroke();
        }
    }
}
