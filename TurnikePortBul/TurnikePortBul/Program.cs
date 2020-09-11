using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TurnikePortBul
{
	class Program
	{
		static void Main(string[] args)
		{
			SerialPort sp = new SerialPort();
			foreach (string item in SerialPort.GetPortNames())
			{
				Console.WriteLine(item);
			}
			Console.WriteLine("--------------");
			while (true)
			{
				if (sp.IsOpen)
				{
					sp.Close();
				}
				sp.Dispose();
				sp = new SerialPort(Console.ReadLine(),9600);
				sp.Open();
				while (true)
				{
					byte[] yazilcak = { 0x02, 0xff, 0x96, 0x04, 0x0b, 0x36, 0x34, 0x03 };
					sp.Write(yazilcak, 0, yazilcak.Length);
					Thread.Sleep(500);
					byte[] gelen = new byte[sp.BytesToRead];
					sp.Read(gelen, 0, sp.BytesToRead);
					Console.WriteLine(ByteArrayToString(gelen));
				}
				

			}
		}
		public static string ByteArrayToString(byte[] ba)
		{
			StringBuilder hex = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba)
				hex.AppendFormat("{0:x2}", b);
			return hex.ToString();
		}
	}
}
