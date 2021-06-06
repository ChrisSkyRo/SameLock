using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinParticlesScript : MonoBehaviour
{
    [SerializeField] private Transform star;
    [SerializeField] private Transform splash;
    [SerializeField] private Transform confetti;

    private void Start()
    {
        if (Random.value < 0.5f)
            Instantiate(confetti);
        else
        {
            Instantiate(star);
            Instantiate(splash, new Vector3(-5, -4, -6), Quaternion.identity);
            Instantiate(splash, new Vector3( 5, -4, -6), Quaternion.identity);
        }
    }

}
