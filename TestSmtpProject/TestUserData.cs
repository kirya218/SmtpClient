using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMTP.Controllers;
using SMTP.Entities;
using SMTP.Interfaces;
using System.Collections.Generic;

namespace TestSmtpProject
{
    public class CustomUser : IUserController
    {
        private readonly List<User> _allUsers = new List<User>();

        public IEnumerable<User> GetUsers()
        {
            _allUsers.Add(new User()
            {
                UserName = "Kirill"
            });
            _allUsers.Add(new User()
            {
                UserName = "Jenok"
            });
            _allUsers.Add(new User()
            {
                UserName = ""
            });

            return _allUsers;
        }
    }

    public class CustomSettingConroller : ISettingController
    {
        public Setting GetSettings() => new Setting()
        {
            Host = "127.0.0.1",
            Port = 25,
            Domain = "yamong.ru",
            Relay = false
        };
    }

    [TestClass]
    public class TestUserData
    {
        private readonly СommandСontroller _сommand = new СommandСontroller(new CustomUser(), new CustomSettingConroller(), new DomainController());

        /*[TestMethod]
        public void TestCommandRcptToRelayTrue()
        {
            Assert.AreEqual("250 ok", _сommand.CommandRcptTo("RCTP TO:<Kirill@yamong.ru>"));
            Assert.AreEqual("250 ok", _сommand.CommandRcptTo("RCTP TO:<Jenok@yamong.ru>"));
            Assert.AreEqual("250 ok", _сommand.CommandRcptTo("RCTP TO:<@mail.ru>"));
        }*/

        [TestMethod]
        public void TestCommandRcptToRelayFalse()
        {
            Assert.AreEqual("250 ok", _сommand.CommandRcptTo("RCTP TO:<Kirill@yamong.ru>"));
            Assert.AreEqual("This server accepts only emails with its own domain", _сommand.CommandRcptTo("RCTP TO:<Jenok@mail.ru>"));
            Assert.AreEqual("This server accepts only emails with its own domain", _сommand.CommandRcptTo("RCTP TO:<@mail.ru>"));
        }
    }
}
