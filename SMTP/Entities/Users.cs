using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SMTP.Entities
{
    /// <summary>
    ///     Пользователи.
    /// </summary>
    public class Users
    {
        /// <summary>
        ///     Все пользователи, списком.
        /// </summary>
        private static List<string> AllUsers = new List<string>();

        /// <summary>
        ///     Получает весь список пользователей.
        /// </summary>
        public Users()
        {
            if (AllUsers.Count == 0)
            {
                string[] users = Directory.GetFiles("Files/Accounts");
                for (int i = 0; i < users.Length; i++)
                {
                    users[i] = users[i].Replace("Files/Accounts\\", string.Empty);
                    users[i] = users[i].Replace(".txt", string.Empty);
                }
                AllUsers = users.ToList();
            }
        }

        /// <returns>Возращает всех пользователей списком</returns>
        public List<string> GetUsers() => AllUsers;
    }
}
