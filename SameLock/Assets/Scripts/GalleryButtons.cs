using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryButtons : MonoBehaviour
{
    [SerializeField] Sprite returnButton, normalButton;
    public int button;
    private SpriteRenderer sr;
    private Color[] colorsArray;
    private GameData data;
    private Transform gallery;

    private void Awake()
    {
        gallery = GameObject.Find("ArtContainer").GetComponent<Transform>();
        data = GameObject.Find("GameData").GetComponent<GameData>();
        data.currentArt = 0;
        sr = GetComponent<SpriteRenderer>();
        colorsArray = new Color[6]
        {
            Color.red,
            Color.magenta,
            Color.yellow,
            Color.blue,
            Color.cyan,
            Color.green,
        };
        if (button == 0)
            transform.position = new Vector3(-13, 0);
    }

    private void OnMouseEnter()
    {
        sr.color = colorsArray[Random.Range(0, 6)];
    }

    private void OnMouseExit()
    {
        sr.color = Color.white;
    }

    private void OnMouseUp()
    {
        if (!data.levelSelectAnimating)
        {
            data.levelSelectAnimating = true;
            if (button == 0)
                StartCoroutine(MoveLeft());
            else
            {
                if (GetComponent<SpriteRenderer>().sprite == returnButton)
                {
                    StartCoroutine(MoveBeginning());
                    if (data.AchievementsUnlocked[12] == 0)
                    {
                        PlayerPrefs.SetInt("Achievement12", 1);
                        PlayerPrefs.Save();
                        data.AchievementsUnlocked[12] = 1;
                    }
                }
                else StartCoroutine(MoveRight());
            }
        }
    }

    IEnumerator MoveLeft()
    {
        data.currentArt--;
        if (data.currentArt == 0)
            transform.position = new Vector3(-13, 0);
        else if (data.currentArt == 18)
            GameObject.Find("BrowseRight").GetComponent<SpriteRenderer>().sprite = normalButton;
        for (int i = 20; i > 0; i--)
        {
            gallery.transform.position -= Vector3.left;
            yield return null;
        }
        data.levelSelectAnimating = false;
    }

    IEnumerator MoveRight()
    {
        data.currentArt++;
        if (data.currentArt == 1)
            GameObject.Find("BrowseLeft").GetComponent<Transform>().transform.position = new Vector3(-8, 0, -1);
        else if (data.currentArt == 19)
            GetComponent<SpriteRenderer>().sprite = returnButton;
        for (int i = 20; i > 0; i--)
        {
            gallery.transform.position -= Vector3.right;
            yield return null;
        }
        data.levelSelectAnimating = false;
    }

    IEnumerator MoveBeginning()
    {
        data.currentArt = 0;
        GetComponent<SpriteRenderer>().sprite = normalButton;
        GameObject.Find("BrowseLeft").GetComponent<Transform>().transform.position = new Vector3(-13, 0, -1);
        for (int i = 20; i > 0; i--)
        {
            gallery.transform.position -= Vector3.right;
            yield return null;
        }
        gallery.transform.position = new Vector3(20, 0);
        for (int i = 20; i > 0; i--)
        {
            gallery.transform.position -= Vector3.right;
            yield return null;
        }
        data.levelSelectAnimating = false;
    }
}
