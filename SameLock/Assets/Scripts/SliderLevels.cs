using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderLevels : MonoBehaviour
{
    [SerializeField] private Transform verticalSlider;
    [SerializeField] private Transform horizontalSlider;
    [SerializeField] private Transform lastLevelGuy;

    private void Awake()
    {
        GameData gd = GameObject.Find("GameData").GetComponent<GameData>();
        if (gd.LevelToLoad > 55)
        {
            if (gd.LevelToLoad == 56 || gd.LevelToLoad == 57 || gd.LevelToLoad == 60)
                Instantiate(verticalSlider, new Vector3(8, 0, -1), Quaternion.identity, transform).eulerAngles = new Vector3(0, 0, -90);
            else if (gd.LevelToLoad == 58 || gd.LevelToLoad == 59)
                Instantiate(horizontalSlider, new Vector3(0, -4.5f, -1), Quaternion.identity, transform);
            if (gd.LevelToLoad == 60)
                Instantiate(lastLevelGuy);
        }
    }
}
