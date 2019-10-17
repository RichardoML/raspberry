using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;


public class ReadVacuumValue
{
    public SerialPort port {get;set;}
    public StringComparer stringcomparer = StringComparer.OrdinalIgnoreCase;
    public byte[] PR1 = new byte[] {0x50,0x52,0x31,0x0A};
    public byte[] PR2 = new byte[] {0x50,0x52,0x32,0x0A};
    public byte[] PR3 = new byte[] {0x50,0x52,0x33,0x0A};
    public byte[] PRX = new byte[] {0x50,0x52,0x58,0x0A};
    public byte[] ENQ = new byte[] {0x05};
    public byte[] ACK = new byte[] {0x06,0x0A,0x0A};
    public byte[] NAK = new byte[] {0x05,0x0A,0x0A};

    public ReadVacuumValue(string com = @"COM5", int baudRate = 9600, 
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
        while(true)
        {
            RVV.send(RVV.PR1);
        }
    }
    //
    public void send(byte[] command,int startIndex=0,int length=4)
    {
        port.Write(command,startIndex,length);
    }

    public void DataReceive(object sender,SerialDataReceivedEventArgs e)
    {
        int n = port.BytesToRead;
        byte[] buf = new byte[n];
        port.Read(buf,0,n);
        if(stringcomparer.Equals(ACK,buf))
        {
            port.Write(ENQ,0,1);
        }
        else
        {
            if(stringcomparer.Equals(NAK,buf))
            {
                //do nothing 
            }
            else
            {
                Console.Write(buf);
            }
        }
    }
}
