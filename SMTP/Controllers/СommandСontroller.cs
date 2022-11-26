using SMTP.Entities;
using SMTP.Interfaces;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SMTP.Controllers
{
    public class СommandСontroller : ICommands
    {
        private readonly IUserController _userController;
        private readonly ISettingController _settingController;
        private readonly IDomainController _domainController;

        private readonly Message _message;

        public СommandСontroller(IUserController userController, ISettingController settingController, IDomainController domainController)
        {
            _userController = userController;
            _settingController = settingController;
            _domainController = domainController;
            _message = new Message();
        }

        public string CommandHelo() => "250 domain name should be qualified";

        public string CommandEhlo() => "250-8BITMIME\r\n250-SIZE\r\n250-STARTSSL\r\n250 LOGIN";

        public string CommandMailFrom(string messageClient)
        {
            var email = GetClearEmail(messageClient);

            if (string.IsNullOrWhiteSpace(email))
            {
                return "354 Start typing mail '<', finish with '>'";
            }

            _message.From = email;
            return "250 ok";
        }

        public string CommandRcptTo(string messageClient)
        {
            var email = GetClearEmail(messageClient);

            if (string.IsNullOrWhiteSpace(email))
            {
                return "354 Start typing mail '<', finish with '>'";
            }

            var userName = email.Split("@").First();
            var emailDomain = email.Split("@").Last();

            var nicks = _userController.GetUsers();
            var settings = _settingController.GetSettings();
            var domain = _domainController.GetDomains().FirstOrDefault(x => x.Dom == emailDomain);

            if (domain == null || (settings.Relay == false && domain.Dom != settings.Domain))
            {
                return "This server accepts only emails with its own domain";
            }

            if (settings.Relay == false || (settings.Relay == true && domain.Dom == settings.Domain))
            {
                var user = nicks.FirstOrDefault(x => x.UserName == userName);

                if (user == null)
                {
                    return "550 No such user here";
                }
            }

            return "250 ok";
        }

        public string CommandData(string clientMessage)
        {
            if (string.IsNullOrWhiteSpace(clientMessage))
            {
                return "Error, string empty";
            }

            foreach (var part in clientMessage.Split("\r\n"))
            {
                if (part.Trim().Contains(':') == false)
                {
                    _message.Body += part;
                    continue;
                }

                var component = part.Split(':');

                if (component.First() == "Subject")
                {
                    _message.Subject = component[1];
                    continue;
                }
            }

            return TrySendMessage();
        }

        private string TrySendMessage()
        {
            if (_message.To.Count == 0 ||
                _message.Body.Length == 0 ||
                _message.Subject.Length == 0 ||
                _message.From.Length == 0)
            {
                return "Error, required fields are not filled in";
            }

            var domain = _domainController.GetDomains()
                .FirstOrDefault(x => x.Dom == _message.From.Split('@')[1]);

            if (domain == null)
            {
                return "Error!";
            }

            try
            {
                var smtp = new SmtpClient(domain.Dom, domain.Port)
                {
                    Credentials = new NetworkCredential(domain.Login, domain.Password),
                    EnableSsl = true
                };

                var messageSMTP = new MailMessage
                {
                    From = new MailAddress(_message.From)
                };

                foreach (var item in _message.To)
                {
                    messageSMTP.To.Add(item);
                }

                messageSMTP.Subject = _message.Subject;
                messageSMTP.Body = _message.Body;
                smtp.Send(messageSMTP);
                return "250 ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string GetClearEmail(string message)
        {
            var ragex = new Regex(@"\w*<(.*?)>");
            return ragex.Match(message).Groups[1].Value;
        }
    }
}
