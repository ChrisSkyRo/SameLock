using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeleteDataScript : MonoBehaviour
{
    public int button;
    private Color[] colorsArray;

    private void Awake()
    {
        colorsArray = new Color[6]
        {
            Color.red,
            Color.magenta,
            Color.yellow,
            Color.blue,
            Color.cyan,
            Color.green,
        };
    }

    private void OnMouseEnter()
    {
        if (button == 0)
            GetComponent<SpriteRenderer>().color = colorsArray[Random.Range(0, 6)];
        else GetComponent<TextMeshPro>().color = colorsArray[Random.Range(0, 6)];
    }

    private void OnMouseExit()
    {
        if (button == 0)
            GetComponent<SpriteRenderer>().color = Color.white;
        else GetComponent<TextMeshPro>().color = Color.white;
    }

    private void OnMouseUp()
    {
        if (button == 0)
            GameObject.Find("Main Camera").GetComponent<Transform>().position = new Vector3(0, 1337, -10);
        else if(button == 1)
            GameObject.Find("Main Camera").GetComponent<Transform>().position = new Vector3(0, 0, -10);
        else
        {
            Destroy(GameObject.Find("GameData"));
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(0);
        }
    }

}
