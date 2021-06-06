using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;

public class HyperlinkScript : MonoBehaviour
{

    public int credit;
    private TextMeshPro text;
    private Color[] colorsArray;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
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
        text.color = colorsArray[Random.Range(0, 6)];
    }

    private void OnMouseExit()
    {
        text.color = Color.white;
    }

    private void OnMouseDown()
    {
        if (credit == 0)
            OpenDeveloper();
        else if (credit == 1)
            OpenProduction();
        else OpenCredits();
    }

    private void OpenProduction()
    {
        GameData gd = GameObject.Find("GameData").GetComponent<GameData>();
        if(gd.AchievementsUnlocked[3] == 0)
        {
            gd.AchievementsUnlocked[3] = 1;
            PlayerPrefs.SetInt("Achievement3", 1);
            PlayerPrefs.Save();
        }
        if(Application.isMobilePlatform)
            Application.OpenURL("https://locklegion.com/");
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            openWindow("https://locklegion.com/");
        else Application.OpenURL("https://locklegion.com/");
    }

    private void OpenDeveloper()
    {
        GameData gd = GameObject.Find("GameData").GetComponent<GameData>();
        if (gd.AchievementsUnlocked[2] == 0)
        {
            gd.AchievementsUnlocked[2] = 1;
            PlayerPrefs.SetInt("Achievement2", 1);
            PlayerPrefs.Save();
        }
        if (Application.isMobilePlatform)
            Application.OpenURL("https://twitter.com/ChrisSkyRo");
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            openWindow("https://twitter.com/ChrisSkyRo");
        else Application.OpenURL("https://twitter.com/ChrisSkyRo");
    }

    private void OpenCredits()
    {
        string[] links = new string[]
        {
            "nothing",
            "here",
            "https://twitter.com/ChrisSkyRo",
            "https://lock-legion.newgrounds.com",
            "https://auberginelock.newgrounds.com",
            "https://chris-the-stick.newgrounds.com",
            "https://wonchop.newgrounds.com",
            "https://cocoalock.newgrounds.com",
            "https://krebskopf.newgrounds.com",
            "https://courgetteclock.newgrounds.com",
            "https://rn86.newgrounds.com",
            "https://staintocton.newgrounds.com",
            "https://thekursedone.newgrounds.com",
            "https://jujubelock.newgrounds.com",
            "https://leaflock.newgrounds.com",
            "https://shaka-zulu.newgrounds.com",
            "https://magicwandlock.newgrounds.com",
            "https://mp3-lock.newgrounds.com",
            "https://savvyoverdog.newgrounds.com",
            "https://nukalock.newgrounds.com",
            "https://twistedgrim.newgrounds.com",
            "https://irregularcharlie.newgrounds.com",
            "https://rho-viii.newgrounds.com",
            "https://psi43.newgrounds.com",
            "https://totghelyr.newgrounds.com",
            "https://lavagasm.newgrounds.com",
            "https://skor.newgrounds.com",
            "https://slimelock696.newgrounds.com",
            "https://uglyslug.newgrounds.com",
            "https://sir-davey.newgrounds.com",
            "https://madanimation.newgrounds.com",
            "https://yarn.newgrounds.com",
            "https://chadsweb.newgrounds.com",
            "https://twitter.com/iamKotoriSan",
            "https://lock-legion.newgrounds.com"
        };
        if (Application.isMobilePlatform)
            Application.OpenURL(links[credit]);
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            openWindow(links[credit]);
        else Application.OpenURL(links[credit]);
    }

    [DllImport("__Internal")]
    private static extern void openWindow(string url);
}
