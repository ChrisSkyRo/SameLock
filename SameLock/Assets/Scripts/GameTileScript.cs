using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileScript : MonoBehaviour
{
    public bool Hover, Fade, Fall, Slide;
    public Color tileColor;
    private SpriteRenderer sr;
    private float fadeTimer, tileAlpha, moveTimer, tileSize;
    private Gameplay mechanics;
    private int steps;

    private void Awake()
    {
        Hover = false;
        Fade = false;
        Fall = false;
        Slide = false;
        fadeTimer = 0.01f;
        tileAlpha = 1f;
        steps = 0;
        tileColor = Color.white;
        sr = GetComponent<SpriteRenderer>();
        transform.parent = GameObject.Find("GameBoard").GetComponent<Transform>();
        mechanics = GameObject.Find("mechanics").GetComponent<Gameplay>();
    }

    private void FixedUpdate()
    {
        if (Fade)
        {
            fadeTimer -= Time.fixedDeltaTime;

            if (fadeTimer < 0)
            {
                fadeTimer = 0.01f;
                tileAlpha -= 0.1f;
                Color alpha = tileColor;
                alpha.a = tileAlpha;
                sr.color = alpha;

                if (tileAlpha <= 0)
                    Destroy(gameObject);
            }
        }
        else if (Fall)
        {
            float fallSpeed = tileSize / 10;
            if (steps < 10)
            {
                steps++;
                transform.position -= transform.up * fallSpeed;
            }
            else
            {
                mechanics.falls--;
                steps = 0;
                Fall = false;
            }
        }
        else if (Slide)
        {
            float fallSpeed = tileSize / 10;
            if (steps < 10)
            {
                steps++;
                transform.position -= transform.right * fallSpeed;
            }
            else
            {
                mechanics.slides--;
                steps = 0;
                Slide = false;
            }
        }
    }

    public void Setup(float scale, Color color, Sprite sprite)
    {
        tileSize = scale;
        scale *= 0.625f;
        transform.localScale = new Vector3(scale, scale, 1);
        sr.sprite = sprite;
        sr.color = color;
        tileColor = color;
    }

    private void OnMouseEnter()
    {
        Hover = true;
    }

    private void OnMouseExit()
    {
        Hover = false;
    }
}
