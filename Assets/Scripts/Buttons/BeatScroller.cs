using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScroller : MonoBehaviour
{
    public float beatTempo;

    public bool hasStarted;

    // Start is called before the first frame update
    void Start()
    {
        //beatTempo = beatTempo / 60f;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasStarted)
        {
            //Debug.DrawLine(new Vector3(-10,0), new Vector3(10,0));

            //transform.position -= new Vector3(0f, beatTempo, 0f);
            transform.position += new Vector3(beatTempo, 0, 0f);
        }
    }
}
