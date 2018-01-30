using System;
using System.Collections.Generic;
using Avery16282Generator.Brewcrafters;
using Avery16282Generator.Dominion;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator
{
    class Program
    {
        static void Main()
        {
            //DrawPlainRectangleLabels();
            BrewcraftersLabels.CreateLabels();
            //DominionLabels.CreateLabels();
        }

        private static void DrawPlainRectangleLabels()
        {
            var drawActionRectangles = new Queue<Action<PdfContentByte, Rectangle>>();
            for (var i = 0; i < 20; i++)
            {
                drawActionRectangles.Enqueue((canvas, rectangle) =>
                {
                    TextSharpHelpers.DrawRectangle(canvas, rectangle, BaseColor.BLUE);
                });
                drawActionRectangles.Enqueue((canvas, rectangle) =>
                {
                    TextSharpHelpers.DrawRectangle(canvas, rectangle, BaseColor.RED);
                });
            }
            PdfGenerator.DrawRectangles(drawActionRectangles, BaseColor.CYAN, "Test");
        }
    }
}
