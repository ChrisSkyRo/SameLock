using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSlider : MonoBehaviour
{
    public int slider;
    private Transform gameboard;
    
    private void Awake()
    {
        gameboard = GameObject.Find("GameBoard").GetComponent<Transform>();
        GameData gd = GameObject.Find("GameData").GetComponent<GameData>();
        if(gd.LevelToLoad == 56)
            GetComponent<Slider>().maxValue = 7.5f;
        else if(gd.LevelToLoad == 57)
            GetComponent<Slider>().maxValue = 22.5f;
        else GetComponent<Slider>().maxValue = 15.5f;
    }

    public void Scroll(float val)
    {
        // vertical
        if (slider == 0)
            gameboard.position = new Vector3(gameboard.position.x, val, gameboard.position.z);
        // horizontal
        else gameboard.position = new Vector3(-val, gameboard.position.y, gameboard.position.z);
    }
}
