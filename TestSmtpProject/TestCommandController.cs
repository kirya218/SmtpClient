using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMTP.Controllers;

namespace TestSmtpProject
{
    [TestClass]
    public class TestCommandController
    {
        private readonly �ommand�ontroller _�ommand = new �ommand�ontroller(new UserController(), new SettingController(), new DomainController());

        [TestMethod]
        public void TestCommandHelo()
        {
            Assert.AreEqual("250 domain name should be qualified", _�ommand.CommandHelo());
        }

        [TestMethod]
        public void TestCommandEhlo()
        {
            Assert.AreEqual("250-8BITMIME\r\n250-SIZE\r\n250-STARTSSL\r\n250 LOGIN", _�ommand.CommandEhlo());
        }

        [TestMethod]
        public void TestCommandMailFrom()
        {
            Assert.AreEqual("250 ok", _�ommand.CommandMailFrom("MAIL FROM:<kirill@mail.ru>"));
        }

        [TestMethod]
        public void TestErrorCommandMailFrom()
        {
            Assert.AreEqual("354 Start typing mail '<', finish with '>'", _�ommand.CommandMailFrom("MAIL FROM:kirill@mail.ru"));
        }

        [TestMethod]
        public void TestCommandRcptToRelay()
        {
            Assert.AreEqual("250 ok", _�ommand.CommandRcptTo("RCTP TO:<zahar0303@yamong.ru>"));
            Assert.AreEqual("550 No such user here", _�ommand.CommandRcptTo("RCTP TO:<zahar@yamong.ru>"));
            Assert.AreEqual("250 ok", _�ommand.CommandRcptTo("RCTP TO:<kirill@mail.ru>"));
        }

        [TestMethod]
        public void TestCommandData()
        {
            Assert.AreEqual("Error, required fields are not filled in", _�ommand.CommandData("Subject:Tema\r\n\r\n\r\n������\r\n��� ����?\r\n."));
            Assert.AreEqual("Error, string empty", _�ommand.CommandData(string.Empty));
            Assert.AreEqual("Error, string empty", _�ommand.CommandData("                       "));
            Assert.AreEqual("Error, string empty", _�ommand.CommandData(null));
        }
    }
}