using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelButton : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color[] colorsArray;
    private bool win;

    private void Awake()
    {
        win = false;
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
        transform.position -= new Vector3(0, 3);
    }

    private void FixedUpdate()
    {
        if (win)
            return;

        GameData gd = GameObject.Find("GameData").GetComponent<GameData>();

        if (gd.LevelsCompleted[gd.LevelToLoad-1] == 1)
        {
            win = true;
            transform.position += new Vector3(0, 3);
        }
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
        if (gd.LevelToLoad == 60)
            SceneManager.LoadScene(6);
        else if (gd.LevelToLoad == 15 || gd.LevelToLoad == 30 || gd.LevelToLoad == 45)
            SceneManager.LoadScene(2);
        else {
            gd.LevelToLoad++;
            SceneManager.LoadScene(3);
        }
    }
}
