using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System;

namespace Azw.Iyi
{
    public class UDPSender
    {
        UdpClient client;

        public UDPSender(string host, int port)
        {
            client = new UdpClient();
            client.Connect(host, port);
        }

        public void Send(List<(string, float)> data) {
            var ms = new MemoryStream();

            ms.Write(BitConverter.GetBytes(System.DateTime.UtcNow.ToBinary()), 0, 8);

            foreach (var pair in data)
            {
                var name = Encoding.UTF8.GetBytes(pair.Item1 + "\0");
                ms.Write(name, 0, name.Length);
                ms.Write(BitConverter.GetBytes(pair.Item2), 0, 4);
            }

            var b = ms.ToArray();
            client.SendAsync(b, b.Length);
        }

        public void Close() {
            client.Close();
        }
    }
}