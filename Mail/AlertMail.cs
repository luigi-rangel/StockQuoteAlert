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
        private string senderName = string.Empty;

        public AlertMail()
        {
            this.message = new MimeMessage();
        }

        public void SetFrom(string fromName, string fromAddress)
        {
            this.message.From.Add(new MailboxAddress(fromName, fromAddress));
            this.senderName = fromName;
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
            this.message.Subject = $"Alerta de preço {(alertType == AlertType.HighPrice ? "alto" : "baixo")}: " + record.Asset;
            this.message.Body = new TextPart("plain")
            {
                Text = $"Olá {this.receiverName}, \n\n" +
                $"Recomendação de {(alertType == AlertType.HighPrice ? "venda" : "compra")}: " +
                $"o preço de {record.Asset} atingiu {record.MarketPrice:C} às {record.MarkedAt:T}. " +
                $"O limite de alerta é {alertThreshold:C}. \n\n" +
                $"Atenciosamente, \n" +
                $"{this.senderName}"
            };
        }

        public MimeMessage GetMessage()
        {
            return this.message;
        }
    }
}
