using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMTP.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace TestSmtpProject
{
    [TestClass]
    public class TestFiles
    {
        private readonly SettingController _settingController = new SettingController();
        private readonly DomainController _domainController = new DomainController();
        private readonly UserController _userController = new UserController();

        [TestMethod]
        public void TestSettings()
        {
            Assert.AreEqual("127.0.0.1", _settingController.GetSettings().Host);
            Assert.AreEqual(1024, _settingController.GetSettings().Port);
            Assert.AreEqual(true, _settingController.GetSettings().Relay);
            Assert.AreEqual("yamong.ru", _settingController.GetSettings().Domain);
        }

        [TestMethod]
        public void TestDomains()
        {
            Assert.AreEqual("mail.ru", _domainController.GetDomains().First().Dom);
            Assert.AreEqual(25, _domainController.GetDomains().First().Port);
            Assert.AreEqual("boss.niga03@mail.ru", _domainController.GetDomains().First().Login);
            Assert.AreEqual("hQ86vJf7K6k9e3HtEWcG", _domainController.GetDomains().First().Password);
        }

        [TestMethod]
        public void TestUsers()
        {
            Assert.AreEqual("kostya123", _userController.GetUsers().First().UserName);
            Assert.AreEqual("zahar0303", _userController.GetUsers().Last().UserName);
        }
    }
}
