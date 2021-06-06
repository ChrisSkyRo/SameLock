using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMusic : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Slider>().value = GameObject.Find("BG Loop").GetComponent<AudioSource>().volume;
    }

    public void SetVolume(float value)
    {
        GameObject.Find("BG Loop").GetComponent<AudioSource>().volume = value;
    }
}