using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System;
using UnityEngine.XR.ARFoundation;
using UnityEngine;

namespace Azw.Iyi
{
    public class UDPSender
    {
        UdpClient client;
        string host;
        int port;

        public UDPSender(string host, int port)
        {
            client = new UdpClient();
            client.Connect(this.host = host, this.port = port);
        }

/**
    bit: data
    8: datetime
    3 * 4: position of face 
    4 * 4: rotation of face
    3 * 4: position of left eye
    4 * 4: rotation of left eye
    3 * 4: position of right eye
    4 * 4: rotation of right eye
    loop
        1: the length of the name (called N)
        N: the name of the value
        4: float value
*/
        public void Send(Transform transform, ARFace face, List<(string, float)> data) {
            
            var ms = new MemoryStream();

            ms.Write(BitConverter.GetBytes(System.DateTime.UtcNow.ToBinary()), 0, 8);

            WriteVector(ms, transform.localPosition);
            WriteVector(ms, transform.localRotation);
            WriteVector(ms, face.leftEye.localPosition);
            WriteVector(ms, face.leftEye.localRotation);
            WriteVector(ms, face.rightEye.localPosition);
            WriteVector(ms, face.rightEye.localRotation);


            foreach (var pair in data)
            {
                var name = Encoding.UTF8.GetBytes(pair.Item1);
                ms.Write(new byte[1]{(byte) name.Length}, 0, 1);
                ms.Write(name, 0, name.Length > byte.MaxValue ? byte.MaxValue : name.Length);
                ms.Write(BitConverter.GetBytes(pair.Item2), 0, 4);
            }

            var b = ms.ToArray();

            if (!client.Client.Connected) {
                Close();
                client = new UdpClient();
                client.Connect(this.host = host, this.port = port);
            }
            client.SendAsync(b, b.Length);
        }

        void WriteVector(MemoryStream ms, Vector3 transform) {
            ms.Write(BitConverter.GetBytes(transform.x), 0, 4);
            ms.Write(BitConverter.GetBytes(transform.y), 0, 4);
            ms.Write(BitConverter.GetBytes(transform.z), 0, 4);
        }
        void WriteVector(MemoryStream ms, Quaternion transform) {
            ms.Write(BitConverter.GetBytes(transform.x), 0, 4);
            ms.Write(BitConverter.GetBytes(transform.y), 0, 4);
            ms.Write(BitConverter.GetBytes(transform.z), 0, 4);
            ms.Write(BitConverter.GetBytes(transform.w), 0, 4);
        }

        public void Close() {
            client.Close();
        }
    }
}