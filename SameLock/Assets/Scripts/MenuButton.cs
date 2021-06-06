using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private int SceneIndex;
    private TextMeshPro text;
    private SpriteRenderer sr;
    private Color[] colorsArray;
    private bool count;
    private float timer;

    private void Awake()
    {
        count = false;
        timer = 1.1f;
        text = GetComponent<TextMeshPro>();
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

    private void Update()
    {
        if (!count)
            return;

        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            count = false;
            GameObject.Find("GameData").GetComponent<GameData>().levelSelectAnimating = false;
            SceneManager.LoadScene(SceneIndex);
        }
    }

    private void OnMouseEnter()
    {
        if (text)
            text.color = colorsArray[Random.Range(0, 6)];
        else sr.color = colorsArray[Random.Range(0, 6)];
    }

    private void OnMouseExit()
    {
        if (text)
            text.color = Color.white;
        else sr.color = Color.white;
    }

    private void OnMouseUp()
    {
        GameData gd = GameObject.Find("GameData").GetComponent<GameData>();
        if (gd.levelSelectAnimating)
            return;

        if (SceneIndex == 4)
        {
            count = true;
            Animator an = GameObject.Find("SkyRollin").GetComponent<Animator>();
            an.SetTrigger("SkyInTrigger");
            gd.levelSelectAnimating = true;
            if (gd.AchievementsUnlocked[5] == 0)
            {
                gd.AchievementsUnlocked[5] = 1;
                PlayerPrefs.SetInt("Achievement5", 1);
                PlayerPrefs.Save();

            }
            if (gd.AchievementsUnlocked[15] == 0)
            {
                bool ok = true;
                for (int i = 0; i < 15 && ok; i++)
                    if (gd.AchievementsUnlocked[i] == 0)
                        ok = false;
                if (ok)
                {
                    gd.AchievementsUnlocked[15] = 1;
                    PlayerPrefs.SetInt("Achievement15", 1);
                    PlayerPrefs.Save();
                }
            }
        }
        else if (SceneIndex == 5)
        {
            count = true;
            Animator an = GameObject.Find("BB10Rollin").GetComponent<Animator>();
            an.SetTrigger("BB10InTrigger");
            gd.levelSelectAnimating = true;
            if (gd.AchievementsUnlocked[4] == 0)
            {
                gd.AchievementsUnlocked[4] = 1;
                PlayerPrefs.SetInt("Achievement4", 1);
                PlayerPrefs.Save();
            }
        }
        else
        {
            if (SceneIndex == 1)
                gd.LevelToLoad = -1;
            SceneManager.LoadScene(SceneIndex);
        }
    }
}
