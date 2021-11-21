using SMTP.Models;
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
        ///     Собирает всю информацию для отправки на другой сервер
        /// </summary>
        /// <param name="info">Вся информация для отправки на другой сервер</param>
        public SetSettingSMTP(ModelInfo info)
        {
            string[] str = info.Authorization.Login.Split('@');
            SmtpClient Smtp = new SmtpClient("smtp." + str[1], info.Port);
            Smtp.Credentials = new NetworkCredential(info.Authorization.Login, info.Authorization.Password);
            Smtp.EnableSsl = info.Options.EnableSSL;
            MailMessage MessageSMTP = new MailMessage();
            MessageSMTP.From = new MailAddress(info.Message.From);
            foreach (var item in info.Message.To)
            {
                MessageSMTP.To.Add(item);
            }
            MessageSMTP.Subject = info.Message.Subject;
            MessageSMTP.Body = info.Message.Body;
            MessageSMTP.IsBodyHtml = info.Options.IsBodyHTML;
            Smtp.Send(MessageSMTP);
        }
    }
}
