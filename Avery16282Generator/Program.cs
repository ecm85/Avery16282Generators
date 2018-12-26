using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avery16282Generator.Dominion;
using Avery16282Generator.Legendary;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator
{
    class Program
    {
        static void Main()
        {
            //DrawPlainRectangleLabels();
            //BrewcraftersLabels.CreateLabels();
            //DominionLabels.CreateLabels();
            //AeonsEndLabels.CreateLabels();
            LegendaryLabels.CreateLabels();
            //var sets =
            //    (DataAccess.GetStartingCardSets().Select(set => set.SetName))
            //    .Concat((DataAccess.GetHenchmenCardSets().Select(set => set.SetName)))
            //    .Concat((DataAccess.GetHeroCardSets().Select(set => set.SetName)))
            //    .Concat((DataAccess.GetMastermindCardSets().Select(set => set.SetName)))
            //    .Concat((DataAccess.GetSetupCardSets().Select(set => set.SetName)))
            //    .Concat((DataAccess.GetVillainCardSets().Select(set => set.SetName)))
            //    .Distinct()
            //    .OrderBy(set => set)
            //    .ToList();
            //foreach (var set in sets)
            //{
            //    Console.WriteLine(set);
            //}

            //var directory = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            //foreach (var file in Directory.EnumerateFiles(directory))
            //{
            //    Console.WriteLine(file);
            //}

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
