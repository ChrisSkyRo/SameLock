using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsScript : MonoBehaviour
{
    [SerializeField] private Sprite completed;
    private Transform[] achievements;

    private void Awake()
    {
        achievements = new Transform[16];
        GameData gd = GameObject.Find("GameData").GetComponent<GameData>();
        for (int i = 0; i < 16; i++)
            if(gd.AchievementsUnlocked[i] == 1)
                GameObject.Find("a" + i).GetComponentInChildren<SpriteRenderer>().sprite = completed;
    }

    public void Scroll(float y)
    {
        transform.position = new Vector3(transform.position.x, y);
    }
}
