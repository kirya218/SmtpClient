using SMTP.Models;
using System;
using System.IO;

namespace SMTP.Controllers
{
    /// <summary>
    ///     Создает письма на сообственный домен.
    /// </summary>
    public class CreateMessagesController
    {
        /// <summary>
        ///     Создает письмо и записывает в файл.
        /// </summary>
        /// <param name="info">Все параметры письма</param>
        public CreateMessagesController(ModelInfo info, string nickname)
        {
            string message = "<from>" + info.Message.From + "</from>" + "<subject> " + info.Message.Subject + "</subject>" + "<body>" + info.Message.Body.Replace("\r\n", ".") + "</body>" + "<date>" + DateTime.Now + "</date>";
            StreamWriter fstream = new StreamWriter("Files/Accounts/" + nickname + ".txt");
            fstream.WriteLine(message);
            fstream.Close();
        }
    }
}
