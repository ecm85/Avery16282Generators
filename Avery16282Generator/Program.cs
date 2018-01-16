using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avery16282Generator.Brewcrafters;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator
{
    class Program
    {
        static void Main()
        {
            //var drawActionRectangles = new Queue<Action<PdfContentByte, Rectangle>>();
            //for (var i = 0; i < 55; i++)
            //{
            //    if (i < 25)
            //    {
            //        drawActionRectangles.Enqueue((contentByte, rectangle) =>
            //        {
            //            contentByte.SetColorFill(BaseColor.BLUE);
            //            DrawRectangle(contentByte, rectangle);
            //        });
            //        drawActionRectangles.Enqueue((contentByte, rectangle) =>
            //        {
            //            contentByte.SetColorFill(BaseColor.GREEN);
            //            DrawRectangle(contentByte, rectangle);
            //        });
            //    }
            //    else
            //    {
            //        drawActionRectangles.Enqueue((contentByte, rectangle) =>
            //        {
            //            contentByte.SetColorFill(BaseColor.RED);
            //            DrawRectangle(contentByte, rectangle);
            //        });
            //        drawActionRectangles.Enqueue((contentByte, rectangle) =>
            //        {
            //            contentByte.SetColorFill(BaseColor.ORANGE);
            //            DrawRectangle(contentByte, rectangle);
            //        });
            //    }
            //}
            //DrawRectangles(drawActionRectangles);
            BrewcraftersLabels.CreateLabels();
        }

        

        
    }
}
