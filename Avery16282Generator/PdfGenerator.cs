using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;

namespace Avery16282Generator
{
    public static class PdfGenerator
    {
        public static byte[] DrawRectangles(
            Queue<Action<PdfCanvas, Rectangle>> drawRectangleActions,
            Color backgroundColor)
        {
            const int maxColumnIndex = 3;
            const int maxRowIndex = 4;
            var pageHeight = Utilities.InchesToPoints(11f);
            var pageWidth = Utilities.InchesToPoints(8.5f);
            var documentRectangle = new Rectangle(0, 0, pageWidth, pageHeight);
            using (var memoryStream = new MemoryStream())
            {
                using (var pdfWriter = new PdfWriter(memoryStream))
                {
                    using (var pdfDocument = new PdfDocument(pdfWriter))
                    {
                        using (var document = new Document(pdfDocument, new PageSize(documentRectangle)))
                        {
                            var page = AddPage(document, documentRectangle, backgroundColor);
                            var topMargin = Utilities.InchesToPoints(.50f);
                            var labelHeight = Utilities.InchesToPoints(1.60f);
                            var labelWidth = Utilities.InchesToPoints(.40f);
                            var leftMargin = Utilities.InchesToPoints(1f);
                            var verticalSpace = Utilities.InchesToPoints(.365f);
                            var horizontalSpace = Utilities.InchesToPoints(1f);
                            var extraPadding = Utilities.InchesToPoints(.05f);
                            var rowIndex = 0;
                            var columnIndex = 0;
                            var createNewPage = false;
                            while (drawRectangleActions.Any())
                            {
                                if (createNewPage)
                                {
                                    page = AddPage(document, documentRectangle, backgroundColor);
                                    createNewPage = false;
                                }

                                var lowerLeftX = leftMargin + extraPadding + columnIndex * (horizontalSpace + labelWidth * 2 + extraPadding * 4);
                                var lowerLeftY = pageHeight - (topMargin + labelHeight + extraPadding + rowIndex * (verticalSpace + labelHeight + extraPadding * 2));
                                var rectangle = new Rectangle(lowerLeftX, lowerLeftY, labelWidth, labelHeight);
                                
                                var reverseLowerLeftX = lowerLeftX + labelWidth + extraPadding * 2;
                                var reverseLowerLeftY = lowerLeftY;
                                var reverseRectangle = new Rectangle(reverseLowerLeftX, reverseLowerLeftY, labelWidth, labelHeight);
                                
                                var nextAction = drawRectangleActions.Dequeue();

                                var canvas = new PdfCanvas(page);

                                var template = new PdfFormXObject(rectangle);
                                var templateCanvas = new PdfCanvas(template, pdfDocument);
                                nextAction(templateCanvas, rectangle);
                                canvas.AddXObject(template);
                                var angle = Math.PI;
                                var xDistanceFromCenterOfPage = pageWidth / 2 - rectangle.GetX();
                                var yDistanceFromCenterOfPage = pageHeight / 2 - rectangle.GetY();

                                canvas.AddXObjectWithTransformationMatrix(template,
                                    (float)Math.Cos(angle), -(float)Math.Sin(angle),
                                    (float)Math.Sin(angle), (float)Math.Cos(angle),
                                    pageWidth - 2 * xDistanceFromCenterOfPage + rectangle.GetWidth() + reverseRectangle.GetX() - rectangle.GetX(),
                                    pageHeight - 2 * yDistanceFromCenterOfPage + rectangle.GetHeight() + reverseRectangle.GetY() - rectangle.GetY());

                                rowIndex++;
                                if (rowIndex > maxRowIndex)
                                {
                                    rowIndex = 0;
                                    columnIndex++;
                                    if (columnIndex > maxColumnIndex)
                                    {
                                        columnIndex = 0;
                                        createNewPage = true;
                                    }
                                }
                            }

                            document.Close();
                        }
                    }
                }
                return memoryStream.ToArray();
            }
        }

        private static PdfPage AddPage(Document document, Rectangle documentRectangle, Color backgroundColor)
        {
            var newPage = document.GetPdfDocument().AddNewPage();
            var canvas = new PdfCanvas(newPage);
            TextSharpHelpers.DrawRectangle(canvas, documentRectangle, backgroundColor);
            return newPage;
        }
    }
}
