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
            char sep = Path.DirectorySeparatorChar;
            if (AllUsers.Count == 0)
            {
                string[] users = Directory.GetFiles($"Files{sep}Accounts");
                for (int i = 0; i < users.Length; i++)
                {
                    users[i] = users[i].Replace($"Files{sep}Accounts{sep}", string.Empty);
                    users[i] = users[i].Replace(".txt", string.Empty);
                }
                AllUsers = users.ToList();
            }
        }

        /// <returns>Возвращает всех пользователей списком</returns>
        public static List<string> GetUsers() => AllUsers;
    }
}
