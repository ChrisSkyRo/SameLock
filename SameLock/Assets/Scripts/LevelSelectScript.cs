using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectScript : MonoBehaviour
{
    [SerializeField] private Transform levelSelectButton;
    private GameData data;
    private Transform ls;

    private void Awake()
    {
        data = GameObject.Find("GameData").GetComponent<GameData>();
        ls = GameObject.Find("LevelSelect").GetComponent<Transform>();
    }

    private void Start()
    {
        BuildSelectionMenu();
    }

    private void BuildSelectionMenu()
    {
        int level = 1;
        for (float y = 2; y >= -3; y -= 2.5f)
            for (float x = -5; x <= 5; x += 2.5f)
            {
                Transform selectButton = Instantiate(levelSelectButton, new Vector3(x, y), Quaternion.identity, ls);
                LevelSelectButton lsb = selectButton.GetComponent<LevelSelectButton>();
                lsb.Setup(level);
                level++;
            }
        for (float y = 2; y >= -3; y -= 2.5f)
            for (float x = -5 + 30; x <= 5 + 30; x += 2.5f)
            {
                Transform selectButton = Instantiate(levelSelectButton, new Vector3(x, y), Quaternion.identity, ls);
                LevelSelectButton lsb = selectButton.GetComponent<LevelSelectButton>();
                lsb.Setup(level);
                level++;
            }
        for (float y = 2; y >= -3; y -= 2.5f)
            for (float x = -5 + 60; x <= 5 + 60; x += 2.5f)
            {
                Transform selectButton = Instantiate(levelSelectButton, new Vector3(x, y), Quaternion.identity, ls);
                LevelSelectButton lsb = selectButton.GetComponent<LevelSelectButton>();
                lsb.Setup(level);
                level++;
            }
        for (float y = 2; y >= -3; y -= 2.5f)
            for (float x = -5 + 90; x <= 5 + 90; x += 2.5f)
            {
                Transform selectButton = Instantiate(levelSelectButton, new Vector3(x, y), Quaternion.identity, ls);
                LevelSelectButton lsb = selectButton.GetComponent<LevelSelectButton>();
                lsb.Setup(level);
                level++;
            }
    }

}
