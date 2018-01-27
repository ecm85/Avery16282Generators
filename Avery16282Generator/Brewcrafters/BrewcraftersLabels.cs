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
            var segoeUi = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "seguisym.ttf");
            var baseFont = BaseFont.CreateFont(segoeUi, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var beers = GetBeers();
            var labelBackground = new CMYKColor(0, 8, 26, 0);
            var drawActionRectangles = beers.SelectMany(beer => new List<Action<PdfContentByte, Rectangle>>
            {
                (contentByte, rectangle) =>
                {
                    var fontSize = string.IsNullOrWhiteSpace(beer.Name2) ? 10 : string.IsNullOrWhiteSpace(beer.Name3) ? 10 : 8;
                    var font = new Font(baseFont, fontSize, Font.NORMAL, BaseColor.BLACK);

                    contentByte.SetColorFill(labelBackground);
                    contentByte.SetColorStroke(labelBackground);
                    TextSharpHelpers.DrawRectangle(contentByte, rectangle);
                    if (beer.Points > 0)
                    {
                        var pointsImage = Image.GetInstance("Brewcrafters\\Points.png");
                        pointsImage.ScalePercent(7f);
                        pointsImage.Rotation = 4.71239f;
                        pointsImage.SetAbsolutePosition(rectangle.Left + 12, rectangle.Bottom + 3);
                        contentByte.AddImage(pointsImage);
                    }

                    var smallFontPadding = string.IsNullOrWhiteSpace(beer.Name3) ? 0 : 6;
                    ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(beer.Name1, font), smallFontPadding + rectangle.Left + fontSize * 2 + 2, rectangle.Top - 3, 270);
                    ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(beer.Name2, font), smallFontPadding + rectangle.Left + fontSize + 1, rectangle.Top - 3, 270);
                    ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(beer.Name3, font), smallFontPadding + rectangle.Left + 0, rectangle.Top - 3, 270);
                    if (beer.Points > 0)
                        ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(beer.Points.ToString(), font), rectangle.Left + 15, rectangle.Bottom + 12, 270);
                    if (!string.IsNullOrEmpty(beer.ImageName))
                    {
                        var beerImage = Image.GetInstance("Brewcrafters\\" + beer.ImageName);
                        beerImage.SetAbsolutePosition(rectangle.Left + 10, rectangle.Bottom + 16);
                        beerImage.Rotation = 4.71239f;
                        //beerImage.Rotation = 1.5708f;
                        beerImage.ScalePercent(9f);
                        contentByte.AddImage(beerImage);
                    }
                    if (beer.Barrel)
                    {
                        var barrelImage = Image.GetInstance("Brewcrafters\\Barrel.png");
                        barrelImage.Rotation = 4.71239f;
                        barrelImage.ScalePercent(20f);
                        barrelImage.SetAbsolutePosition(rectangle.Left + 12, rectangle.Bottom + 34);
                        contentByte.AddImage(barrelImage);
                    }
                    if (beer.Hops)
                    {
                        var hopsImage = Image.GetInstance("Brewcrafters\\Hops.png");
                        hopsImage.Rotation = 4.71239f;
                        hopsImage.ScalePercent(15f);
                        hopsImage.SetAbsolutePosition(rectangle.Left + 12, rectangle.Bottom + 34);
                        contentByte.AddImage(hopsImage);
                    }
                },
                (contentByte, rectangle) =>
                {
                    var fontSize = string.IsNullOrWhiteSpace(beer.Name2) ? 9 : string.IsNullOrWhiteSpace(beer.Name3) ? 10 : 8;
                    var font = new Font(baseFont, fontSize, Font.NORMAL, BaseColor.BLACK);
                    contentByte.SetColorFill(labelBackground);
                    contentByte.SetColorStroke(labelBackground);
                    TextSharpHelpers.DrawRectangle(contentByte, rectangle);
                    var smallFontPadding = string.IsNullOrWhiteSpace(beer.Name3) ? 0 : 6;
                    if (beer.Points > 0)
                    {
                        var pointsImage = Image.GetInstance("Brewcrafters\\Points.png");
                        pointsImage.ScalePercent(7f);
                        pointsImage.Rotation = 1.5708f;
                        pointsImage.SetAbsolutePosition(rectangle.Right - (12 + pointsImage.ScaledWidth),
                            rectangle.Top - (3 + pointsImage.ScaledHeight));
                        contentByte.AddImage(pointsImage);
                    }
                    ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(beer.Name1, font), rectangle.Right - (smallFontPadding + fontSize * 2 + 2), rectangle.Bottom + 3, 90);
                    ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(beer.Name2, font), rectangle.Right - (smallFontPadding + fontSize + 1), rectangle.Bottom + 3, 90);
                    ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(beer.Name3, font), rectangle.Right  - smallFontPadding, rectangle.Bottom + 3, 90);
                    if (beer.Points > 0)
                        ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT, new Phrase(beer.Points.ToString(), font), rectangle.Right - 15, rectangle.Top - 12, 90);
                    if (!string.IsNullOrEmpty(beer.ImageName))
                    {
                        var beerImage = Image.GetInstance("Brewcrafters\\" + beer.ImageName);
                        beerImage.ScalePercent(9f);
                        beerImage.SetAbsolutePosition(rectangle.Right - (beerImage.ScaledWidth + 10), rectangle.Top - (beerImage.ScaledHeight + 16));
                        beerImage.Rotation = 1.5708f;
                        contentByte.AddImage(beerImage);
                    }

                    if (beer.Barrel)
                    {
                        var barrelImage = Image.GetInstance("Brewcrafters\\Barrel.png");
                        barrelImage.Rotation = 1.5708f;
                        barrelImage.ScalePercent(20f);
                        barrelImage.SetAbsolutePosition(rectangle.Right - (barrelImage.ScaledWidth + 12), rectangle.Top - (barrelImage.ScaledHeight + 34));
                        contentByte.AddImage(barrelImage);
                    }
                    if (beer.Hops)
                    {
                        var hopsImage = Image.GetInstance("Brewcrafters\\Hops.png");
                        hopsImage.Rotation = 1.5708f;
                        hopsImage.ScalePercent(15f);
                        hopsImage.SetAbsolutePosition(rectangle.Right - (hopsImage.ScaledWidth + 12), rectangle.Top - (hopsImage.ScaledHeight + 34));
                        contentByte.AddImage(hopsImage);
                    }

                },
            }).ToList();

            var drawActionRectangleQueue = new Queue<Action<PdfContentByte, Rectangle>>(drawActionRectangles);
            PdfGenerator.DrawRectangles(drawActionRectangleQueue, labelBackground, "BrewCrafters");
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
