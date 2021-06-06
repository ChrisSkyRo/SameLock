using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color[] colorsArray;
    private Transform cam;
    private bool open;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        cam = GameObject.Find("Main Camera").GetComponent<Transform>();
        open = false;
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
    }

    private void OnMouseExit()
    {
        sr.color = Color.white;
    }

    private void OnMouseUp()
    {
        GameObject.Find("SFXManager").GetComponent<SFXScript>().PlaySFX("menu");
        if (open)
        {
            cam.position -= new Vector3(1337, 0);
            transform.position -= new Vector3(1337, 0);
        }
        else
        {
            cam.position += new Vector3(1337, 0);
            transform.position += new Vector3(1337, 0);
        }
        open = !open;
    }
}
