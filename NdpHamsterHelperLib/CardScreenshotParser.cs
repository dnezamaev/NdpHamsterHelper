using System.Diagnostics;
using System.Text.RegularExpressions;

using Tesseract;

namespace NdpHamsterHelperLib
{
    public record Card(string FilePath, double Income, double Price)
    {
        public double Payback { get => Price / Income; }

        public string FriendlyName =>
            $"{Payback:0.##} = {Price:0.##} / {Income:0.##} - {FilePath}";
    }

    public class CardScreenshotParser
    {
        private const char ThousandsSuffix = 'K';

        public List<Card> ParseImages(IEnumerable<string> imagePathes)
        {
            var parsed = imagePathes
                .AsParallel()
                .Select(p =>
                    {
                        try
                        {
                            return ParseImage(p);
                        }
                        catch
                        {
                            return null;
                        }
                    }
                )
                .Where(p => p != null)
                .ToList();

            return parsed;
        }

        public Card ParseImage(string filePath)
        {
            using (var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(filePath))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        return ParseTextLines(text, filePath);
                    }
                }
            }
        }

        private Card ParseTextLines(string text, string filePath)
        {
            var lines = text
                .Split("\n", StringSplitOptions.RemoveEmptyEntries);

            var numberLines = lines
                .Reverse()
                .Where(l => l.StartsWith("@"))
                .Take(2)
                .Select(l => l.ToUpper())
                .ToList();

            var cleanNumberLines = numberLines
                .Select(l => Regex.Replace(l, "[^0-9K,]", ""))
                .Select(ParseNumber)
                .ToList();

            if (cleanNumberLines.Count != 2)
            {
                throw new Exception("Income and/or price not found on card");
            }

            return new Card(filePath, cleanNumberLines[1], cleanNumberLines[0]);
        }

        private Card ParseTextRegex(string text, string filePath)
        {
            // Dollar picture recognised as at symbol.
            var regex = new Regex("^@[0-9\\s+,]+[K]?$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            var regexMatches = regex.Matches(text.ToUpper()).ToList();

            var numbers = regexMatches
                // Remove unnecessary symbols.
                .Select(m => Regex.Replace(m.Value, "[@\\s+]", ""))
                .Select(ParseNumber)
                .ToList();

            if (numbers.Count != 2)
            {
                throw new Exception("Income and/or price not found on card");
            }

            return new Card(filePath, numbers[0], numbers[1]);
        }

        private static double ParseNumber(string text)
        {
            // Raw number.
            if (!text.EndsWith(ThousandsSuffix))
            {
                return double.Parse(text);
            }

            // Shorten number with K on end.
            var rawNumber = text.TrimEnd(ThousandsSuffix);
            var number = double.Parse(rawNumber) * 1000;
            return number;
        }
    }
}
