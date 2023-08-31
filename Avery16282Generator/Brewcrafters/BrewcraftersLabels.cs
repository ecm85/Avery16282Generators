using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Path = System.IO.Path;

namespace Avery16282Generator.Brewcrafters
{
    public static class BrewcraftersLabels
    {
        public static string CurrentPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static byte[] CreateLabels(int labelsToSkip)
        {
            var garamond = Path.Combine(CurrentPath, "Fonts", "GARA.TTF");
            var garamondBold = Path.Combine(CurrentPath, "Fonts", "GARABD.TTF");
            var font = PdfFontFactory.CreateFont(garamond, true);
            var boldFont = PdfFontFactory.CreateFont(garamondBold, true);

            var beers = GetBeers();
            var labelBackground = new DeviceRgb(254, 246, 229);
            var drawActionRectangles = beers.SelectMany(beer => new List<Action<PdfCanvas, Rectangle>>
            {
                (canvas, rectangle) =>
                {
                    TextSharpHelpers.DrawRectangle(canvas, rectangle, labelBackground);
                    //name
                    //gold label
                    const float startOfLabelOffset = 4f;
                    var topCursor = new Cursor();
                    topCursor.AdvanceCursor(rectangle.GetTop() - startOfLabelOffset);
                    if (beer.Points > 0)
                        DrawPoints(rectangle, topCursor, canvas, beer, boldFont);
                    if (beer.Barrel)
                        DrawBarrel(rectangle, topCursor, canvas);
                    if (beer.Hops)
                        DrawHops(rectangle, topCursor, canvas);
                    if (!string.IsNullOrEmpty(beer.GoldLabelImageName))
                        DrawGoldLabel(rectangle, topCursor, canvas, beer);

                    DrawBeerName(rectangle, topCursor, canvas, beer, font);
                }
            }).ToList();

            return PdfGenerator.DrawRectangles(
                drawActionRectangles,
                labelsToSkip,
                ColorConstants.WHITE);
        }

        private static void DrawBeerName(
            Rectangle rectangle,
            Cursor topCursor,
            PdfCanvas canvas,
            Beer beer,
            PdfFont font)
        {
            var textRectangle = new Rectangle(
                rectangle.GetLeft(),
                rectangle.GetBottom(),
                rectangle.GetWidth(),
                topCursor.GetCurrent() - rectangle.GetBottom());
            var fontSize = TextSharpHelpers.GetMultiLineFontSize(
                canvas,
                beer.FullName,
                textRectangle,
                font,
                12) - .5f;
            DrawCenteredWrappingText(
                canvas,
                beer.FullName,
                textRectangle,
                font,
                ColorConstants.BLACK,
                fontSize);
        }

        private static void DrawBarrel(Rectangle rectangle, Cursor topCursor, PdfCanvas canvas)
        {
            const float barrelImageHeight = 12f;
            const float barrelImageHeightPadding = 3f;
            var barrelRectangle = new Rectangle(
                rectangle.GetLeft(),
                topCursor.GetCurrent() - barrelImageHeight,
                rectangle.GetWidth(),
                barrelImageHeight);
            DrawImage(barrelRectangle, canvas, Path.Combine(CurrentPath, "Brewcrafters", "Barrel.png"), centerHorizontally: true);
            topCursor.AdvanceCursor(-(barrelRectangle.GetHeight() + barrelImageHeightPadding));
        }

        private static void DrawHops(Rectangle rectangle, Cursor topCursor, PdfCanvas canvas)
        {
            const float hopsImageHeight = 10f;
            const float hopsImageHeightPadding = 3f;
            const float hopsImageWidth = 14f;
            const float hopsImageWidthPadding = 7f;
            var hopsRectangle = new Rectangle(
                rectangle.GetLeft() + hopsImageWidthPadding,
                topCursor.GetCurrent() - hopsImageHeight,
                hopsImageWidth,
                hopsImageHeight);
            DrawImage(hopsRectangle, canvas, Path.Combine(CurrentPath, "Brewcrafters", "Hops.png"), centerVertically: true);
            topCursor.AdvanceCursor(-(hopsRectangle.GetHeight() + hopsImageHeightPadding));
        }

        private static void DrawPoints(Rectangle rectangle, Cursor topCursor, PdfCanvas canvas, Beer beer, PdfFont font)
        {
            const float pointsImageHeight = 12f;
            const float pointsImageHeightPadding = 3f;
            var pointsRectangle = new Rectangle(
                rectangle.GetLeft(),
                topCursor.GetCurrent() - pointsImageHeight,
                rectangle.GetWidth(),
                pointsImageHeight);
            DrawImage(pointsRectangle, canvas, Path.Combine(CurrentPath, "Brewcrafters", "Points.png"), centerHorizontally: true);
            topCursor.AdvanceCursor(-(pointsRectangle.GetHeight() + pointsImageHeightPadding));
            const float pointsTextWidthOffset = 9f;
            const float pointsTextHeightOffset = 3.5f;
            const float pointsFontSize = 10f;
            var pointsText = beer.Points.ToString();
            DrawText(canvas, pointsText, pointsRectangle, pointsTextWidthOffset, pointsTextHeightOffset, font, ColorConstants.BLACK, pointsFontSize, FontWeight.Bold);
        }

        private static void DrawGoldLabel(Rectangle rectangle, Cursor topCursor, PdfCanvas canvas, Beer beer)
        {
            const float goldLabelImageHeight = 12f;
            const float goldLabelImageHeightPadding = 3f;
            var goldLabelRectangle = new Rectangle(
                rectangle.GetLeft(),
                topCursor.GetCurrent() - goldLabelImageHeight,
                rectangle.GetWidth(),
                goldLabelImageHeight);

            DrawImage(goldLabelRectangle, canvas, Path.Combine(CurrentPath, "Brewcrafters", beer.GoldLabelImageName), centerHorizontally: true);
            topCursor.AdvanceCursor(-(goldLabelRectangle.GetHeight() + goldLabelImageHeightPadding));
        }

        private static void DrawImage(
            Rectangle rectangle,
            PdfCanvas canvas,
            string imagePath,
            bool scaleAbsolute = false,
            bool centerVertically = false,
            bool centerHorizontally = false)
        {
            TextSharpHelpers.DrawImage(rectangle, canvas, imagePath, System.Drawing.RotateFlipType.Rotate90FlipNone, scaleAbsolute, centerVertically, centerHorizontally);
        }

        private static void DrawCenteredWrappingText(
            PdfCanvas canvas,
            string text,
            Rectangle rectangle,
            PdfFont font,
            Color color,
            float size)
        {
            const float textRotation = (float)(3 * Math.PI / 2);
            TextSharpHelpers.WriteWrappingTextInRectangle(canvas, text, rectangle, font, color, size, textRotation);
        }

        private static void DrawText(
            PdfCanvas canvas,
            string text,
            Rectangle rectangle,
            float textWidthOffset,
            float textHeightOffset,
            PdfFont font,
            Color color,
            float size,
            FontWeight fontWeight)
        {
            const float textRotation = (float)(3 * Math.PI / 2);
            TextSharpHelpers.WriteNonWrappingTextInRectangle(canvas, text, rectangle, textWidthOffset, textHeightOffset, font, color, size, textRotation, fontWeight);
        }

        private static IEnumerable<Beer> GetBeers()
        {
            var beerLines = File.ReadAllLines(Path.Combine(CurrentPath, "Brewcrafters", "BrewcraftersBeerList.txt"));
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
                Name3 = tokens[3],
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
