using Newtonsoft.Json.Linq;
using System;

namespace stock_quote_alert { 

    public interface QuoteState
    {
        void sendAlert();
        void seeQuote();

    }
    public class QuoteAlert
    {
        QuoteState moreThan;
        QuoteState lessThan;
        QuoteState holder;

        QuoteState quoteState;
        bool hasChange;

        double max, min;
        String asset;
        EmailHost email;
        EmailTarget emailTo;
        DateTime lastEmail;
        int timeToSendEmail;

        DateTime lastRequest;
        int timeToRequest;
        String body;
        String apikey;
        public QuoteAlert(double max, double min, String asset, int timeToRequest,int timeToSendEmail ,String hostEmail = "joseph25vieira@gmail.com", String hostPassword = "vrxcekhnamqcjxun",String apikey= "9cbb0ac6cb604e5c96e492275f5a878b" )
        {
            
            this.max = max;
            this.min = min;
            this.asset = asset;
            this.email = new EmailHost(hostEmail, hostPassword);
            this.emailTo = new();
            this.lastEmail = DateTime.Now;
            lastEmail = lastEmail.AddSeconds(-(timeToSendEmail+1));
            this.timeToSendEmail = timeToSendEmail;
            this.hasChange = false;

            this.lastRequest = DateTime.Now;
            lastRequest = lastRequest.AddSeconds(-(timeToRequest+1));
            this.timeToRequest = timeToRequest;
            this.body = "Erro ao adquirir informações";
            this.apikey = apikey;

            moreThan = new MoreThan(this);
            lessThan = new LessThan(this);
            holder = new Holder(this);

            quoteState = holder;

        }

        public void setQuoteState(QuoteState newQuoteState)
        {
            quoteState = newQuoteState;
            hasChange = true;
        }

        public void sendAlert()
        {
            quoteState.sendAlert();
        }
        public void seeQuote()
        {
            hasChange = false;
            quoteState.seeQuote();
        }

        public QuoteState getMoreThanState() { return moreThan; }
        public QuoteState getLessThanState() { return lessThan; }
        public QuoteState getHolderState() { return holder; }
        public bool getHasChange() { return hasChange; }


        public EmailHost getEmailHost() { return email; }
        public EmailTarget getEmailTarget() { return emailTo; }
        public void setLastEmail(DateTime newLastEmail) { lastEmail = newLastEmail; }
        public DateTime getLastEmail() { return lastEmail; }
        public int getTimeToSendEmail() { return timeToSendEmail; }

        public String getAsset() { return asset; }
        public double getMin() { return min; }
        public double getMax() { return max; }

        public DateTime getLastRequest() { return lastRequest; }
        public int getTimeToRequest() { return timeToRequest; }
        

        public async Task<String> getQuote()
        {
            DateTime timeNow = new DateTime();
            timeNow = DateTime.Now;
            TimeSpan ts = timeNow - lastRequest;


            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://api.twelvedata.com/price?symbol=" + asset + "&apikey=" + apikey),
            };

            if (ts.TotalSeconds > timeToRequest)
            {
                try
                {
                    using (var response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        body = await response.Content.ReadAsStringAsync();
                        lastRequest = timeNow;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Verifique a sua conexão a internet e peça para a assistência ajuda");
                    return "Erro em adquirir os valores";
                }
            }
            var bodyQuote = JObject.Parse(body);
            var actualQuote = bodyQuote["price"];
            if (Convert.ToDouble(actualQuote)>=0 ) return (Convert.ToString(actualQuote));
            else return "Erro em adquirir os valores";
        }
    }


    public class MoreThan : QuoteState {
        QuoteAlert quoteAlert;
        public MoreThan(QuoteAlert quoteAlert)
        {
            this.quoteAlert = quoteAlert;
        }

