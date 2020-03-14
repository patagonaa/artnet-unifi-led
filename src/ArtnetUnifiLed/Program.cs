using Haukcode.ArtNet.Packets;
using Haukcode.ArtNet.Sockets;
using Haukcode.Sockets;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;

namespace ArtnetUnifiLed
{
    class Program
    {
        private static int _universe;
        private static byte[] _lastFrame = new byte[0];

        private static readonly IList<SshClient> _sshClients = new List<SshClient>();

        static void Main(string[] args)
        {
            var configRoot = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .Build();

            var config = configRoot.Get<Configuration>();

            _universe = config.Universe;

            var devices = config.Devices;

            var socket = new ArtNetSocket
            {
                EnableBroadcast = true
            };

            socket.NewPacket += ArtnetReceived;

            foreach (var device in devices)
            {
                _sshClients.Add(new SshClient(device.Host, device.Port, device.Username, device.Password));
            }

            socket.Open(IPAddress.Any, IPAddress.Broadcast);

            while (true)
            {
                foreach (var client in _sshClients)
                {
                    if (!client.IsConnected)
                    {
                        client.Connect();
                        Console.WriteLine($"Connected to {client.ConnectionInfo.Host}");
                    }
                }
                Thread.Sleep(10000);
            }
        }

        private static void ArtnetReceived(object sender, NewPacketEventArgs<ArtNetPacket> e)
        {
            if(!(e.Packet is ArtNetDmxPacket dmxPacket))
            {
                return;
            }

            if(dmxPacket.Universe != _universe)
            {
                return;
            }

            if (dmxPacket.DmxData.SequenceEqual(_lastFrame))
            {
                return;
            }

            Console.WriteLine("Got new ArtNet Packet");

            _lastFrame = dmxPacket.DmxData;

            for (int i = 0; i < _sshClients.Count; i++)
            {
                var client = _sshClients[i];
                var dmxStartChannel = i * 2;

                var whiteValue = dmxPacket.DmxData[dmxStartChannel];
                var blueValue = dmxPacket.DmxData[dmxStartChannel+1];

                var valueToSend = 0;
                if(blueValue > 127)
                {
                    valueToSend |= 1;
                }
                if (whiteValue > 127)
                {
                    valueToSend |= 2;
                }

                Console.WriteLine($"AP {client.ConnectionInfo.Host} Color {valueToSend}");

                if (client.IsConnected)
                {
                    using (var sc = client.CreateCommand($"echo '{valueToSend.ToString(CultureInfo.InvariantCulture)}' > /proc/gpio/led_pattern"))
                    {
                        sc.Execute();
                    }
                }
            }
        }
    }
}
