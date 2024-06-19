using NdpHamsterHelperLib;

namespace NdpHamsterHelperCli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string screensDirectory = "screenshots";

            if (args.Length > 0)
            {
                screensDirectory = args[0];
            }

            var files = Directory.GetFiles
                (screensDirectory, "*", SearchOption.AllDirectories);

            var parser = new CardScreenshotParser();
            var cards = parser.ParseImages(files);

            var handler = new CardsManager(cards);
            var ordered = handler.OrderByPayback(cards);

            Console.WriteLine(
                string.Join(
                    Environment.NewLine,
                    ordered.Select(c => c.FriendlyName)
                ));
        }
    }
}
