namespace SMTP.Models
{
    /// <summary>
    ///     Модель дополнительный опций
    /// </summary>
    public class ModelAdditionalOptions
    {
        /// <summary>
        ///     Включает SSL
        /// </summary>
        public bool EnableSSL { get; set; }

        /// <summary>
        ///     Сообщение содержит HTML код
        /// </summary>
        public bool IsBodyHTML { get; set; }
    }
}
