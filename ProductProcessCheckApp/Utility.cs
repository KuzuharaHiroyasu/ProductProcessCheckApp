using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.Storage.Streams;

namespace ProductProcessCheckApp
{
    class Utility
    {
        public static string GetUUIDString(Guid guid)
        {
            return guid.ToString();
        }

        public static Guid CreateUUID(string uuid)
        {
            return (new Guid(uuid));
        }

        public static string getFormatDeviceAddress(string address)
        {
            return separateEvery(address, ":", 2).ToUpper();
        }

        public static string separateEvery(string input, string separator, int every)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (i > 0 && i % every == 0)
                {
                    sb.Append(separator);
                }

                sb.Append(input[i]);
            }

            return sb.ToString();
        }

        public static void createDirectoryIfNeed(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 16進数文字列 => Byte配列
        /// </summary>
        public static byte[] StringToBytes(string str)
        {
            var bs = new List<byte>();
            for (int i = 0; i < str.Length / 2; i++)
            {
                bs.Add(Convert.ToByte(str.Substring(i * 2, 2), 16));
            }
            // "01-AB-EF" こういう"-"区切りを想定する場合は以下のようにする
            // var bs = str.Split('-').Select(hex => Convert.ToByte(hex, 16));

            return bs.ToArray();
        }

        /// <summary>
        /// Converts from standard 128bit UUID to the assigned 32bit UUIDs
        /// </summary>
        public static ushort ConvertUuidToShortId(Guid uuid)
        {
            // Get the short Uuid
            var bytes = uuid.ToByteArray();
            var shortUuid = (ushort)(bytes[0] | (bytes[1] << 8));

            return shortUuid;
        }

        /// <summary>
        /// Converts from a buffer to a properly sized byte array
        /// </summary>
        public static byte[] ReadBufferToBytes(IBuffer buffer)
        {
            var dataLength = buffer.Length;
            var data = new byte[dataLength];
            using (var reader = DataReader.FromBuffer(buffer))
            {
                reader.ReadBytes(data);
            }

            return data;
        }
    }
}
