using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMTP.Controllers;

namespace TestSmtpProject
{
    [TestClass]
    public class TestCommandController
    {
        private readonly —ommand—ontroller _Òommand = new —ommand—ontroller(new UserController(), new SettingController(), new DomainController());

        [TestMethod]
        public void TestCommandHelo()
        {
            Assert.AreEqual("250 domain name should be qualified", _Òommand.CommandHelo());
        }

        [TestMethod]
        public void TestCommandEhlo()
        {
            Assert.AreEqual("250-8BITMIME\r\n250-SIZE\r\n250-STARTSSL\r\n250 LOGIN", _Òommand.CommandEhlo());
        }

        [TestMethod]
        public void TestCommandMailFrom()
        {
            Assert.AreEqual("250 ok", _Òommand.CommandMailFrom("MAIL FROM:<kirill@mail.ru>"));
        }

        [TestMethod]
        public void TestErrorCommandMailFrom()
        {
            Assert.AreEqual("354 Start typing mail '<', finish with '>'", _Òommand.CommandMailFrom("MAIL FROM:kirill@mail.ru"));
        }

        [TestMethod]
        public void TestCommandRcptToRelay()
        {
            Assert.AreEqual("250 ok", _Òommand.CommandRcptTo("RCTP TO:<zahar0303@yamong.ru>"));
            Assert.AreEqual("550 No such user here", _Òommand.CommandRcptTo("RCTP TO:<zahar@yamong.ru>"));
            Assert.AreEqual("250 ok", _Òommand.CommandRcptTo("RCTP TO:<kirill@mail.ru>"));
        }

        [TestMethod]
        public void TestCommandData()
        {
            Assert.AreEqual("Error, required fields are not filled in", _Òommand.CommandData("Subject:Tema\r\n\r\n\r\nœË‚ÂÚ\r\n ‡Í ‰ÂÎ‡?\r\n."));
            Assert.AreEqual("Error, string empty", _Òommand.CommandData(string.Empty));
            Assert.AreEqual("Error, string empty", _Òommand.CommandData("                       "));
            Assert.AreEqual("Error, string empty", _Òommand.CommandData(null));
        }
    }
}