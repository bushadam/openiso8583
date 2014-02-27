﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.TermAppIso;
using OpenIso8583Net.Formatter;
using OpenIso8583Net.FieldValidator;
using OpenIso8583Net.LengthFormatters;

namespace OpenIso8583Net.Tests.TermAppIso
{
    [TestClass]
    public class SecurityInfoFieldTests
    {
        [TestMethod]
        public void testFieldDescriptor()
        {
            IFieldDescriptor fd = new FieldDescriptor(new VariableLengthFormatter(2, 96), FieldValidators.Hex, Formatters.Binary, null);
            var expected = "3038FFFFDDDDEEEECCCC";
            var packed = fd.Pack(53, "FFFFDDDDEEEECCCC");
            var packedString = Formatters.Binary.GetString(packed);
            Assert.AreEqual(expected, packedString);
        }

        [TestMethod]
        public void testMessagePack()
        {
            var expected = "423132303000000000020008003636363038FFFFDDDDEEEECCCC";
            var msg = new Iso8583TermApp();
            msg.MessageType = Iso8583Rev93.MsgType._1200_TRAN_REQ;
            msg[Iso8583Rev93.Bit._039_ACTION_CODE] = "666";
            msg[Iso8583Rev93.Bit._053_SECURITY_INFO] = "FFFFDDDDEEEECCCC";
            byte[] bytes = msg.ToMsg();
            var actual = Formatters.Binary.GetString(bytes);
            Assert.AreEqual(expected, actual);
        }
    }
}
