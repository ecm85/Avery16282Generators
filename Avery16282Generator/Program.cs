using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator
{
    class Program
    {
        static void Main()
        {
            var drawActionRectangles = new Queue<Action<PdfContentByte, Rectangle>>();
            for (var i = 0; i < 55; i++)
            {
                if (i < 25)
                {
                    drawActionRectangles.Enqueue((contentByte, rectangle) =>
                    {
                        contentByte.SetColorFill(BaseColor.BLUE);
                        DrawRectangle(contentByte, rectangle);
                    });
                    drawActionRectangles.Enqueue((contentByte, rectangle) =>
                    {
                        contentByte.SetColorFill(BaseColor.GREEN);
                        DrawRectangle(contentByte, rectangle);
                    });
                }
                else
                {
                    drawActionRectangles.Enqueue((contentByte, rectangle) =>
                    {
                        contentByte.SetColorFill(BaseColor.RED);
                        DrawRectangle(contentByte, rectangle);
                    });
                    drawActionRectangles.Enqueue((contentByte, rectangle) =>
                    {
                        contentByte.SetColorFill(BaseColor.ORANGE);
                        DrawRectangle(contentByte, rectangle);
                    });
                }
            }
            DrawRectangles(drawActionRectangles);
        }

        public static void DrawRectangles(Queue<Action<PdfContentByte, Rectangle>> drawRectangleActions)
        {
            var pageWidth = Utilities.InchesToPoints(8.5f);
            var pageHeight = Utilities.InchesToPoints(11f);
            var documentRectangle = new Rectangle(0, 0, pageWidth, pageHeight);
            using (var document = new Document(documentRectangle))
            {
                using (var fileStream =
                    new FileStream($@"C:\Avery\Labels{DateTime.Now.ToFileTime()}.pdf",
                        FileMode.Create))
                {
                    using (var pdfWriter = PdfWriter.GetInstance(document, fileStream))
                    {
                        document.Open();
                        document.NewPage();
                        var contentByte = pdfWriter.DirectContent;
                        var topMargin = Utilities.InchesToPoints(.5f);
                        var labelHeight = Utilities.InchesToPoints(1.75f);
                        var labelWidth = Utilities.InchesToPoints(.5f);
                        var leftMargin = Utilities.InchesToPoints(1f);
                        var verticalSpace = Utilities.InchesToPoints(.3125f);
                        var horizontalSpace = Utilities.InchesToPoints(1f);
                        var rowIndex = 0;
                        var columnIndex = 0;
                        while(drawRectangleActions.Any())
                        {
                            var lowerLeftX = leftMargin + columnIndex * (horizontalSpace + labelWidth * 2);
                            var lowerLeftY = pageHeight - (topMargin + labelHeight + rowIndex * (verticalSpace + labelHeight));
                            var upperRightX = lowerLeftX + labelWidth;
                            var upperRightY = lowerLeftY + labelHeight;
                            var rectangle = new Rectangle(lowerLeftX, lowerLeftY, upperRightX, upperRightY);
                            var reverseRectangle = new Rectangle(lowerLeftX + labelWidth, lowerLeftY, upperRightX + labelWidth, upperRightY);
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
                                    document.NewPage();
                                }
                            }
                        }
                        
                        document.Close();
                    }
                }
            }
        }

        private static void DrawRectangle(PdfContentByte content, Rectangle rectangle)
        {
            content.Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, rectangle.Height);
            content.FillStroke();
        }
    }
}
