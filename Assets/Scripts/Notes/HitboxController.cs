using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxController : MonoBehaviour
{
    private MaterialPropertyBlock _propBlock;
    public Color Color1, Color2;
    public float Speed = 1, Offset;

    private SpriteRenderer spriteRenderer;
    private Animation animation;

    public bool canBePressed;
    
    void Awake()
    {
        Color1 = Color.red;
        Color2 = Color.blue;
        _propBlock = new MaterialPropertyBlock();
        animation = GetComponent<Animation>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material.SetFloat("_Color", 100f);
    }

    // Update is called once per frame
    void Update()
    {
    }
 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Cube")
        {
            // Get the current value of the material properties in the renderer.
            spriteRenderer.GetPropertyBlock(_propBlock);
            // Assign our new value.
            _propBlock.SetColor("_Color", Color.Lerp(Color1, Color2, (Mathf.Sin(Time.time * Speed + Offset) + 1) / 2f));
            // Apply the edited values to the renderer.
            spriteRenderer.SetPropertyBlock(_propBlock);
        }
    }
}
