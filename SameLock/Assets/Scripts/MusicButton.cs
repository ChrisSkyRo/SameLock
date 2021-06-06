using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicButton : MonoBehaviour
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
        if (GameObject.Find("GameData").GetComponent<GameData>().Music)
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
        if (gd.Music)
        {
            gd.Music = false;
            sr.sprite = Empty;
        }
        else
        {
            gd.Music = true;
            sr.sprite = Checked;
        }
        GameObject.Find("BG Loop").GetComponent<AudioSource>().mute = !GameObject.Find("BG Loop").GetComponent<AudioSource>().mute;
        PlayerPrefs.Save();
    }
}
