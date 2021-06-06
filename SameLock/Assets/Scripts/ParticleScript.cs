using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    [SerializeField] private float framerate;
    [SerializeField] private Sprite[] particleSprites;
    private float timer;
    private int currentSprite, frames;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        timer = framerate;
        currentSprite = 0;
        sr.sprite = particleSprites[currentSprite];
        frames = particleSprites.Length;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            timer = framerate;
            currentSprite = (currentSprite+1)%frames;
            if (currentSprite == 0)
                Destroy(gameObject);
            sr.sprite = particleSprites[currentSprite];
        }
    }
}
