using System;
using System.Configuration;

namespace stock_quote_alert
{
    public class EmailTarget
    {
        private String email;
        private String client;
        private String port;
        public EmailTarget(string email, string client, string port)
        {
            this.email = email;
            this.client = client;
            this.port = port;
        }
        public EmailTarget()
        {
            
            readFromConfig();
        }
        public void readFromConfig()
        {
            this.email = ConfigurationManager.AppSettings["email"].ToString();
            this.client = ConfigurationManager.AppSettings["client"].ToString();
            this.port = ConfigurationManager.AppSettings["port"].ToString();
        }
        public String getEmail() { return email; }
        public String getClient() { return client; }
        public int getPort() { return  Convert.ToInt16(port); }
    }
}
