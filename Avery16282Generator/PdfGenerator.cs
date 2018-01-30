using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.awt.geom;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator
{
    class PdfGenerator
    {
        public static void DrawRectangles(Queue<Action<PdfContentByte, Rectangle>> drawRectangleActions, BaseColor backgroundColor, string filePrefix)
        {
            const int maxColumnIndex = 3;
            const int maxRowIndex = 4;

            var documentRectangle = new Rectangle(0, 0, PageWidth, PageHeight);
            using (var document = new Document(documentRectangle))
            {
                using (var fileStream =
                    new FileStream($@"C:\Avery\{filePrefix}Labels{DateTime.Now.ToFileTime()}.pdf",
                        FileMode.Create))
                {
                    using (var pdfWriter = PdfWriter.GetInstance(document, fileStream))
                    {
                        
                        var topMargin = Utilities.InchesToPoints(.48f);
                        var labelHeight = Utilities.InchesToPoints(1.60f);
                        var labelWidth = Utilities.InchesToPoints(.40f);
                        var leftMargin = Utilities.InchesToPoints(1.03f);
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
                            var nextAction = drawRectangleActions.Dequeue();
                            nextAction(canvas, rectangle);
                            RotateCanvas180(canvas);
                            var reverseLowerLeftX = PageWidth - (upperRightX + labelWidth + extraPadding * 2);
                            var reverseLowerLeftY = PageHeight - upperRightY;
                            var reverseUpperRightX = PageWidth - (lowerLeftX + labelWidth + extraPadding * 2);
                            var reverseUpperRightY = PageHeight - lowerLeftY;
                            var reverseRectangle = new Rectangle(reverseLowerLeftX, reverseLowerLeftY, reverseUpperRightX, reverseUpperRightY);
                            nextAction(canvas, reverseRectangle);
                            RotateCanvas180(canvas);
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
                }
            }
        }

        private static void RotateCanvas180(PdfContentByte canvas)
        {
            canvas.Transform(AffineTransform.GetRotateInstance(3.14159, PageWidth / 2, PageHeight / 2));
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
