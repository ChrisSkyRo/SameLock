using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectButton : MonoBehaviour
{
    private bool unlocked;
    private int level;

    public void Setup(int level)
    {
        this.level = level;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        SpriteRenderer src = transform.Find("BG").GetComponent<SpriteRenderer>();
        unlocked = IsLevelUnlocked(level);
        if (GameObject.Find("GameData").GetComponent<GameData>().LevelsCompleted[level - 1] == 1)
        {
            TextMeshPro text = GetComponentInChildren<TextMeshPro>();
            text.text = level.ToString();
            sr.color = Color.blue;
            src.color = Color.blue;
        }
        else if (!unlocked)
        {
            foreach (Transform child in gameObject.transform)
            {
                Destroy(child.gameObject);
            }
            sr.color = Color.red;
            src.color = Color.red;
        }
        else
        {
            TextMeshPro text = GetComponentInChildren<TextMeshPro>();
            text.text = level.ToString();
            sr.color = Color.yellow;
            src.color = Color.yellow;
        }
    }

    private bool IsLevelUnlocked(int level)
    {
        GameData data = GameObject.Find("GameData").GetComponent<GameData>();
        int unlockedLevels = 0;
        
        if (level < 16)
        {
            for (int i = 1; i <= 15 && unlockedLevels < 3; i++)
                if (i == level)
                    return true;
                else if (data.LevelsCompleted[i - 1] == 0)
                    unlockedLevels++;
        }
        else if (level < 31)
        {
            for (int i = 16; i <= 30 && unlockedLevels < 3; i++)
                if (i == level)
                    return true;
                else if (data.LevelsCompleted[i - 1] == 0)
                    unlockedLevels++;
        }
        else if (level < 46)
        {
            for (int i = 31; i <= 45 && unlockedLevels < 3; i++)
                if (i == level)
                    return true;
                else if (data.LevelsCompleted[i - 1] == 0)
                    unlockedLevels++;
        }
        else
        {
            for (int i = 46; i <= 60 && unlockedLevels < 3; i++)
                if (i == level)
                    return true;
                else if (data.LevelsCompleted[i - 1] == 0)
                    unlockedLevels++;
        }

        return false;
    }

    private void OnMouseEnter()
    {
        if (unlocked)
        {
            TextMeshPro text = GetComponentInChildren<TextMeshPro>();
            text.color = Color.white;
        }
    }

    private void OnMouseExit()
    {
        if (unlocked)
        {
            TextMeshPro text = GetComponentInChildren<TextMeshPro>();
            text.color = Color.black;
        }
    }

    private void OnMouseDown()
    {
        if (unlocked && !GameObject.Find("GameData").GetComponent<GameData>().levelSelectAnimating)
        {
            GameObject.Find("GameData").GetComponent<GameData>().LevelToLoad = level;
            SceneManager.LoadScene(3);
        }
    }
}
