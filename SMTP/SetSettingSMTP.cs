using SMTP.Models;
using SMTP.Settings;
using System.Net;
using System.Net.Mail;

namespace SMTP
{
    /// <summary>
    ///     Класс для отправки сообщения
    /// </summary>
    public class SetSettingSMTP
    {
        /// <summary>
        ///     Отправляет информацию на другой сервер
        /// </summary>
        public SetSettingSMTP()
        {
            string[] domain = ModelMessage.From.Split('@');
            string[] value = Domain.SettingsAllDomains[domain[1]];
            SmtpClient Smtp = new SmtpClient("smtp." + domain[1], int.Parse(value[0]));
            Smtp.Credentials = new NetworkCredential(value[1], value[2]);
            Smtp.EnableSsl = true;
            MailMessage MessageSMTP = new MailMessage();
            MessageSMTP.From = new MailAddress(ModelMessage.From);
            foreach (var item in ModelMessage.To)
            {
                MessageSMTP.To.Add(item);
            }
            MessageSMTP.Subject = ModelMessage.Subject;
            MessageSMTP.Body = ModelMessage.Body;
            Smtp.Send(MessageSMTP);
        }
    }
}
