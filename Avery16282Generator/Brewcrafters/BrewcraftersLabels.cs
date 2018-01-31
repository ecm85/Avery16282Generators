using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator.Brewcrafters
{
    public static class BrewcraftersLabels
    {
        public static void CreateLabels()
        {
            var garamond = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "GARA.TTF");
            var garamondBold = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "GARABD.TTF");
            var baseFont = BaseFont.CreateFont(garamond, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var boldBaseFont = BaseFont.CreateFont(garamondBold, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            var beers = GetBeers();
            var labelBackground = new CMYKColor(0, 8, 26, 0);
            var drawActionRectangles = beers.SelectMany(beer => new List<Action<PdfContentByte, Rectangle>>
            {
                (canvas, rectangle) =>
                {
                    TextSharpHelpers.DrawRectangle(canvas, rectangle, labelBackground);
                    //name
                    //gold label
                    const float startOfLabelOffset = 4f;
                    var topCursor = new Cursor();
                    topCursor.AdvanceCursor(rectangle.Top - startOfLabelOffset);
                    if (beer.Points > 0)
                        DrawPoints(rectangle, topCursor, canvas, beer, boldBaseFont);
                    if (beer.Barrel)
                        DrawBarrel(rectangle, topCursor, canvas);
                    if (beer.Hops)
                        DrawHops(rectangle, topCursor, canvas);
                    if (!string.IsNullOrEmpty(beer.GoldLabelImageName))
                        DrawGoldLabel(rectangle, topCursor, canvas, beer);

                    DrawBeerName(rectangle, topCursor, canvas, beer, baseFont);
                }
            }).ToList();

            var drawActionRectangleQueue = new Queue<Action<PdfContentByte, Rectangle>>(drawActionRectangles);
            PdfGenerator.DrawRectangles(drawActionRectangleQueue, BaseColor.WHITE, "BrewCrafters");
        }

        private static void DrawBeerName(Rectangle rectangle, Cursor topCursor, PdfContentByte canvas, Beer beer,
            BaseFont baseFont)
        {
            var textRectangle = new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Right, topCursor.GetCurrent());
            var templateTextRectangle = new Rectangle(textRectangle.Height, textRectangle.Width);
            var template = canvas.CreateTemplate(templateTextRectangle.Width, templateTextRectangle.Height);
            var fontSize = TextSharpHelpers.GetMultiLineFontSize(canvas, beer.FullName, templateTextRectangle, baseFont, 12,
                               Element.ALIGN_LEFT, Font.NORMAL) - .5f;
            var textFont = new Font(baseFont, fontSize, Font.NORMAL, BaseColor.BLACK);
            TextSharpHelpers.WriteWrappingTextInRectangle(template, beer.FullName, textFont, templateTextRectangle,
                Element.ALIGN_LEFT);
            var angle = Math.PI / 2;
            canvas.AddTemplate(template,
                (float) Math.Cos(angle), -(float) Math.Sin(angle),
                (float) Math.Sin(angle), (float) Math.Cos(angle),
                textRectangle.Left, textRectangle.Top);
        }

        private static void DrawBarrel(Rectangle rectangle, Cursor topCursor, PdfContentByte canvas)
        {
            const float barrelImageHeight = 12f;
            const float barrelImageHeightPadding = 3f;
            var barrelRectangle = new Rectangle(
                rectangle.Left,
                topCursor.GetCurrent() - barrelImageHeight,
                rectangle.Right,
                topCursor.GetCurrent());
            DrawImage(barrelRectangle, canvas, "Brewcrafters\\Barrel.png", centerHorizontally: true);
            topCursor.AdvanceCursor(-(barrelRectangle.Height + barrelImageHeightPadding));
        }

        private static void DrawHops(Rectangle rectangle, Cursor topCursor, PdfContentByte canvas)
        {
            const float hopsImageHeight = 10f;
            const float hopsImageHeightPadding = 3f;
            const float hopsImageWidth = 14f;
            const float hopsImageWidthPadding = 7f;
            var hopsRectangle = new Rectangle(
                rectangle.Left + hopsImageWidthPadding,
                topCursor.GetCurrent() - hopsImageHeight,
                rectangle.Left + hopsImageWidthPadding + hopsImageWidth,
                topCursor.GetCurrent());
            DrawImage(hopsRectangle, canvas, "Brewcrafters\\Hops.png", centerVertically: true);
            topCursor.AdvanceCursor(-(hopsRectangle.Height + hopsImageHeightPadding));
        }

        private static void DrawPoints(Rectangle rectangle, Cursor topCursor, PdfContentByte canvas, Beer beer, BaseFont boldBaseFont)
        {
            const float pointsImageHeight = 12f;
            const float pointsImageHeightPadding = 3f;
            var pointsRectangle = new Rectangle(
                rectangle.Left,
                topCursor.GetCurrent() - pointsImageHeight,
                rectangle.Right,
                topCursor.GetCurrent());
            DrawImage(pointsRectangle, canvas, "Brewcrafters\\Points.png", centerHorizontally: true);
            topCursor.AdvanceCursor(-(pointsRectangle.Height + pointsImageHeightPadding));
            const float pointsTextWidthOffset = 11.5f;
            const float pointsTextHeightOffset = 3.5f;
            const float pointsFontSize = 10f;
            var pointsText = beer.Points.ToString();
            var font = new Font(boldBaseFont, pointsFontSize, Font.BOLD, BaseColor.BLACK);
            DrawText(canvas, pointsText, pointsRectangle, pointsTextWidthOffset, pointsTextHeightOffset, font);
        }

        private static void DrawGoldLabel(Rectangle rectangle, Cursor topCursor, PdfContentByte canvas, Beer beer)
        {
            const float goldLabelImageHeight = 12f;
            const float goldLabelImageHeightPadding = 3f;
            var goldLabelRectangle = new Rectangle(
                rectangle.Left,
                topCursor.GetCurrent() - goldLabelImageHeight,
                rectangle.Right,
                topCursor.GetCurrent());

            DrawImage(goldLabelRectangle, canvas, "Brewcrafters\\" + beer.GoldLabelImageName, centerHorizontally: true);
            topCursor.AdvanceCursor(-(goldLabelRectangle.Height + goldLabelImageHeightPadding));
        }

        private static void DrawImage(
            Rectangle rectangle,
            PdfContentByte canvas,
            string imagePath,
            bool scaleAbsolute = false,
            bool centerVertically = false,
            bool centerHorizontally = false)
        {
            const float imageRotationInRadians = 4.71239f;
            TextSharpHelpers.DrawImage(rectangle, canvas, imagePath, imageRotationInRadians, scaleAbsolute, centerVertically, centerHorizontally);
        }

        private static void DrawText(PdfContentByte canvas, string text, Rectangle rectangle,
            float textWidthOffset, float textHeightOffset, Font font)
        {
            const int textRotation = 270;
            TextSharpHelpers.WriteNonWrappingTextInRectangle(canvas, text, rectangle, textWidthOffset, textHeightOffset, font, textRotation);
        }

        private static IEnumerable<Beer> GetBeers()
        {
            var beerLines = File.ReadAllLines("Brewcrafters\\BrewcraftersBeerList.txt");
            return beerLines
                .Select(ConvertLineToBeer)
                .ToList();
        }

        private static Beer ConvertLineToBeer(string beerLine)
        {
            var tokens = beerLine.Split(',');
            return new Beer
            {
                //BeerType = ParseBeerType(tokens[0]),
                Name1 = tokens[1],
                Name2 = tokens[2],
                Name3  = tokens[3],
                Points = int.Parse(tokens[4]),
                Barrel = bool.Parse(tokens[5]),
                Hops = bool.Parse(tokens[6]),
                GoldLabelImageName = tokens[7],
                TokenCount = int.Parse(tokens[8])
            };
        }

        //private enum BeerType
        //{
        //    Ale,
        //    Stout,
        //    Porter
        //}

        //private static BeerType ParseBeerType(string value)
        //{
        //    switch (value)
        //    {
        //        case "Ale":
        //            return BeerType.Ale;
        //        case "Porter":
        //             return BeerType.Porter;
        //        case "Stout":
        //            return BeerType.Stout;
        //        default:
        //            throw new InvalidOperationException();
        //    }
        //}
    }
}
