using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Avery16282Generator.Brewcrafters
{
    public class BrewcraftersLabels
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
                    //points
                    //name
                    //hops
                    //barrel
                    //gold label
                    const float startOfLabelOffset = 4f;
                    var topOfText = rectangle.Top - startOfLabelOffset;
                    if (beer.Points > 0)
                    {
                        topOfText = DrawCostAndReturnTop(rectangle, startOfLabelOffset, canvas, topOfText, beer, boldBaseFont);
                    }
                    Console.WriteLine(topOfText);
                    //var usesLine2 = string.IsNullOrWhiteSpace(beer.Name2);
                    //var usesLine3 = string.IsNullOrWhiteSpace(beer.Name3);

                    //var line1FontSize = TextSharpHelpers.GetFontSize(canvas, beer.Name1, rectangle.Height - 65, baseFont, 12, Element.ALIGN_LEFT, Font.NORMAL);
                    //var line2FontSize = usesLine2 ? (float?) null : TextSharpHelpers.GetFontSize(canvas, beer.Name2, rectangle.Height - 45, baseFont, 12, Element.ALIGN_LEFT, Font.NORMAL);
                    //var line3FontSize = usesLine3 ? (float?) null : TextSharpHelpers.GetFontSize(canvas, beer.Name3, rectangle.Height - 45, baseFont, 12, Element.ALIGN_LEFT, Font.NORMAL);
                    //var scaledFontSize = Math.Min(line1FontSize, Math.Min(line2FontSize ?? float.MaxValue, line3FontSize ?? float.MaxValue));
                    //var font = new Font(baseFont, scaledFontSize, Font.NORMAL, BaseColor.BLACK);
                    //ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(beer.Name1, font), rectangle.Left + scaledFontSize * 2 + 2, rectangle.Top - 3, 270);
                    //ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(beer.Name2, font), rectangle.Left + scaledFontSize + 1, rectangle.Top - 3, 270);
                    //ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(beer.Name3, font), rectangle.Left + 0, rectangle.Top - 3, 270);
                    //if (!string.IsNullOrEmpty(beer.ImageName))
                    //{
                    //    var beerImage = Image.GetInstance("Brewcrafters\\" + beer.ImageName);
                    //    beerImage.SetAbsolutePosition(rectangle.Left + 10, rectangle.Bottom + 16);
                    //    beerImage.Rotation = 4.71239f;
                    //    //beerImage.Rotation = 1.5708f;
                    //    beerImage.ScalePercent(9f);
                    //    canvas.AddImage(beerImage);
                    //}
                    //if (beer.Barrel)
                    //{
                    //    var barrelImage = Image.GetInstance("Brewcrafters\\Barrel.png");
                    //    barrelImage.Rotation = 4.71239f;
                    //    barrelImage.ScalePercent(20f);
                    //    barrelImage.SetAbsolutePosition(rectangle.Left + 12, rectangle.Bottom + 34);
                    //    canvas.AddImage(barrelImage);
                    //}
                    //if (beer.Hops)
                    //{
                    //    var hopsImage = Image.GetInstance("Brewcrafters\\Hops.png");
                    //    hopsImage.Rotation = 4.71239f;
                    //    hopsImage.ScalePercent(15f);
                    //    hopsImage.SetAbsolutePosition(rectangle.Left + 12, rectangle.Bottom + 34);
                    //    canvas.AddImage(hopsImage);
                    //}
                }
            }).ToList();

            var drawActionRectangleQueue = new Queue<Action<PdfContentByte, Rectangle>>(drawActionRectangles);
            PdfGenerator.DrawRectangles(drawActionRectangleQueue, BaseColor.WHITE, "BrewCrafters");
        }

        private static float DrawCostAndReturnTop(Rectangle rectangle, float startOfLabelOffset, PdfContentByte canvas,
            float topOfText, Beer beer, BaseFont boldBaseFont)
        {
            const float pointsImageHeight = 16f;
            var pointsRectangle = new Rectangle(rectangle.Left, rectangle.Top - (pointsImageHeight + startOfLabelOffset * 2),
                rectangle.Right, rectangle.Top - startOfLabelOffset);
            DrawImage(pointsRectangle, canvas, "Brewcrafters\\Points.png", centerHorizontally: true);
            topOfText -= pointsRectangle.Height;
            const float pointsTextWidthOffset = 11.5f;
            const float pointsTextHeightOffset = 7.5f;
            const float pointsFontSize = 10f;
            var pointsText = beer.Points.ToString();
            var font = new Font(boldBaseFont, pointsFontSize, Font.BOLD, BaseColor.BLACK);
            DrawText(canvas, pointsText, pointsRectangle, pointsTextWidthOffset, pointsTextHeightOffset, font);
            return topOfText;
        }

        private static Image DrawImage(
            Rectangle rectangle,
            PdfContentByte canvas,
            string imagePath,
            bool scaleAbsolute = false,
            bool centerVertically = false,
            bool centerHorizontally = false)
        {
            const float imageRotationInRadians = 4.71239f;
            return TextSharpHelpers.DrawImage(rectangle, canvas, imagePath, imageRotationInRadians, scaleAbsolute, centerVertically, centerHorizontally);
        }

        private static void DrawText(PdfContentByte canvas, string text, Rectangle rectangle,
            float textWidthOffset, float textHeightOffset, Font font)
        {
            const int textRotation = 270;
            TextSharpHelpers.DrawText(canvas, text, rectangle, textWidthOffset, textHeightOffset, font, textRotation);
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
                ImageName = tokens[7]
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
