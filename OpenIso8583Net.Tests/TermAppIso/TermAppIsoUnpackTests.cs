using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.Formatter;
using OpenIso8583Net.TermAppIso;

namespace OpenIso8583Net.Tests.TermAppIso
{
    [TestClass]
    public class TermAppIsoUnpackTests
    {
        [TestMethod]
        public void TestUnpack()
        {
            var data = Formatters.Binary.GetBytes("413138313430323330303030303032433030303030303232373133303035353030303030323134303232373135303035353830303838373737373132383837373737312020202020202020");
            var rsp = new Iso8583TermApp(data);
        }
    }
}
