
namespace stock_quote_alert
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Inicialize o executavel da seguinte maneira: \n stock-quote-alert.exe PETR4 22.67 22.59 ");
            }
            double max = double.Parse(args[1], System.Globalization.CultureInfo.InvariantCulture);
            double min = double.Parse(args[2], System.Globalization.CultureInfo.InvariantCulture);
            
            //Create newSection
            Console.WriteLine("Sistema de controle de cotação");
            QuoteAlert newSection = new QuoteAlert(max, min, args[0], 1 * 60, 60*60);

            // Set and execute command
            while (true)
            {
                newSection.seeQuote();
                newSection.sendAlert();
            }
        }
     }
}