        public void sendAlert()
        {
            String body = "<h1>Atenção</h1><h2>Alteração no valor da cotação</h2><p>Sua ação consegiu superar o valor informado</p>";
            String subtitulo = "Sua ação superou o valor desejado";
            DateTime timeNow = new DateTime();
            timeNow = DateTime.Now;
            TimeSpan ts = timeNow - quoteAlert.getLastEmail();
            if (ts.TotalSeconds > quoteAlert.getTimeToSendEmail() || quoteAlert.getHasChange())
            {
                quoteAlert.getEmailHost().sendEmail(body, subtitulo, quoteAlert.getEmailTarget());
                quoteAlert.setLastEmail(timeNow);
                Console.WriteLine("A cotação ainda está acima do valor maximo: "+ quoteAlert.getQuote().Result);
            }
        }
        public void seeQuote()
        {
            String stringQuote = quoteAlert.getQuote().Result;
            while (stringQuote == "Erro em adquir os valores" || stringQuote == "") { Console.WriteLine(stringQuote); stringQuote = quoteAlert.getQuote().Result; }
            Double quote = double.Parse(stringQuote, System.Globalization.CultureInfo.InvariantCulture);
            if (quote <= quoteAlert.getMin()) { quoteAlert.setQuoteState(quoteAlert.getLessThanState()); }
            if (quote >= quoteAlert.getMin() && quote <= quoteAlert.getMax()) { quoteAlert.setQuoteState(quoteAlert.getHolderState()); }
        } 
    }
    public class LessThan : QuoteState
    {
        QuoteAlert quoteAlert;
        public LessThan(QuoteAlert quoteAlert)
        {
            this.quoteAlert = quoteAlert;
        }

        public void sendAlert()
        {
            String body = "<h1>Atenção</h1><h2>Alteração no valor da cotação</h2><p>Sua ação caiu para abaixo do valor informado</p>";
            String subtitulo = "Sua ação ficou abaixo do esperado";
            DateTime timeNow = new DateTime();
            timeNow = DateTime.Now;
            TimeSpan ts = timeNow - quoteAlert.getLastEmail();
            if (ts.TotalSeconds > quoteAlert.getTimeToSendEmail() || quoteAlert.getHasChange())
            {

                quoteAlert.getEmailHost().sendEmail(body, subtitulo, quoteAlert.getEmailTarget());
                quoteAlert.setLastEmail(timeNow);

                Console.WriteLine("A cotação ainda está abaixo do valor minimo: " + quoteAlert.getQuote().Result);
            }

        }
        public void seeQuote()
        {
            String stringQuote = quoteAlert.getQuote().Result;
            while (stringQuote == "Erro em adquir os valores" || stringQuote == "") { stringQuote = quoteAlert.getQuote().Result; }
            Double quote = double.Parse(stringQuote, System.Globalization.CultureInfo.InvariantCulture);
            if (quote >= quoteAlert.getMax()) { quoteAlert.setQuoteState(quoteAlert.getMoreThanState()); }
            if (quote >= quoteAlert.getMin() && quote <= quoteAlert.getMax()) { quoteAlert.setQuoteState(quoteAlert.getHolderState()); }
        }
    }
    public class Holder : QuoteState
    {
        QuoteAlert quoteAlert;
        public Holder(QuoteAlert quoteAlert)
        {
            this.quoteAlert = quoteAlert;
        }

        public void sendAlert()
        {
            DateTime timeNow = new DateTime();
            timeNow = DateTime.Now;
            TimeSpan ts = timeNow - quoteAlert.getLastEmail();
            if (ts.TotalSeconds > quoteAlert.getTimeToSendEmail() || quoteAlert.getHasChange())
            {
                Console.WriteLine("A cotação ainda está abaixo do valor maximo, e superior ao valor minimo");
            }
        }
        public void seeQuote()
        {
            String stringQuote = quoteAlert.getQuote().Result;
            while (stringQuote == "Erro em adquir os valores" || stringQuote == "") { stringQuote = quoteAlert.getQuote().Result; }
            Double quote = double.Parse(stringQuote, System.Globalization.CultureInfo.InvariantCulture); 
            if (quote <= quoteAlert.getMin()) { quoteAlert.setQuoteState(quoteAlert.getLessThanState()); }
            if ( quote >= quoteAlert.getMax()) { quoteAlert.setQuoteState(quoteAlert.getMoreThanState()); }
        }

    }
}




