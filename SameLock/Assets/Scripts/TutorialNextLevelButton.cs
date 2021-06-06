using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialNextLevelButton : MonoBehaviour
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
        
        if (gd.AchievementsUnlocked[0] == 1)
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
        SceneManager.LoadScene(0);
    }
}
