namespace SMTP.Models
{
    /// <summary>
    ///     Модель авторизации
    /// </summary>
    public class ModelAuthorization
    {
        /// <summary>
        ///     Логин пользователя для авторизации
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        ///     Пароль пользователя для авторизации
        /// </summary>
        public string Password { get; set; }
    }
}
