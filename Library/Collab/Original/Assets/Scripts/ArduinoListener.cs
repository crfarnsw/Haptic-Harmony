using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

//port = new SerialPort("COM3", 9600);
public class ArduinoListener : IDisposable
{
    //public SerialPort port;
    public bool portExists;
    public static EnhancedSerialPort port;
    public ArduinoListener()
    {
        //portExists = SerialPort.GetPortNames().Any(x => x == "COM3");

        if (portExists)
        {
            try
            {
                //port = new SerialPort("COM3", 9600);
                port = new EnhancedSerialPort("COM3", 9600);
                port.DataReceived += HandlePortDataReceived;
                port.ReadTimeout = 100;
                port.Open();
            }
            catch (Exception ex)
            {
                port = null;
            }
        }

        //if (port != null)
        //{
        //    if (port.IsOpen)
        //    {
        //        port.Close();
        //    }
        //    else
        //    {
        //        port.Open();
        //        port.ReadTimeout = 2;
        //    }
        //}
    }

    static void HandlePortDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        while (port.BytesToRead > 0)
        {
            int bt = port.ReadByte();
            Console.WriteLine("{0}", bt);
        }
    }

    public bool anyKeyDown()
    {
        return port.ReadByte() > -1;
    }

    /// <summary>
    /// Here are the correlations to respective buttons: 1, 2, 3, 4 represent green, red, orange, and blue respectively.
    /// </summary>
    /// <returns></returns>
    public int ReceiveInput()
    {
        return port.ReadByte();
    }

    //Call this function to initiate a vibration on the controller. 
    //Acceptable inputs are as follows the following strings:
    // {g, r, o, b, gr, go, gb, ro, rb, ob, grb, gro, gob, rob, grob}
    //Other inputs will result in no vibration occuring
    public void SendVibration(Color color)
    {
        string c = string.Empty;

        // temporary lol
        if (color == Color.green)
        {
            c = "g";
        }

        if (color == Color.red)
        {
            c = "r";
        }

        if (color == Color.yellow)
        {
            c = "o";
        }

        if (color == Color.blue)
        {
            c = "b";
        }

        port.Write(c);
    }

    public void Dispose()
    {
        port.Close();
    }
}
