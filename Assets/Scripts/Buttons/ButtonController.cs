using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite defaultImage;
    public Sprite pressedImage;

    public KeyCode keyToPress;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameManager.instance.cam;
        spriteRenderer = GetComponent<SpriteRenderer>();
        //cam.WorldToViewportPoint(gameObject.transform.position)
        print($"{gameObject.name} is at {gameObject.transform.position}");
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(cam.transform.position.x,cam.transform.position.y);

        if (Input.GetKeyDown(keyToPress))
        {
            spriteRenderer.sprite = pressedImage;
        }
        if (Input.GetKeyUp(keyToPress))
        {
            spriteRenderer.sprite = defaultImage;
        }
    }
}
