using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTileStyle : MonoBehaviour
{
    public int value;
    private TextMeshPro text;
    private Color[] colorsArray;
    private int tileStyle;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
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
        tileStyle = PlayerPrefs.GetInt("TileStyle");
        PlayerPrefs.Save();
        if (tileStyle == value)
            text.color = Color.white;
        else text.color = Color.black;
    }

    private void OnMouseEnter()
    {
        text.color = colorsArray[Random.Range(0, 6)];
    }

    private void OnMouseExit()
    {
        if (tileStyle == value)
            text.color = Color.white;
        else text.color = Color.black;
    }

    private void OnMouseUp()
    {
        GameData gd = GameObject.Find("GameData").GetComponent<GameData>();
        if (gd.AchievementsUnlocked[11] == 0 && value == 0)
        {
            gd.AchievementsUnlocked[11] = 1;
            PlayerPrefs.SetInt("Achievement11", 1);
        }
        gd.TileStyle = value;
        PlayerPrefs.SetInt("TileStyle", value);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
