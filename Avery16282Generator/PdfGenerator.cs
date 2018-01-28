using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator
{
    class PdfGenerator
    {
        public static void DrawRectangles(Queue<Action<PdfContentByte, Rectangle>> drawRectangleActions, BaseColor backgroundColor, string filePrefix)
        {
            var pageWidth = Utilities.InchesToPoints(8.5f);
            var pageHeight = Utilities.InchesToPoints(11f);
            var documentRectangle = new Rectangle(0, 0, pageWidth, pageHeight);
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
                        var contentByte = pdfWriter.DirectContent;

                        while (drawRectangleActions.Any())
                        {
                            if (rowIndex == 0 && columnIndex == 0)
                                AddPage(document, contentByte, documentRectangle, backgroundColor);

                            var lowerLeftX = leftMargin + extraPadding + columnIndex * (horizontalSpace + labelWidth * 2 + extraPadding * 4);
                            var lowerLeftY = pageHeight - (topMargin + labelHeight + extraPadding + rowIndex * (verticalSpace + labelHeight + extraPadding * 2));
                            var upperRightX = lowerLeftX + labelWidth;
                            var upperRightY = lowerLeftY + labelHeight;
                            var rectangle = new Rectangle(lowerLeftX, lowerLeftY, upperRightX, upperRightY);
                            var reverseRectangle = new Rectangle(lowerLeftX + labelWidth + extraPadding * 2, lowerLeftY, upperRightX + labelWidth + extraPadding * 2, upperRightY);
                            var nextAction = drawRectangleActions.Dequeue();
                            nextAction(contentByte, rectangle);
                            var reverseAction = drawRectangleActions.Dequeue();
                            reverseAction(contentByte, reverseRectangle);
                            rowIndex++;
                            if (rowIndex > 4)
                            {
                                rowIndex = 0;
                                columnIndex++;
                                if (columnIndex > 3)
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

        private static void AddPage(Document document, PdfContentByte contentByte, Rectangle documentRectangle, BaseColor backgroundColor)
        {
            document.NewPage();
            TextSharpHelpers.DrawRectangle(contentByte, documentRectangle, backgroundColor);
        }
    }
}
