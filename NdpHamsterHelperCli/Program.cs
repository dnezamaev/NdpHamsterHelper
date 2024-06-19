using NdpHamsterHelperLib;

namespace NdpHamsterHelperCli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles(@"C:\Users\user\Downloads\hamster", "*", SearchOption.AllDirectories);

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
