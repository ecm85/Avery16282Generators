using System;
using System.Collections.Generic;
using System.Linq;
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
            foreach (var card in DataAccess.GetCardSets().SelectMany(cardSet => cardSet.Heroes).SelectMany(hero => hero.Cards))
            {
                if (card.HeroCardSection2 == null)
                    Console.WriteLine(card.HeroCardSection1.Name + " - " + card.HeroCardSection1.HeroCardType);
                else
                    Console.WriteLine(card.HeroCardSection1.Name + " - " + card.HeroCardSection1.HeroCardType + 
                                      "   ---------   " +
                                      card.HeroCardSection2.Name + " - " + card.HeroCardSection2.HeroCardType);
            }
            Console.WriteLine(DataAccess.GetCardSets().SelectMany(cardSet => cardSet.Heroes).SelectMany(hero => hero.Cards).Where(card => card.HeroCardSection2 != null).Count());
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
