using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

//port = new SerialPort("COM3", 9600);
public class ArduinoListener : MonoBehaviour
{
    public SerialPort port = new SerialPort("COM3", 9600);
    private readonly Dictionary<int, string> ColorMap = new Dictionary<int, string> { { 1, "g" }, { 2, "r" }, { 3, "o" }, { 4, "b" } };
    private Thread t;
    public int value = 0;

    void Start()
    {
        try
        {
            if (port != null)
            {
                port.Open();
                //port.ReadTimeout = 20;
                port.ReadTimeout = 10;
                print("port open");
            }
        }
        catch (Exception ex)
        {
            print("could not open");
        }
    }

    public byte ReadData()
    {
        byte tmpByte;
        string rxString = "";

        tmpByte = (byte)port.ReadByte();

        return tmpByte;
    }

    void Update()
    {
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
        if (color == 1)
        {
            port.Write("g");
        }

        if (color == 2)
        {
            port.Write("r");
        }
        //if (ColorMap.TryGetValue(color, out string c))
        //{
        //}
    }

    public void Dispose()
    {
        port.Close();
    }

    private void OnApplicationQuit()
    {
        Destroy(gameObject);
    }
}
