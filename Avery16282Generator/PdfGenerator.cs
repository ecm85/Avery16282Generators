using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator
{
    public static class PdfGenerator
    {
        public static byte[] DrawRectangles(
            Queue<Action<PdfContentByte, Rectangle>> drawRectangleActions,
            BaseColor backgroundColor)
        {
            const int maxColumnIndex = 3;
            const int maxRowIndex = 4;
            var documentRectangle = new Rectangle(0, 0, PageWidth, PageHeight);
            using (var document = new Document(documentRectangle))
            {
                using (var memoryStream = new MemoryStream())
                { 
                    using (var pdfWriter = PdfWriter.GetInstance(document, memoryStream))
                    {

                        var topMargin = Utilities.InchesToPoints(.50f);
                        var labelHeight = Utilities.InchesToPoints(1.60f);
                        var labelWidth = Utilities.InchesToPoints(.40f);
                        var leftMargin = Utilities.InchesToPoints(1f);
                        var verticalSpace = Utilities.InchesToPoints(.365f);
                        var horizontalSpace = Utilities.InchesToPoints(1f);
                        var extraPadding = Utilities.InchesToPoints(.05f);
                        var rowIndex = 0;
                        var columnIndex = 0;
                        document.Open();
                        var canvas = pdfWriter.DirectContent;

                        while (drawRectangleActions.Any())
                        {
                            if (rowIndex == 0 && columnIndex == 0)
                                AddPage(document, canvas, documentRectangle, backgroundColor);

                            var lowerLeftX = leftMargin + extraPadding + columnIndex * (horizontalSpace + labelWidth * 2 + extraPadding * 4);
                            var lowerLeftY = PageHeight - (topMargin + labelHeight + extraPadding + rowIndex * (verticalSpace + labelHeight + extraPadding * 2));
                            var upperRightX = lowerLeftX + labelWidth;
                            var upperRightY = lowerLeftY + labelHeight;
                            var rectangle = new Rectangle(lowerLeftX, lowerLeftY, upperRightX, upperRightY);
                            var reverseLowerLeftX = lowerLeftX + labelWidth + extraPadding * 2;
                            var reverseLowerLeftY = lowerLeftY;
                            var reverseUpperRightX = upperRightX + labelWidth + extraPadding * 2;
                            var reverseUpperRightY = upperRightY;
                            var reverseRectangle = new Rectangle(reverseLowerLeftX, reverseLowerLeftY, reverseUpperRightX, reverseUpperRightY);
                            var templateRectangle = new Rectangle(rectangle.Width, rectangle.Height);

                            var template = canvas.CreateTemplate(rectangle.Width, rectangle.Height);
                            var nextAction = drawRectangleActions.Dequeue();
                            nextAction(template, templateRectangle);
                            canvas.AddTemplate(template, rectangle.Left, rectangle.Bottom);
                            const double angle = Math.PI;
                            canvas.AddTemplate(template,
                                (float)Math.Cos(angle), -(float)Math.Sin(angle),
                                (float)Math.Sin(angle), (float)Math.Cos(angle),
                                reverseRectangle.Right, reverseRectangle.Top);
                            rowIndex++;
                            if (rowIndex > maxRowIndex)
                            {
                                rowIndex = 0;
                                columnIndex++;
                                if (columnIndex > maxColumnIndex)
                                {
                                    columnIndex = 0;
                                }
                            }
                        }

                        document.Close();
                    }

                    return memoryStream.ToArray();
                }
            }
        }

        private static float PageHeight => Utilities.InchesToPoints(11f);

        private static float PageWidth => Utilities.InchesToPoints(8.5f);

        private static void AddPage(IDocListener document, PdfContentByte canvas, Rectangle documentRectangle, BaseColor backgroundColor)
        {
            document.NewPage();
            TextSharpHelpers.DrawRectangle(canvas, documentRectangle, backgroundColor);
        }
    }
}
