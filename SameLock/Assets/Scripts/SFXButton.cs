using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXButton : MonoBehaviour
{
    public Sprite Checked, Empty;
    private SpriteRenderer sr;
    private Color[] colorsArray;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
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

    private void Start()
    {
        int current = PlayerPrefs.GetInt("SFX");
         PlayerPrefs.Save();
        if (current == 1)
            sr.sprite = Checked;
        else
           sr.sprite = Empty;
    }

    private void OnMouseEnter()
    {
        sr.color = colorsArray[Random.Range(0, 6)];
    }

    private void OnMouseExit()
    {
        sr.color = Color.white;
    }

    private void OnMouseUp()
    {
        GameData gd = GameObject.Find("GameData").GetComponent<GameData>();
        int previous = gd.SFX;
        if (previous == 1)
        {
            PlayerPrefs.SetInt("SFX", 0);
            gd.SFX = 0;
            sr.sprite = Empty;
        }
        else
        {
            GameObject.Find("SFXManager").GetComponent<SFXScript>().PlaySFX("menu");
            PlayerPrefs.SetInt("SFX", 1);
            gd.SFX = 1;
            sr.sprite = Checked;
        }
        PlayerPrefs.Save();
    }
}
