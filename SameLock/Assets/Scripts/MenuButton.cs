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
        GameObject.Find("SFXManager").GetComponent<SFXScript>().PlaySFX("menu");

        GameData gd = GameObject.Find("GameData").GetComponent<GameData>();
        if (gd.levelSelectAnimating)
            return;

        if (SceneIndex == 4)
        {
            count = true;
            Animator an = GameObject.Find("SkyRollin").GetComponent<Animator>();
            an.SetTrigger("SkyInTrigger");
            gd.levelSelectAnimating = true;
            if(gd.AchievementsUnlocked[5] == 0)
            {
                gd.AchievementsUnlocked[5] = 1;
                PlayerPrefs.SetInt("Achievement5", 1);
                PlayerPrefs.Save();
                GameObject.Find("API Handler").GetComponent<APIHandler>().UnlockMedal(62026);

                // Makes sure to unlock the achievements if they didn't unlock before
                APIHandler api = GameObject.Find("API Handler").GetComponent<APIHandler>();

                api.ngio_core.onReady(() =>
                {
                    for (int i = 0; i < 16; i++)
                        if (PlayerPrefs.GetInt("Achievement" + i) == 1)
                            api.UnlockMedal(62021 + i);
                });

            }
            if(gd.AchievementsUnlocked[15] == 0)
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
                    GameObject.Find("API Handler").GetComponent<APIHandler>().UnlockMedal(62036);
                }
            }
        }
        else if(SceneIndex == 5)
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
                GameObject.Find("API Handler").GetComponent<APIHandler>().UnlockMedal(62025);
            }
        }
        else SceneManager.LoadScene(SceneIndex);
    }
}
