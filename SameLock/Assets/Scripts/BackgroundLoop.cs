using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    [SerializeField] private AudioClip[] song;
    private int current = 0;

    private void Update()
    {
        AudioSource AS = GetComponent<AudioSource>();
        if (!AS.isPlaying)
        {
            current = (current + 1) % 3;
            AS.PlayOneShot(song[current]);
        }

    }
}
