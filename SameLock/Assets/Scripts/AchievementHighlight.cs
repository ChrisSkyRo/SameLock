using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AchievementHighlight : MonoBehaviour
{
    private SpriteRenderer sr;
    private TextMeshPro[] text;
    private Color[] colorsArray;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        text = GetComponentsInChildren<TextMeshPro>();
        colorsArray = new Color[6]
        {
            Color.red,
            Color.magenta,
            Color.yellow,
            Color.blue,
            Color.cyan,
            Color.green,
        };
    }

    private void OnMouseEnter()
    {
        sr.color = colorsArray[Random.Range(0, 6)];
        text[0].color = colorsArray[Random.Range(0, 6)];
        text[1].color = colorsArray[Random.Range(0, 6)];
    }

    private void OnMouseExit()
    {
        sr.color = Color.white;
        text[0].color = Color.white;
        text[1].color = Color.white;
    }

}
