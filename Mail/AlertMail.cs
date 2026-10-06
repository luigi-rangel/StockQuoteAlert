using MimeKit;

namespace StockQuoteAlert.Mail
{
    public class AlertMail
    {
        private enum AlertType
        {
            HighPrice = 1,
            LowPrice
        }

        private readonly MimeMessage message;
        private string receiverName = string.Empty;

        public AlertMail()
        {
            this.message = new MimeMessage();
        }

        public void SetFrom(string fromName, string fromAddress)
        {
            this.message.From.Add(new MailboxAddress(fromName, fromAddress));
        }

        public void SetTo(string toName, string toAddress)
        {
            this.message.To.Add(new MailboxAddress(toName, toAddress));
            this.receiverName = toName;
        }

        public void SetHighPriceAlert(StockQuote record, decimal alertThreshold)
        {
            SetPriceAlert(AlertType.HighPrice, record, alertThreshold);
        }

        public void SetLowPriceAlert(StockQuote record, decimal alertThreshold)
        {
            SetPriceAlert(AlertType.LowPrice, record, alertThreshold);
        }

        private void SetPriceAlert(AlertType alertType, StockQuote record, decimal alertThreshold)
        {
            this.message.Subject = $"Alerta de preço { (alertType == AlertType.HighPrice ? "alto" : "baixo") }: " + record.Asset;
            this.message.Body = new TextPart("plain")
            {
                Text = $"Olá {this.receiverName}, \n\n" +
                $"O preço de {record.Asset} atingiu {record.MarketPrice:C} às {record.MarkedAt.ToLongTimeString()}. " +
                $"O limite de alerta é {alertThreshold:C}."
            };
        }

        public MimeMessage GetMessage()
        {
            return this.message;
        }
    }
}
