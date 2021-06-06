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

    private void Awake()
    {
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
            if (direction == "left")
                StartCoroutine(MoveLeft());
            else StartCoroutine(MoveRight());
        }
    }

    IEnumerator MoveLeft()
    {
        for(int i = 15;i > 0; i--)
        {
            levelSelect.transform.position -= Vector3.left * 2;
            yield return null;
        }
        data.levelSelectAnimating = false;
    }

    IEnumerator MoveRight()
    {
        for (int i = 15; i > 0; i--)
        {
            levelSelect.transform.position -= Vector3.right * 2;
            yield return null;
        }
        data.levelSelectAnimating = false;
    }
}
