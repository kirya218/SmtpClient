using SMTP.Models;
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
        public CreateMessagesController(string nickname)
        {
            StreamWriter fstream = new StreamWriter("Files/Accounts/" + nickname + ".txt", true);
            fstream.WriteLine(ModelMessage.FullMessage);
            fstream.Close();
        }
    }
}
