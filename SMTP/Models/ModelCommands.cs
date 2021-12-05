using System.Collections.Generic;

namespace SMTP.Models
{
    /// <summary>
    ///     Список команд
    /// </summary>
    public class ModelCommands
    {  
        /// <summary>
        ///     Все команды для SMTP
        /// </summary>
        public List<string>  commands = new List<string>();

        /// <summary>
        ///     Все команды записывает в лист
        /// </summary>
        public ModelCommands()
        {
            commands.Add("MAIL FROM");// От кого
            commands.Add("RCPT TO");// Кому
            commands.Add("LOGIN");// Аунтификация
            commands.Add("STARTSSL");// Защита для отправления сообщения
            commands.Add("DATA");// Начать сообщение
            commands.Add("SUBJECT");// Тема сообщения
            commands.Add("BODY");// Тест сообщения
            commands.Add("ISHTML");// Тест сообщения HMTL код
            commands.Add("EHLO");//  Расширения
            commands.Add("QUIT");//   Выход
        }
    }
}
