using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

//port = new SerialPort("COM3", 9600);
public class ArduinoListener : IDisposable
{
    public SerialPort port;
    public bool portExists;

    private readonly Dictionary<int, string> ColorMap = new Dictionary<int, string> { { 1, "g" }, { 2, "r" }, { 3, "o" }, { 4, "b" } };
    
    public ArduinoListener()
    {
        portExists = SerialPort.GetPortNames().Any(x => x == "COM3");

        if (portExists)
        {
            try
            {
                port = new SerialPort("COM3", 9600);
            }
            catch (Exception ex)
            {
                port = null;
            }
        }

        if (port != null)
        {
            if (port.IsOpen)
            {
                port.Close();
            }
            else
            {
                port.Open();
                port.ReadTimeout = 1000;
            }
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
    public void SendVibration(int color)
    {
        port.Write(ColorMap[color]);
    }

    public void Dispose()
    {
        port.Close();
    }
}
