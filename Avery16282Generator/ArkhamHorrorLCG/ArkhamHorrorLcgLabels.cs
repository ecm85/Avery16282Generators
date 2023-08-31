using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using Path = System.IO.Path;

namespace Avery16282Generator.ArkhamHorrorLCG;

public class ArkhamHorrorLcgLabels
{
    public static string CurrentPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    
    public const string InvestigatorStarterDecks = "Investigator Starter Decks";
    public const string ParallelChallengeScenariosAndBooks = "Parallel Challenge Scenarios and Books";
    public const string StandaloneScenarios = "Standalone Scenarios";

    public static readonly IEnumerable<string> NonCycleFolders = new[]
    {
        InvestigatorStarterDecks,
        ParallelChallengeScenariosAndBooks,
        StandaloneScenarios
    };
    
    public const string PlayerCards = "Player Cards";

    public class Divider
    {
        public string Text { get; set; }
        public string IconFullPath { get; set; }
    } 

    public static IEnumerable<Cycle> GetCycles()
    {
        return Directory
            .GetDirectories(Path.Combine(CurrentPath, "ArkhamHorrorLCG", "Arkham Horror LCG Set Icons"))
            .Select(directory => new
            {
                Directory = directory,
                CycleName = directory.Split(Path.DirectorySeparatorChar).Last()

            })
            .Where(cycle => !NonCycleFolders.Contains(cycle.CycleName))
            .SelectMany(cycle =>
            {
                var cycles = new List<Cycle>
                {
                    new() {Name = cycle.CycleName, ParentName = null}
                };
                var returnToDirectory = GetDirectoryWithPrefix(cycle.Directory, "Return to");
                if (returnToDirectory != null)
                {
                    cycles.Add(new Cycle
                    {
                        Name = returnToDirectory.Split(Path.DirectorySeparatorChar).Last(),
                        ParentName = cycle.CycleName
                    });
                }
                return cycles;
            })
            .Concat(new[] {new Cycle {Name = PlayerCards, ParentName = null}})
            .ToList();
    }

