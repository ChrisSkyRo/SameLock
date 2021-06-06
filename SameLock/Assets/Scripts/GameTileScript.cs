using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileScript : MonoBehaviour
{
    public bool Hover;
    public Color tileColor;

    private void Awake()
    {
        Hover = false;
        tileColor = Color.white;
        transform.parent = GameObject.Find("GameBoard").GetComponent<Transform>();
    }

    public void Setup(float scale, Color color, Sprite sprite)
    {
        transform.localScale = new Vector3(scale, scale, 1);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = color;
        tileColor = color;
    }

    private void OnMouseEnter()
    {
        if (!Application.isMobilePlatform)
            Hover = true;
    }

    private void OnMouseExit()
    {
        Hover = false;
    }
}
