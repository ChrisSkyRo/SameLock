using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXScript : MonoBehaviour
{
    [SerializeField] private AudioClip[] buttonClick;
    [SerializeField] private AudioClip[] tilleClick;
    [SerializeField] private AudioClip[] levelWin;
    [SerializeField] private AudioClip[] reset;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        GameObject[] objs = GameObject.FindGameObjectsWithTag("sfx");

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(string sound)
    {
        if (GameObject.Find("GameData").GetComponent<GameData>().SFX == 1)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (sound == "menu")
                audioSource.PlayOneShot(buttonClick[Random.Range(0, 4)]);
            else if (sound == "win")
                audioSource.PlayOneShot(levelWin[Random.Range(0, 2)]);
            else if (sound == "reset")
                audioSource.PlayOneShot(reset[Random.Range(0, 3)]);
            else audioSource.PlayOneShot(tilleClick[Random.Range(0, 3)]);
        }
    }
}
