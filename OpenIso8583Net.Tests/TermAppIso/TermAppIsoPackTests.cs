using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.Formatter;
using OpenIso8583Net.TermAppIso;

namespace OpenIso8583Net.Tests.TermAppIso
{
    [TestClass]
    public class TermAppIsoPackTests
    {
        [TestMethod]
        public void PackStructuredData()
        {
            Iso8583TermApp msg = new Iso8583TermApp();
            msg.MessageType = Iso8583TermApp.MsgType._1200_TRAN_REQ;
            msg[Iso8583TermApp.Bit._011_SYS_TRACE_AUDIT_NUM] = "123456";
            HashtableMessage sd = new HashtableMessage();
            sd.Add("key", "value");
            msg.StructuredData = sd;

            String actual = Formatters.Binary.GetString(msg.ToMsg());
            String expected = "4231323030002000000001000031323334353630303231F0002100013030313231336B6579313576616C7565";

            Assert.AreEqual(expected, actual);
        }
    }
}
