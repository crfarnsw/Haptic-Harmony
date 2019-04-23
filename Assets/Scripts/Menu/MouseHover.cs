using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHover : MonoBehaviour
{
    // Original Menu Item Color
    void Start()
    {
        GetComponent<Renderer>().material.color = Color.white;  
    }

    // Color when selected by mouse
    void OnMouseEnter()
    {
        GetComponent<Renderer>().material.color = Color.yellow;   
    }
    //Color when removing mouse from item
    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }
}
