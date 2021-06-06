using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtworkLockScript : MonoBehaviour
{
    public int index;

    private void Start()
    {
        if (GameObject.Find("GameData").GetComponent<GameData>().AchievementsUnlocked[index] == 1)
            Destroy(gameObject);
    }
}
