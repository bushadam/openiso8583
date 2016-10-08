using System;
using System.Text;

namespace OpenIso8583Net
{
    /// <summary>
    /// Utilities class with helper functions
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Returns the luhn digit for a given PAN
        /// </summary>
        /// <param name="pan">PAN missing the luhn check digit</param>
        /// <returns>Luhn check digit</returns>
        public static string GetLuhn(string pan)
        {
            int sum = 0;

            bool alternate = true;
            for (int i = pan.Length - 1; i >= 0; i--)
            {
                int num = int.Parse(pan[i].ToString());

                if (alternate)
                {
                    num *= 2;
                    if (num > 9)
                        num = num - 9;
                }

                sum += num;
                alternate = !alternate;
            }

            int luhnDigit = 10 - (sum % 10);
            if (luhnDigit == 10)
                luhnDigit = 0;

            return luhnDigit.ToString();
        }

        /// <summary>
        /// Checks that the luhn check digit is valid
        /// </summary>
        /// <param name="pan">PAN to validate</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool IsValidPAN(String pan)
        {
            string luhn = GetLuhn(pan.Substring(0, pan.Length - 1));
            return luhn == pan.Substring(pan.Length - 1);
        }

        /// <summary>
        /// PCI DSS PAN mask. For strings longer than 10 chars masks characters [6..Length-4] 
        /// by character 'x'; otherwise returns the pan parameter unchanged.
        /// </summary>
        /// <param name="pan">a PAN string</param>
        /// <returns>a masked PAN string</returns>
        public static string MaskPan(string pan)
        {
            if (pan == null)
                return null;

            const int frontLength = 6;
            const int endLength = 4;
            const int unmaskedLength = frontLength + endLength;

            var totalLength = pan.Length;

            if (totalLength <= unmaskedLength)
                return pan;

            return
                new StringBuilder(totalLength, totalLength)
                    .Append(pan.Substring(0, frontLength)) // front
                    .Append(new string('x', totalLength - unmaskedLength))  // mask
                    .Append(pan.Substring((totalLength - endLength), endLength)) // end
                    .ToString();
        }

          /// <summary>
        /// Convert a byte array to a hex string
        /// </summary>
        /// <param name="data">
        /// The data 
        /// </param>
        /// <returns>
        /// Hex string representing the input data 
        /// </returns>
        public static string ToHex(this byte[] data)
        {
            var c = new char[data.Length * 2];

            for (int bx = 0, cx = 0; bx < data.Length; ++bx, ++cx)
            {
                var b = (byte)(data[bx] >> 4);
                c[cx] = (char)(b > 9 ? b + 0x41 - 0x0A : b + 0x30);

                b = (byte)(data[bx] & 0x0F);
                c[++cx] = (char)(b > 9 ? b + 0x41 - 0x0A : b + 0x30);
            }

            return new string(c);
        }

        /// <summary>
        /// Debug print a byte array
        /// </summary>
        /// <param name="data">
        /// The data to pring 
        /// </param>
        /// <returns>
        /// Debug output string 
        /// </returns>
        public static string DebugPrint(byte[] bytes)
        {
            var sb = new StringBuilder();

            var numberOfLines = Math.Ceiling(bytes.Length / 16.0);
            for (int line = 0; line < numberOfLines; line++)
            {
                sb.AppendLine();
                var lineOffset = line * 16;
                sb.Append(Convert.ToString(lineOffset, 16).PadLeft(5, '0'));
                sb.Append("  ");

                int endOffset = lineOffset + 16;
                if (endOffset > bytes.Length)
                    endOffset = bytes.Length;

                var charter = new char[endOffset - lineOffset];
                for (int i = lineOffset; i < endOffset; i++)
                {
                    var b = (byte)(bytes[i] >> 4);
                    sb.Append((char)(b > 9 ? b + 0x41 - 0x0A : b + 0x30));

                    b = (byte)(bytes[i] & 0x0F);
                    sb.Append((char)(b > 9 ? b + 0x41 - 0x0A : b + 0x30));

                    charter[i - lineOffset] = (bytes[i] < 0x20 || bytes[i] > 126) ? '.' : (char)bytes[i];

                    sb.Append(' ');
                }

                if (endOffset != lineOffset + 16)
                    sb.Append(' ', (lineOffset + 16 - endOffset) * 3);
                sb.Append(" ");
                sb.Append(new string(charter));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Convert a hex string to byte array.
        /// </summary>
        /// <param name="hex">
        /// The string 
        /// </param>
        /// <returns>
        /// Byte array representing the input string 
        /// </returns>
        public static byte[] ToByteArray(this string hex)
        {
            hex = hex.Replace("\n", "").Replace(" ", "");
            if (hex.Length == 0 || hex.Length % 2 != 0)
                return new byte[0];

            var buffer = new byte[hex.Length / 2];
            for (int bx = 0, sx = 0; bx < buffer.Length; ++bx, ++sx)
            {
                // Convert first half of byte
                var c = hex[sx];
                buffer[bx] = (byte)((c > '9' ? (c > 'Z' ? c - 'a' + 10 : c - 'A' + 10) : c - '0') << 4);

                // Convert second half of byte
                c = hex[++sx];
                buffer[bx] |= (byte)(c > '9' ? (c > 'Z' ? c - 'a' + 10 : c - 'A' + 10) : c - '0');
            }

            return buffer;
        }
    }
}
