using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net;

namespace OpenIso8583Net.Tests
{
    /// <summary>
    ///   Summary description for Iso8583PostTests
    /// </summary>
    [TestClass]
    public class Iso8583PostTests
    {
        ///<summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestIso8583PostToMsg()
        {
            var msg = new Iso8583Post();
            msg.MessageType = 0x200;
            msg[3] = "000000";
            msg.Private[2] = "hello";
            var actual = msg.ToMsg();

            var mtid = Encoding.ASCII.GetBytes("0200");

            var bitmap = new Bitmap();
            bitmap[3] = true;
            bitmap[127] = true;
            var primaryBitmap = bitmap.ToMsg();
            var primaryMessageContent = Encoding.ASCII.GetBytes("000000");

            bitmap = new Bitmap();
            bitmap[2] = true;
            var privateBitmap = bitmap.ToMsg();
            var privateContent = Encoding.ASCII.GetBytes("05hello");
            var privateLength = privateBitmap.Length + privateContent.Length;
            var privateMessage = new byte[privateLength];
            Array.Copy(privateBitmap, privateMessage, privateBitmap.Length);
            Array.Copy(privateContent, 0, privateMessage, privateBitmap.Length, privateContent.Length);
            var privateMessageLengthHeader = Encoding.ASCII.GetBytes(privateLength.ToString().PadLeft(6, '0'));

            var messageLength = 4 + primaryBitmap.Length + 6 + 6 + privateMessage.Length;

            var message = new byte[messageLength];
            var offset = 0;
            Array.Copy(mtid, 0, message, offset, mtid.Length);
            offset += mtid.Length;

            Array.Copy(primaryBitmap, 0, message, offset, primaryBitmap.Length);
            offset += primaryBitmap.Length;

            Array.Copy(primaryMessageContent, 0, message, offset, primaryMessageContent.Length);
            offset += primaryMessageContent.Length;

            Array.Copy(privateMessageLengthHeader, 0, message, offset, privateMessageLengthHeader.Length);
            offset += privateMessageLengthHeader.Length;

            Array.Copy(privateMessage, 0, message, offset, privateMessage.Length);

            Assert.AreEqual(messageLength, msg.PackedLength, "Message length not equal");

            var equals = true;
            for (var i = 0; i < messageLength; i++)
                equals &= message[i] == actual[i];

            Assert.AreEqual(true, equals, "Messages not equal");
        }

        [TestMethod]
        public void TestIso8583PostUnpack()
        {
            var mtid = Encoding.ASCII.GetBytes("0200");

            var bitmap = new Bitmap();
            bitmap[3] = true;
            bitmap[127] = true;
            var primaryBitmap = bitmap.ToMsg();
            var primaryMessageContent = Encoding.ASCII.GetBytes("000000");

            bitmap = new Bitmap();
            bitmap[2] = true;
            var privateBitmap = bitmap.ToMsg();
            var privateContent = Encoding.ASCII.GetBytes("05hello");
            var privateLength = privateBitmap.Length + privateContent.Length;
            var privateMessage = new byte[privateLength];
            Array.Copy(privateBitmap, privateMessage, privateBitmap.Length);
            Array.Copy(privateContent, 0, privateMessage, privateBitmap.Length, privateContent.Length);
            var privateMessageLengthHeader = Encoding.ASCII.GetBytes(privateLength.ToString().PadLeft(6, '0'));

            var messageLength = 4 + primaryBitmap.Length + 6 + 6 + privateMessage.Length;

            var message = new byte[messageLength];
            var offset = 0;
            Array.Copy(mtid, 0, message, offset, mtid.Length);
            offset += mtid.Length;

            Array.Copy(primaryBitmap, 0, message, offset, primaryBitmap.Length);
            offset += primaryBitmap.Length;

            Array.Copy(primaryMessageContent, 0, message, offset, primaryMessageContent.Length);
            offset += primaryMessageContent.Length;

            Array.Copy(privateMessageLengthHeader, 0, message, offset, privateMessageLengthHeader.Length);
            offset += privateMessageLengthHeader.Length;

            Array.Copy(privateMessage, 0, message, offset, privateMessage.Length);

            var msg = new Iso8583Post();
            msg.Unpack(message, 0);

            Assert.AreEqual(message.Length, msg.PackedLength);
            Assert.AreEqual("000000", msg[3]);
            Assert.AreEqual("hello", msg.Private[2]);
        }

        [TestMethod]
        public void TestDontKnow()
        {
            var msg = new Iso8583Post();
            msg.MessageType = Iso8583.MsgType._0200_TRAN_REQ;
            msg[3] = "270000";
            msg.TransactionAmount = 400;
            msg.TransmissionDateTime.SetNow();
            msg[11] = "123456";
            msg[12] = "151518";
            msg[13] = "1212";
            msg[22] = "012";
            msg[25] = "00";
            msg[26] = "12";
            msg[32] = "588892";
            msg[33] = "123456";
            msg[37] = "123456789123";
            msg[41] = "21458796";
            msg[42] = "100200300400500";
            msg[43] = new string('x', 40);
            msg[48] = "A";
            msg[49] = "716";
            msg[100] = "123456";
            msg[102] = "9012273811";
            msg[103] = "010203040506";
            msg[123] = "100111100130119";

            msg.Private[Field127.Bit._002_SWITCH_KEY] = DateTime.Now.ToString("yyyyMMDDHHmmss");

#pragma warning disable 168
            var data = msg.ToMsg();
#pragma warning restore 168
        }
    }
}