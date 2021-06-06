using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScript : MonoBehaviour
{
    private Transform credits;

    private void Awake()
    {
        credits = GameObject.Find("CreditsParent").GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        credits.position += Vector3.up * 0.01f;
        if (credits.position.y > 50)
            credits.position -= Vector3.up * 57;
    }
}
