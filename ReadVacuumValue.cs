using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Collections;
using System.Threading;


namespace readvalue
{
	public class ReadVacuumValue
	{
		public SerialPort port { get; set; }
		public StringComparer stringcomparer = StringComparer.OrdinalIgnoreCase;
		public byte[] PR1 = new byte[] { 0x50, 0x52, 0x31, 0x0D,0x0A };
		public byte[] PR2 = new byte[] { 0x50, 0x52, 0x32, 0x0D,0x0A };
		public byte[] PR3 = new byte[] { 0x50, 0x52, 0x33, 0x0D,0x0A};
		public byte[] PRX = new byte[] { 0x50, 0x52, 0x58, 0x0D,0x0A };
		public byte[] ENQ = new byte[] { 0x05 };
		public byte[] ACK = new byte[] { 0x06, 0x0D, 0x0A };
		public byte[] NAK = new byte[] { 0x05, 0x0D, 0x0A };

		public ReadVacuumValue(string com = @"COM3", int baudRate = 9600,
								Parity parity = Parity.None, StopBits stopBits = StopBits.One,
								int dataBits = 8, Handshake handshake = Handshake.None)
		{
			port = new SerialPort(com)
			{
				BaudRate = baudRate,
				Parity = parity,
				StopBits = stopBits,
				DataBits = dataBits,
				Handshake = handshake,

				//不加这个有些带串口的板子用不了
				RtsEnable = true
			};

			port.DataReceived += new SerialDataReceivedEventHandler(DataReceive);
			port.Open();
		}

		public static void Main()
		{
			ReadVacuumValue RVV = new ReadVacuumValue();
			while (true)
			{
				RVV.send(RVV.PR1);
				Thread.Sleep(2000);
			}
		}
		//
		public void send(byte[] command, int startIndex = 0, int length = 4)
		{
			//Console.Write("call send\t");
			//Console.Write(System.Text.Encoding.ASCII.GetString(command));
			//Console.Write("\n");
			port.Write(command, startIndex, length);
		}

		public void DataReceive(object sender, SerialDataReceivedEventArgs e)
		{

			int n = port.BytesToRead;
			byte[] buf = new byte[n];
			port.Read(buf, 0, n);

			//Console.Write("call DataReceived\t");
			//Console.Write(BitConverter.ToString(buf));
			//Console.Write("\t");
			//////Console.Write("\n");
			//Console.Write(n+"\t");
			////Console.Write(Array.Equals(buf,ACK));
			if (n==3 && buf[0] == ACK[0] && buf[1]==ACK[1] && buf[2] == ACK[2])
			{ 
				//Console.Write("send ENQ\n");
				port.Write(ENQ, 0, 1);
			}
			else
			{
				if ((n == 3 && buf[0] == NAK[0] && buf[1] == NAK[1] && buf[2] == NAK[2]))
				{
					//do nothing 
				}
				else
				{
					Console.Write(System.Text.Encoding.ASCII.GetString(buf));
				}
			}
		}
	}
}
