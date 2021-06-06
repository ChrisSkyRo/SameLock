using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectNavigation : MonoBehaviour
{
    public string direction;
    private SpriteRenderer sr;
    private Color[] colorsArray;
    private GameData data;
    private Transform levelSelect;
    private int steps;
    private bool nav;

    private void Awake()
    {
        nav = false;
        steps = 0;
        levelSelect = GameObject.Find("LevelSelect").GetComponent<Transform>();
        sr = GetComponent<SpriteRenderer>();
        data = GameObject.Find("GameData").GetComponent<GameData>();
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

    private void OnMouseDown()
    {
        if (!data.levelSelectAnimating)
        {
            data.levelSelectAnimating = true;
            steps = 0;
            nav = true;
        }
    }

    private void FixedUpdate()
    {
        if (!nav)
            return;

        if(direction == "left")
            levelSelect.transform.position -= Vector3.left * 2;
        else
            levelSelect.transform.position += Vector3.left * 2;

        steps++;
        if (steps == 15)
        {
            nav = false;
            data.levelSelectAnimating = false;
        }
    }

}