    public static byte[] CreateLabels(IList<string> selectedCycleNames, int labelsToSkip)
    {
        var teutonic = Path.Combine(CurrentPath, "Fonts", "TEUTONIC2.TTF");
        var font = PdfFontFactory.CreateFont(teutonic, true);
        
        var selectedCycles = GetCycles()
            .Where(cycle => selectedCycleNames.Contains(cycle.Name))
            .ToList();
        
        var standardCycles = selectedCycles
            .Where(cycle => cycle.Name != PlayerCards)
            .ToList();
        var background = CreateCardTypeXObject("Background.png");
        var encountersAndScenarios = standardCycles
            .SelectMany(standardCycle =>
            {
                var baseCyclePath = Path.Combine(
                    CurrentPath,
                    "ArkhamHorrorLCG",
                    "Arkham Horror LCG Set Icons",
                    standardCycle.ParentName ?? standardCycle.Name);
                var cyclePath = !string.IsNullOrWhiteSpace(standardCycle.ParentName) ? 
                    GetDirectoryWithPrefix(baseCyclePath, "Return to") :
                    baseCyclePath;
                var encounterSets = Directory.GetFiles(cyclePath)
                    .Where(file => !Path.GetFileNameWithoutExtension(file).StartsWith("_"))
                    .ToList();
                var scenarios = Directory.GetFiles(GetDirectoryWithPrefix(cyclePath, "Scenarios")).ToList();
                return encounterSets.Concat(scenarios).ToList();
            })
            .ToList();
        var allDividers = new List<Divider>();
        var encountersAndScenarioDividers = encountersAndScenarios
            .Select(fullPath =>
            {
                var fileName = Path.GetFileNameWithoutExtension(fullPath);
                return new Divider
                {
                    IconFullPath = fullPath,
                    Text = fileName.Contains(" - ") ? fileName.Split(" - ").Last() : fileName
                };
            })
            .ToList();
        allDividers.AddRange(encountersAndScenarioDividers);
        if (selectedCycles.Any(cycle => cycle.Name == PlayerCards))
        {
            var cardClasses = Directory
                .GetFiles(Path.Combine(CurrentPath, "ArkhamHorrorLCG", "Arkham Horror LCG Set Icons", "Core Set (Night of the Zealot)", "Miscellaneous"))
                .Where(path => Path.GetFileName(path).StartsWith("Investigator -"))
                .ToList();
            var cardClassDividers = cardClasses
                .Select(cardClass => new Divider
                {
                    IconFullPath = cardClass,
                    Text = Path.GetFileNameWithoutExtension(cardClass).Substring(15)
                })
                .ToList();
            allDividers.AddRange(cardClassDividers);
            var multiClassDivider = new Divider
            {
                IconFullPath = null,
                Text = "Multi-Class"
            };
            allDividers.Add(multiClassDivider);
            var investigatorsDivider = new Divider
            {
                IconFullPath = (Path.Combine(CurrentPath, "ArkhamHorrorLCG", "Arkham Horror LCG Set Icons", "Core Set (Night of the Zealot)", "Miscellaneous", "Investigators.png")),
                Text = "Investigators"
            };
            allDividers.Add(investigatorsDivider);
            var basicWeaknessDivider = new Divider
            {
                IconFullPath = (Path.Combine(CurrentPath, "ArkhamHorrorLCG", "Arkham Horror LCG Set Icons", "Core Set (Night of the Zealot)", "Miscellaneous", "Treachery.png")),
                Text = "Basic Weaknesses"
            };
            allDividers.Add(basicWeaknessDivider);
            var currentDeckDividers = Enumerable.Range(1, 4)
                .Select(index => new Divider
                {
                    IconFullPath = null,
                    Text = $"Player Deck {index}"
                })
                .ToList();
            allDividers.AddRange(currentDeckDividers);
        }

        var drawActionRectangles = allDividers.Select(foo => new Action<PdfCanvas, Rectangle>((canvas, rectangle) =>
        {
            const float maxFontSize = 15f;
            TextSharpHelpers.DrawImage(rectangle, canvas, background, System.Drawing.RotateFlipType.Rotate90FlipNone, true, true, false);
            const float imageWidth = 15f;
            const float imageHeight = imageWidth;
            const float borderPadding = 3f;
            const float textOffset = 10f;
            const float imagePadding = 3f;

            var textRectangle = new Rectangle(
                rectangle.GetX() + textOffset,
                rectangle.GetY() + borderPadding + imageWidth + imagePadding,
                rectangle.GetWidth() - (textOffset + borderPadding),
                rectangle.GetHeight() - (borderPadding + borderPadding + imageWidth + imagePadding));
            var imageRectangle = new Rectangle(textRectangle.GetX() + textRectangle.GetWidth()/2 - imageWidth/2, rectangle.GetY() + borderPadding, imageWidth, imageHeight);
            if (foo.IconFullPath != null)
                TextSharpHelpers.DrawImage(imageRectangle, canvas, foo.IconFullPath, System.Drawing.RotateFlipType.Rotate90FlipNone, false, true, true);
            var textFontSize = TextSharpHelpers.GetFontSize(canvas, foo.Text, textRectangle.GetHeight(), font, maxFontSize);
            DrawText(canvas, foo.Text, textRectangle, font, ColorConstants.BLACK, textFontSize);
        }));

        return PdfGenerator.DrawRectangles(drawActionRectangles, labelsToSkip, ColorConstants.WHITE);
    }

    private static string GetDirectoryWithPrefix(string directoryParent, string prefix)
    {
        return Directory
            .GetDirectories(directoryParent)
            .SingleOrDefault(directory => directory.Split(Path.DirectorySeparatorChar).Last().StartsWith(prefix));
    }
    
    private static PdfImageXObject CreateCardTypeXObject(string cardTypeImage)
    {
        var imagePath = Path.Combine(CurrentPath, "ArkhamHorrorLCG", cardTypeImage);
        using (var originalImage = System.Drawing.Image.FromFile(imagePath))
        {
            originalImage.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
            using (var memoryStream = new MemoryStream())
            {
                originalImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                var imageData = ImageDataFactory.Create(memoryStream.ToArray());
                return new PdfImageXObject(imageData);
            }
        }
    }
    
    private static void DrawText(
        PdfCanvas canvas,
        string text,
        Rectangle rectangle,
        PdfFont font,
        Color color,
        float size)
    {
        const float textRotation = (float)(3 * Math.PI / 2);
        TextSharpHelpers.WriteNonWrappingTextInRectangle(canvas, text, rectangle, 0f, 0f, font, color, size, textRotation, FontWeight.Regular);
    }
}