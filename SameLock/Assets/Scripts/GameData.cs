using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public int TileStyle;
    public int[] LevelsCompleted;
    public int LevelToLoad, currentArt;
    public bool levelSelectAnimating;
    public int[] AchievementsUnlocked;
    public int[] Last4CompletedLevels;
    public bool Music;

    /*
        === Normal Achievements ===
        1.  Educated fellow - Complete the tutorial 			  
        2.  Easy as 1, 2, 3 - Complete the first 3 levels		
        3.  Show some love - Check out the Developer			   
        4.  Lights, camera, production - Check out the Production
        5.  Art enthusiast - Check out the Gallery			     
        6.  Casual achiever - Check out the Achievements		 
        7.  Lift off - Complete all the Easy levels			     
        8.  Rising - Complete all the Medium levels			        
        9.  To the top - Complete all Hard levels			        
        10. Extra, extra! - Complete all the Extras levels		    
        11. Completionist - Complete all the levels		        	

        === Secret Achievements ===
        12. Simplicity - Try the tile style 2#								                            
        13. Art connoisseur - Reach the end of the gallery then go back to the first piece of artwork	
        14. leet - Complete level 1, level 3 twice and level 7, in this order				            
        15. Accelerated learning - Complete the Tutorial in 3 seconds or less				            
        16. Obsession - Unlock all the achievements... I'm impressed					                
     */

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameData");
        LevelsCompleted = new int[60];
        AchievementsUnlocked = new int[16];
        Last4CompletedLevels = new int[4];
        LevelToLoad = 0;
        currentArt = 0;
        levelSelectAnimating = false;
        Music = true;

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        for (int i = 1; i <= 60; i++)
        {
           if(!PlayerPrefs.HasKey("Level" + i + "Completed"))
                PlayerPrefs.SetInt("Level" + i + "Completed", 0);
            LevelsCompleted[i - 1] = PlayerPrefs.GetInt("Level" + i + "Completed");
        }

        for (int i = 0; i < 16; i++)
        {
            if (!PlayerPrefs.HasKey("Achievement" + i))
                PlayerPrefs.SetInt("Achievement" + i, 0);
            AchievementsUnlocked[i] = PlayerPrefs.GetInt("Achievement" + i);
        }

        if (!PlayerPrefs.HasKey("TileStyle"))
            PlayerPrefs.SetInt("TileStyle", 1);
        TileStyle = PlayerPrefs.GetInt("TileStyle");
        PlayerPrefs.Save();
    }

}
