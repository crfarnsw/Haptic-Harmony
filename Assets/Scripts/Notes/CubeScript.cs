using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    public SpriteRenderer sr;
    public bool canBePressed;
    public bool noteWasPressed;
    public int keyToPress;
    private Color color { get; set; }

    private void Start()
    {
        noteWasPressed = false;
        sr.sortingLayerName = "Arrows";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Activator")
        {
            canBePressed = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (canBePressed)
        {
            if (!noteWasPressed)
            {
                if (GameManager.instance.arduinoListener.port.ReadByte() == keyToPress)
                {
                    noteWasPressed = true;
                    GameManager.instance.NoteHit(keyToPress);
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        canBePressed = false;

        if (!noteWasPressed)
        {
            GameManager.instance.NoteMissed(keyToPress);
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //If the note can be pressed and it wasn't pressed previously
        //if (canBePressed)
        //{
        //    if (!noteWasPressed)
        //    {
        //        if (GameManager.instance.arduinoListener.port.ReadByte() > 0)
        //        {
        //            if (GameManager.instance.arduinoListener.port.ReadByte() == keyToPress)
        //            {
        //                noteWasPressed = true;
        //                GameManager.instance.NoteHit(keyToPress);
        //                Destroy(gameObject);
        //            }
        //        }
        //        else if (Input.anyKeyDown)
        //        {
        //            if (!noteWasPressed)
        //            {
        //                noteWasPressed = true;
        //                GameManager.instance.NoteHit(keyToPress);
        //                Destroy(gameObject);
        //            }
        //        }
        //    }
        //}
    }

    public void SendVibration()
    {
        GameManager.instance.arduinoListener.SendVibration(keyToPress);
    }
}
