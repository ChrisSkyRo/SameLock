using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gameplay : MonoBehaviour
{
    // Game tile specifics
    [SerializeField] private Sprite[] tileSprite;
    [SerializeField] private Transform gameTile;
    private float cornerX, cornerY, tileSize;   // The upper left corner of the level and the size of the game tile

    // Level specifics
    [SerializeField] private Transform win;
    private Color[] colorsArray;                // Array composed of RGB colors
    private int[,] levelMatrix;                 // The colors that make the level
    private int levelWidth, levelHeight;
    private bool levelWon;

    // Level properties that are subject to constant change
    [System.NonSerialized] public Transform[,] tiles;       // The gameobjects that composes the level
    private bool AnimatingGameBoard;                        // Whether the gameboard is animating or expecting user input
    private int currentX, currentY;                         // Mouse position on gameboard
    [System.NonSerialized] public int falls;                // Number of falling animations playing
    [System.NonSerialized] public int slides;               // Number of sliding animations playing
    private int moves;                                      // The number of moves made (used for some Extras levels)
    private float timer;                                    // Timer used for Accelerated Learning achievement

    private void Awake()
    {
        timer = 3f;
        levelWon = false;
        AnimatingGameBoard = false;
        currentX = currentY = -1;
        falls = 0;
        slides = 0;
        moves = 0;
        if (GameObject.Find("GameData").GetComponent<GameData>().LevelToLoad == -1)
        {
            TextMeshPro text = GameObject.Find("Instructions").GetComponent<TextMeshPro>();
            text.alpha = 0;
            text = GameObject.Find("TextSettings").GetComponent<TextMeshPro>();
            text.alpha = 0;
            text = GameObject.Find("TextReset").GetComponent<TextMeshPro>();
            text.alpha = 0;
        }
    }

    private void Start()
    {
        // Tutorial instructions
        if (GameObject.Find("GameData").GetComponent<GameData>().LevelToLoad == -1)
            StartCoroutine(TextFadeIn());
        // Initialize the colors array
        InitializeColors();
        // Set the color of the game tiles in the matrix
        InitializeMatrix();
        // Creates the game board based on the colors in the level matrix
        BuildLevel();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (!AnimatingGameBoard)
        {
            if (!levelWon && levelMatrix[levelHeight - 1, 0] == 0)
            {
                levelWon = true;
                Instantiate(win, new Vector3(0, 0, -5), Quaternion.identity);
                GameData gd = GameObject.Find("GameData").GetComponent<GameData>();

                if(gd.LevelToLoad == -1)
                {
                    Destroy(GameObject.Find("Instructions"));
                    GameObject.Find("SettingsButton").transform.GetChild(0).position += new Vector3(100, 0);
                    GameObject.Find("ResetButton").transform.GetChild(0).position += new Vector3(100, 0);
                    gd.AchievementsUnlocked[0] = 1;
                    PlayerPrefs.SetInt("Achievement0", 1);
                    if (gd.AchievementsUnlocked[14] == 0 && timer > 0)
                    {
                        gd.AchievementsUnlocked[14] = 1;
                        PlayerPrefs.SetInt("Achievement14", 1);
                    }
                    PlayerPrefs.Save();

                    return;
                }

                gd.LevelsCompleted[gd.LevelToLoad - 1] = 1;

                if (gd.AchievementsUnlocked[1] == 0)
                {
                    // Easy as 1, 2, 3 achievement
                    if (gd.LevelToLoad == 1 || gd.LevelToLoad == 2 || gd.LevelToLoad == 3)
                    {
                        if (gd.LevelsCompleted[0] == gd.LevelsCompleted[1] && gd.LevelsCompleted[1] == gd.LevelsCompleted[2])
                        {
                            gd.AchievementsUnlocked[1] = 1;
                            PlayerPrefs.SetInt("Achievement1", 1);
                        }
                    }
                }
                // Lift off achievement
                if (gd.LevelToLoad <= 15 && gd.AchievementsUnlocked[6] == 0)
                {
                    bool ok = true;
                    for (int i = 0; i < 15 && ok; i++)
                        if (gd.LevelsCompleted[i] == 0)
                            ok = false;
                    if (ok)
                    {
                        gd.AchievementsUnlocked[6] = 1;
                        PlayerPrefs.SetInt("Achievement6", 1);
                    }
                }
                // Rising achievement
                else if (gd.LevelToLoad <= 30 && gd.AchievementsUnlocked[7] == 0)
                {
                    bool ok = true;
                    for (int i = 15; i < 30 && ok; i++)
                        if (gd.LevelsCompleted[i] == 0)
                            ok = false;
                    if (ok)
                    {
                        gd.AchievementsUnlocked[7] = 1;
                        PlayerPrefs.SetInt("Achievement7", 1);
                    }
                }
                // To the top achievement
                else if (gd.LevelToLoad <= 45 && gd.AchievementsUnlocked[8] == 0)
                {
                    bool ok = true;
                    for (int i = 30; i < 45 && ok; i++)
                        if (gd.LevelsCompleted[i] == 0)
                            ok = false;
                    if (ok)
                    {
                        gd.AchievementsUnlocked[8] = 1;
                        PlayerPrefs.SetInt("Achievement8", 1);
                    }
                }
                // Extra, extra! achievement
                else if (gd.AchievementsUnlocked[9] == 0)
                {
                    bool ok = true;
                    for (int i = 45; i < 60 && ok; i++)
                        if (gd.LevelsCompleted[i] == 0)
                            ok = false;
                    if (ok)
                    {
                        gd.AchievementsUnlocked[9] = 1;
                        PlayerPrefs.SetInt("Achievement9", 1);
                    }
                }
                // Completionist achievement
                if(gd.AchievementsUnlocked[10] == 0 && gd.AchievementsUnlocked[6] == 1 && gd.AchievementsUnlocked[7] == 1 && gd.AchievementsUnlocked[8] == 1 && gd.AchievementsUnlocked[9] == 1)
                {
                    gd.AchievementsUnlocked[10] = 1;
                    PlayerPrefs.SetInt("Achievement10", 1);
                }
                // leet achievement
                if (gd.AchievementsUnlocked[13] == 0)
                {
                    for (int i = 0; i < 3; i++)
                        gd.Last4CompletedLevels[i] = gd.Last4CompletedLevels[i + 1];
                    gd.Last4CompletedLevels[3] = gd.LevelToLoad;
                    if (gd.Last4CompletedLevels[0] == 1 && gd.Last4CompletedLevels[1] == 3 && gd.Last4CompletedLevels[2] == 3 && gd.Last4CompletedLevels[3] == 7)
                    {
                        gd.AchievementsUnlocked[13] = 1;
                        PlayerPrefs.SetInt("Achievement13", 1);
                    }
                }

                PlayerPrefs.SetInt("Level" + gd.LevelToLoad + "Completed", 1);
                PlayerPrefs.Save();
            }

            bool tileHover = FindTileMouseOver();

            if (!tileHover)
                return;

            if (!Application.isMobilePlatform)
                HighlightTiles(currentX, currentY);

            if (Input.GetMouseButtonDown(0) && CanDestroyTile(currentX, currentY))
            {
                AnimatingGameBoard = true;
                moves++;
                DestroyTiles(currentX, currentY);
                AnimateGameBoard();
            }
        }
    }

    // Initialize the colors array
    private void InitializeColors()
    {
        colorsArray = new Color[9]
        {
            new Color(0, 0, 0),                             // 0 - BLACK (unused)
            new Color(255/255.0f, 0, 204/255.0f),           // 1 - PINK
            new Color(200/255.0f, 0, 0),                    // 2 - RED
            new Color(122/255.0f, 0, 178/255.0f),           // 3 - PURPLE
            new Color(0, 217/255.0f, 0),                    // 4 - GREEN
            new Color(0, 0, 200/255.0f),                    // 5 - BLUE
            new Color(255/255.0f, 118/255.0f, 0),           // 6 - ORANGE
            new Color(255/255.0f, 255/255.0f, 0),           // 7 - YELLOW
            new Color(101/255.0f, 67/255.0f, 33/255.0f),    // 8 - BROWN
        };

        // Randomize colors array and sprite array
        GameData gd = GameObject.Find("GameData").GetComponent<GameData>();
        if (gd.LevelToLoad > 30 && gd.LevelToLoad <= 45)
        {
            for (int i = 1; i < 9; i++)
            {
                int j = Random.Range(1, 9);
                Color temp1 = colorsArray[j];
                colorsArray[j] = colorsArray[i];
                colorsArray[i] = temp1;
                Sprite temp2 = tileSprite[j];
                tileSprite[j] = tileSprite[i];
                tileSprite[i] = temp2;
            }
        }
    }

    // Level designer
    private void InitializeMatrix()
    {
        
        GameData gd = GameObject.Find("GameData").GetComponent<GameData>();
        if(gd.LevelToLoad == -1)
        {

            levelMatrix = new int[,]
            {
                { 1, 2, 2},
                { 1, 3, 3},
                { 2, 1, 1}
            };
            cornerX = -2;
            cornerY = 3.5f;
            tileSize = 2;
            levelHeight = 3;
            levelWidth = 3;
        }
        else if (gd.LevelToLoad == 1)
        {
            levelMatrix = new int[,]
            {
                { 1, 3, 2},
                { 1, 3, 1},
                { 2, 2, 1}
            };
            cornerX = -2;
            cornerY = 3.5f;
            tileSize = 2;
            levelHeight = 3;
            levelWidth = 3;
        }
        else if(gd.LevelToLoad == 2)
        {
            levelMatrix = new int[,]
            {
                { 1, 1, 3},
                { 3, 3, 2},
                { 1, 1, 2}
            };
            cornerX = -2;
            cornerY = 3.5f;
            tileSize = 2;
            levelHeight = 3;
            levelWidth = 3;
        }
        else if (gd.LevelToLoad == 3)
        {
            levelMatrix = new int[,]
            {
                { 1, 1, 2, 1},
                { 2, 3, 2, 1},
                { 2, 1, 1, 3}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 2;
            levelHeight = 3;
            levelWidth = 4;
        }
        else if (gd.LevelToLoad == 4)
        {
            levelMatrix = new int[,]
            {
                { 1, 1, 2, 1},
                { 3, 2, 3, 1},
                { 1, 1, 2, 2}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 2;
            levelHeight = 3;
            levelWidth = 4;
        }
        else if (gd.LevelToLoad == 5)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 1},
                { 1, 3, 1},
                { 3, 1, 2},
                { 1, 3, 3},
            };
            cornerX = -2f;
            cornerY = 3.5f;
            tileSize = 2;
            levelHeight = 4;
            levelWidth = 3;
        }
        else if (gd.LevelToLoad == 6)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3},
                { 3, 1, 3},
                { 2, 2, 2},
                { 3, 1, 1},
            };
            cornerX = -2f;
            cornerY = 3.5f;
            tileSize = 2;
            levelHeight = 4;
            levelWidth = 3;
        }
        else if (gd.LevelToLoad == 7)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 3},
                { 1, 3, 1, 2},
                { 2, 2, 1, 3}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 2;
            levelHeight = 3;
            levelWidth = 4;
        }
        else if (gd.LevelToLoad == 8)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3},
                { 1, 2, 2},
                { 3, 1, 3},
                { 1, 2, 3},
            };
            cornerX = -2f;
            cornerY = 3.5f;
            tileSize = 2;
            levelHeight = 4;
            levelWidth = 3;
        }
        else if (gd.LevelToLoad == 9)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 2},
                { 1, 1, 2, 1},
                { 3, 1, 3, 2},
                { 3, 2, 2, 1}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 2;
            levelHeight = 4;
            levelWidth = 4;
        }
        else if (gd.LevelToLoad == 10)
        {
            levelMatrix = new int[,]
            {
                { 1, 1, 1, 2},
                { 3, 2, 3, 3},
                { 1, 3, 2, 1},
                { 1, 2, 3, 3}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 2;
            levelHeight = 4;
            levelWidth = 4;
        }
        else if (gd.LevelToLoad == 11)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 1, 1},
                { 3, 1, 2, 2},
                { 3, 1, 3, 1},
                { 1, 3, 2, 1}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 2;
            levelHeight = 4;
            levelWidth = 4;
        }
        else if (gd.LevelToLoad == 12)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 3},
                { 1, 4, 4, 1},
                { 3, 1, 2, 1},
                { 4, 1, 3, 4}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 2;
            levelHeight = 4;
            levelWidth = 4;
        }
        else if (gd.LevelToLoad == 13)
        {
            levelMatrix = new int[,]
            {
                { 1, 1, 2, 1},
                { 3, 4, 5, 4},
                { 1, 4, 5, 4},
                { 1, 3, 2, 1}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 2;
            levelHeight = 4;
            levelWidth = 4;
        }
        else if (gd.LevelToLoad == 14)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 1, 2, 2},
                { 1, 3, 2, 2, 3},
                { 2, 3, 1, 1, 3},
                { 2, 1, 2, 3, 1},
                { 3, 3, 2, 3, 1}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 15)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 1, 2},
                { 1, 2, 2, 3, 1},
                { 3, 1, 2, 1, 3},
                { 3, 3, 1, 3, 2},
                { 1, 2, 2, 1, 3}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 16)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 2, 3, 2},
                { 1, 3, 1, 3, 1},
                { 2, 1, 2, 1, 1},
                { 2, 3, 1, 3, 2},
                { 1, 1, 2, 3, 1}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 17)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 1, 3, 1},
                { 2, 3, 2, 2, 1},
                { 2, 2, 1, 3, 2},
                { 1, 2, 3, 2, 3},
                { 3, 1, 1, 3, 2}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 18)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 2, 3, 1},
                { 1, 3, 1, 2, 1},
                { 3, 2, 1, 2, 3},
                { 1, 2, 3, 1, 2},
                { 1, 3, 1, 3, 2}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 19)
        {
            levelMatrix = new int[,]
            {
                { 1, 1, 2, 3, 3},
                { 3, 4, 3, 2, 2},
                { 3, 4, 1, 3, 3},
                { 2, 2, 1, 4, 2},
                { 1, 1, 3, 4, 2}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 20)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 1, 3, 2},
                { 3, 4, 1, 3, 2},
                { 3, 2, 2, 1, 3},
                { 1, 4, 3, 2, 2},
                { 3, 2, 1, 4, 4}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 21)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 4, 4},
                { 1, 2, 3, 3, 2},
                { 4, 3, 4, 2, 4},
                { 2, 3, 1, 2, 4},
                { 2, 4, 1, 3, 2}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 22)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 2, 4},
                { 4, 3, 1, 4, 4},
                { 4, 2, 3, 4, 1},
                { 3, 4, 3, 1, 2},
                { 3, 4, 2, 2, 1}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 23)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 1, 2, 3},
                { 3, 2, 4, 2, 3},
                { 1, 1, 3, 1, 2},
                { 2, 3, 2, 2, 1},
                { 3, 1, 1, 4, 1}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 24)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 1, 3, 1},
                { 2, 2, 4, 2, 3},
                { 1, 4, 2, 2, 3},
                { 1, 2, 4, 3, 4},
                { 3, 3, 4, 2, 4}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 25)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 4, 4},
                { 1, 4, 1, 1, 2},
                { 4, 1, 2, 2, 3},
                { 4, 1, 3, 4, 1},
                { 2, 4, 3, 4, 1}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 26)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 1, 3, 4},
                { 1, 1, 4, 3, 4},
                { 2, 2, 4, 1, 2},
                { 4, 2, 3, 4, 1},
                { 4, 1, 1, 3, 4}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 27)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 2, 1, 3},
                { 4, 4, 2, 1, 4},
                { 3, 2, 3, 2, 3},
                { 4, 4, 1, 2, 4},
                { 1, 1, 4, 3, 4}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 28)
        {
            levelMatrix = new int[,]
            {
                { 1, 1, 2, 1, 3},
                { 2, 4, 4, 4, 3},
                { 4, 1, 3, 3, 4},
                { 3, 2, 1, 4, 3},
                { 4, 4, 1, 2, 2}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 29)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 4, 2},
                { 1, 2, 3, 4, 1},
                { 4, 4, 4, 2, 3},
                { 2, 2, 3, 4, 3},
                { 1, 1, 3, 4, 1}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 30)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 4, 2},
                { 4, 1, 4, 1, 2},
                { 1, 2, 2, 3, 1},
                { 4, 3, 2, 1, 3},
                { 4, 1, 3, 3, 1}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 31)
        {
            levelMatrix = new int[,]
            {
                { 1, 1, 2, 3, 1},
                { 3, 4, 5, 2, 5},
                { 3, 2, 5, 1, 5},
                { 5, 2, 4, 1, 5},
                { 5, 1, 1, 4, 4}
            };
            cornerX = -3f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 32)
        {
            levelMatrix = new int[,]
            {
                { 1, 1, 2, 1, 3, 1},
                { 2, 3, 1, 3, 2, 1},
                { 3, 1, 2, 1, 3, 3},
                { 3, 3, 2, 3, 2, 1},
                { 1, 3, 1, 3, 2, 1},
                { 3, 1, 2, 2, 1, 2},
            };
            cornerX = -3.375f;
            cornerY = 3.75f;
            tileSize = 1.35f;
            levelHeight = 6;
            levelWidth = 6;
        }
        else if (gd.LevelToLoad == 33)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 1, 2, 3},
                { 1, 2, 3, 1, 2, 3},
                { 3, 1, 1, 3, 1, 1},
                { 1, 2, 3, 2, 2, 3},
                { 3, 1, 3, 1, 2, 1},
                { 2, 3, 3, 1, 3, 1},
            };
            cornerX = -3.375f;
            cornerY = 3.75f;
            tileSize = 1.35f;
            levelHeight = 6;
            levelWidth = 6;
        }
        else if (gd.LevelToLoad == 34)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 1, 2, 1},
                { 1, 1, 2, 3, 2, 3},
                { 3, 1, 3, 1, 3, 1},
                { 3, 3, 2, 3, 2, 3},
                { 2, 1, 3, 1, 2, 3},
                { 1, 2, 2, 3, 3, 1},
            };
            cornerX = -3.375f;
            cornerY = 3.75f;
            tileSize = 1.35f;
            levelHeight = 6;
            levelWidth = 6;
        }
        else if (gd.LevelToLoad == 35)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 2, 2, 4},
                { 1, 2, 2, 2, 3, 4},
                { 3, 1, 4, 4, 3, 1},
                { 3, 3, 4, 3, 2, 1},
                { 2, 4, 1, 3, 4, 3},
                { 2, 4, 2, 1, 1, 3},
            };
            cornerX = -3.375f;
            cornerY = 3.75f;
            tileSize = 1.35f;
            levelHeight = 6;
            levelWidth = 6;
        }
        else if (gd.LevelToLoad == 36)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 2, 3, 4, 3},
                { 4, 3, 3, 3, 3, 2},
                { 3, 2, 2, 1, 4, 2},
                { 3, 4, 3, 1, 4, 1},
                { 1, 3, 4, 3, 2, 4},
                { 2, 2, 4, 1, 2, 3},
            };
            cornerX = -3.375f;
            cornerY = 3.75f;
            tileSize = 1.35f;
            levelHeight = 6;
            levelWidth = 6;
        }
        else if (gd.LevelToLoad == 37)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 3, 1, 4},
                { 4, 2, 1, 2, 1, 4},
                { 4, 4, 4, 2, 3, 2},
                { 3, 2, 3, 3, 4, 2},
                { 3, 2, 1, 1, 4, 3},
                { 2, 4, 2, 4, 1, 1},
            };
            cornerX = -3.375f;
            cornerY = 3.75f;
            tileSize = 1.35f;
            levelHeight = 6;
            levelWidth = 6;
        }
        else if (gd.LevelToLoad == 38)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 1, 1, 2, 3},
                { 4, 2, 5, 2, 1, 3},
                { 5, 1, 3, 5, 1, 5},
                { 4, 1, 3, 5, 2, 1},
                { 2, 2, 1, 3, 1, 5},
                { 4, 5, 4, 3, 2, 5},
            };
            cornerX = -3.375f;
            cornerY = 3.75f;
            tileSize = 1.35f;
            levelHeight = 6;
            levelWidth = 6;
        }
        else if (gd.LevelToLoad == 39)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 2, 2, 2, 1},
                { 1, 2, 1, 3, 4, 2, 1},
                { 3, 1, 2, 2, 4, 4, 2},
                { 4, 4, 4, 4, 3, 4, 4},
                { 3, 1, 2, 4, 1, 1, 3},
                { 1, 3, 1, 2, 3, 4, 2},
                { 3, 4, 1, 2, 1, 3, 2},
            };
            cornerX = -3.6f;
            cornerY = 4f;
            tileSize = 1.2f;
            levelHeight = 7;
            levelWidth = 7;
        }
        else if (gd.LevelToLoad == 40)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 1, 2, 1, 2, 2, 1, 1, 3},
                { 1, 2, 3, 2, 1, 2, 2, 3, 2, 3},
                { 2, 3, 3, 3, 2, 3, 3, 1, 2, 1},
                { 1, 1, 2, 1, 2, 3, 2, 1, 3, 1},
                { 2, 3, 2, 3, 1, 1, 2, 3, 2, 2}
            };
            cornerX = -7f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 10;
        }
        else if (gd.LevelToLoad == 41)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 1, 3, 2, 2, 3, 4, 3, 1},
                { 1, 2, 2, 3, 1, 4, 3, 2, 4, 2},
                { 4, 1, 1, 4, 2, 4, 2, 1, 4, 1},
                { 4, 3, 2, 3, 2, 1, 4, 1, 3, 2},
                { 1, 3, 1, 3, 4, 1, 4, 2, 2, 4}
            };
            cornerX = -7f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 10;
        }
        else if (gd.LevelToLoad == 42)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 1, 3, 1, 4, 1, 1, 4},
                { 3, 2, 1, 4, 1, 3, 1, 1, 1, 3},
                { 3, 4, 1, 4, 3, 1, 3, 4, 2, 2},
                { 4, 1, 3, 2, 2, 3, 2, 3, 4, 3},
                { 1, 2, 2, 4, 4, 1, 1, 2, 1, 1}
            };
            cornerX = -7f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 10;
        }
        else if (gd.LevelToLoad == 43)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 2, 1, 4, 5, 2, 2, 2},
                { 5, 5, 4, 5, 4, 3, 5, 2, 3, 3},
                { 1, 3, 4, 2, 4, 2, 2, 3, 2, 1},
                { 3, 5, 1, 4, 5, 4, 5, 1, 2, 1},
                { 3, 2, 1, 4, 5, 3, 5, 3, 4, 4}
            };
            cornerX = -7f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 10;
        }
        else if (gd.LevelToLoad == 44)
        {
            levelMatrix = new int[,]
            {
                { 1, 1, 2, 3, 4, 2, 3, 5, 2, 1},
                { 6, 2, 1, 5, 4, 1, 3, 4, 3, 5},
                { 6, 4, 6, 1, 5, 6, 4, 3, 4, 5},
                { 5, 4, 3, 4, 4, 2, 1, 5, 3, 4},
                { 5, 2, 5, 1, 5, 3, 1, 4, 1, 4}
            };
            cornerX = -7f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 10;
        }
        else if (gd.LevelToLoad == 45)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 1, 4, 1, 3, 3, 4, 3},
                { 3, 2, 1, 2, 2, 3, 2, 3, 4, 3},
                { 3, 3, 1, 2, 2, 3, 2, 1, 2, 2},
                { 1, 2, 4, 4, 1, 4, 4, 2, 1, 4},
                { 1, 4, 3, 2, 1, 4, 4, 2, 1, 4},
                { 2, 4, 1, 2, 4, 3, 1, 1, 2, 2},
                { 3, 1, 1, 3, 3, 1, 1, 3, 4, 1},
                { 3, 2, 4, 1, 1, 3, 3, 4, 3, 2},
                { 4, 3, 3, 3, 3, 2, 1, 4, 3, 2},
                { 4, 2, 1, 2, 2, 4, 1, 2, 4, 1},
            };
            cornerX = -3.825f;
            cornerY = 4.25f;
            tileSize = 0.85f;
            levelHeight = 10;
            levelWidth = 10;
        }
        else if(gd.LevelToLoad == 46)
        {
            levelMatrix = new int[,]
            {
                { 1},
                { 1},
                { 2},
                { 2},
                { 3},
                { 3},
                { 1},
            };
            cornerX = 0f;
            cornerY = 4f;
            tileSize = 1.2f;
            levelHeight = 7;
            levelWidth = 1;
        }
        else if (gd.LevelToLoad == 47)
        {
            levelMatrix = new int[,]
            {
                { 1},
                { 2},
                { 2},
                { 3},
                { 2},
                { 2},
                { 3},
                { 1},
                { 2},
                { 3},
                { 3},
                { 1},
                { 1},
                { 2},
                { 1},
                { 1},
                { 3},
                { 1},
                { 1},
                { 3},
            };
            cornerX = 0f;
            cornerY = 20f;
            tileSize = 1.2f;
            levelHeight = 20;
            levelWidth = 1;
        }
        else if(gd.LevelToLoad == 48)
        {
            levelMatrix = new int[,]
            {
                { 3, 2, 2, 2, 3, 1, 2, 3, 1, 2, 3, 1, 2, 1, 3, 3, 3, 2, 3, 1, 2, 1, 2, 3, 1, 2, 3, 2, 2, 3},
                { 1, 1, 3, 1, 3, 2, 2, 3, 1, 2, 3, 1, 2, 1, 3, 3, 3, 1, 1, 1, 2, 3, 2, 3, 1, 2, 3, 2, 1, 3},
                { 3, 2, 1, 3, 2, 1, 3, 2, 3, 1, 1, 2, 3, 3, 1, 1, 1, 2, 2, 2, 3, 3, 1, 2, 3, 1, 1, 3, 1, 2},
                { 3, 1, 2, 2, 2, 3, 1, 2, 3, 2, 3, 3, 1, 2, 3, 2, 3, 1, 3, 1, 1, 2, 3, 3, 1, 3, 2, 1, 3, 1},
                { 2, 3, 1, 1, 3, 1, 2, 1, 1, 2, 1, 2, 1, 2, 3, 1, 3, 2, 3, 2, 3, 2, 1, 2, 1, 3, 2, 1, 3, 1},
            };
            cornerX = -7f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 30;
        }
        else if(gd.LevelToLoad == 49)
        {

            levelMatrix = new int[,]
            {
                { 1, 3, 1, 2, 3, 1, 2, 1, 2, 1, 2, 2, 2, 2, 1, 1, 3, 2, 2, 2},
                { 1, 3, 2, 1, 3, 3, 2, 1, 2, 3, 2, 1, 2, 2, 2, 3, 3, 2, 2, 2},
                { 2, 1, 3, 1, 2, 1, 3, 3, 3, 3, 3, 2, 1, 3, 1, 3, 1, 3, 1, 3},
                { 1, 3, 3, 3, 2, 3, 2, 1, 3, 2, 1, 2, 1, 3, 2, 1, 1, 1, 2, 3},
                { 2, 1, 2, 2, 1, 1, 1, 2, 1, 3, 2, 1, 3, 1, 2, 2, 2, 1, 2, 1},
                { 2, 2, 3, 2, 1, 2, 3, 2, 1, 3, 3, 2, 1, 2, 3, 2, 3, 3, 3, 3},
                { 3, 1, 1, 1, 3, 1, 2, 1, 2, 1, 3, 1, 3, 3, 1, 1, 1, 1, 2, 3},
                { 3, 3, 3, 1, 1, 2, 3, 2, 3, 1, 2, 3, 3, 3, 3, 3, 1, 3, 2, 1},
                { 2, 3, 1, 3, 3, 3, 1, 3, 1, 2, 1, 1, 2, 1, 3, 3, 2, 3, 1, 2},
                { 2, 2, 2, 2, 1, 3, 3, 2, 1, 3, 3, 2, 1, 2, 1, 2, 3, 2, 1, 2},
                { 1, 3, 2, 3, 3, 2, 2, 3, 2, 1, 3, 2, 1, 2, 3, 1, 2, 1, 3, 3},
                { 1, 1, 1, 1, 1, 1, 3, 1, 2, 2, 1, 1, 3, 1, 2, 1, 2, 1, 2, 1},
                { 2, 3, 1, 2, 3, 2, 2, 1, 3, 1, 3, 2, 3, 2, 1, 3, 1, 2, 2, 2},
                { 2, 3, 2, 1, 1, 1, 1, 2, 2, 3, 2, 3, 1, 1, 1, 3, 2, 1, 3, 3},
                { 3, 2, 1, 2, 3, 2, 3, 3, 3, 1, 2, 1, 3, 2, 3, 2, 1, 3, 2, 1},
                { 2, 1, 3, 1, 1, 2, 2, 1, 2, 3, 3, 3, 1, 3, 1, 1, 3, 2, 1, 3},
                { 2, 3, 1, 2, 3, 1, 1, 3, 3, 3, 2, 3, 3, 2, 1, 2, 1, 3, 3, 2},
                { 3, 1, 2, 3, 1, 2, 2, 2, 1, 1, 1, 1, 3, 2, 3, 2, 1, 2, 2, 3},
                { 2, 1, 3, 2, 1, 2, 3, 1, 2, 3, 2, 3, 2, 1, 2, 3, 2, 1, 3, 2},
                { 1, 3, 1, 2, 3, 1, 3, 2, 1, 3, 1, 3, 2, 3, 1, 1, 3, 2, 3, 1},
            };
            cornerX = -3f;
            cornerY = 26f;
            tileSize = 1.5f;
            levelHeight = 20;
            levelWidth = 20;
        }
        else if(gd.LevelToLoad == 50)
        {
            levelMatrix = new int[,]
            {
                { 2, 1, 2, 2, 1, 1, 2, 2, 1, 3},
                { 1, 3, 2, 1, 3, 3, 3, 2, 1, 3},
                { 1, 3, 1, 3, 3, 2, 2, 1, 3, 2},
                { 3, 2, 2, 3, 1, 2, 1, 1, 3, 1},
                { 3, 1, 3, 2, 3, 1, 2, 3, 2, 2},
                { 2, 3, 2, 3, 2, 1, 2, 1, 1, 1},
                { 1, 3, 2, 3, 2, 3, 3, 1, 3, 2},
                { 3, 2, 3, 1, 1, 2, 1, 2, 3, 1},
                { 3, 2, 1, 2, 1, 2, 3, 2, 2, 2},
                { 1, 3, 3, 2, 3, 3, 1, 3, 1, 1},
            };
            cornerX = -3.375f;
            cornerY = 3.5f;
            tileSize = 0.75f;
            levelHeight = 10;
            levelWidth = 10;
        }
        else if (gd.LevelToLoad == 51)
        {
            levelMatrix = new int[,]
            {
                { 1, 3, 3, 1, 2, 1, 3, 3, 1, 3},
                { 2, 1, 3, 1, 3, 1, 3, 2, 1, 3},
                { 2, 1, 2, 2, 1, 3, 1, 2, 2, 2},
                { 3, 2, 1, 3, 3, 2, 2, 1, 1, 2},
                { 3, 3, 3, 2, 1, 3, 2, 1, 3, 3},
                { 1, 1, 3, 2, 3, 2, 1, 3, 3, 3},
                { 2, 2, 2, 1, 1, 2, 3, 1, 2, 2},
                { 3, 3, 1, 3, 2, 1, 2, 1, 3, 1},
                { 1, 2, 3, 1, 1, 2, 3, 3, 1, 2},
                { 1, 2, 1, 2, 3, 1, 2, 2, 1, 3},
            };
            cornerX = -3.375f;
            cornerY = 3.5f;
            tileSize = 0.75f;
            levelHeight = 10;
            levelWidth = 10;
        }
        else if (gd.LevelToLoad == 52)
        {
            levelMatrix = new int[,]
            {
                { 1, 3, 2, 1, 3, 2, 2, 3, 2, 3},
                { 2, 3, 2, 1, 2, 3, 3, 3, 2, 3},
                { 3, 1, 3, 2, 3, 1, 3, 2, 1, 2},
                { 3, 1, 3, 1, 3, 1, 1, 1, 3, 2},
                { 2, 2, 1, 2, 1, 2, 3, 3, 2, 1},
                { 1, 3, 3, 1, 3, 1, 1, 1, 1, 2},
                { 1, 3, 1, 2, 1, 2, 3, 3, 3, 1},
                { 3, 2, 1, 3, 3, 1, 2, 1, 1, 3},
                { 3, 1, 2, 2, 2, 2, 1, 2, 2, 2},
                { 1, 2, 3, 1, 1, 3, 1, 3, 1, 1},
            };
            cornerX = -3.375f;
            cornerY = 3.5f;
            tileSize = 0.75f;
            levelHeight = 10;
            levelWidth = 10;
        }
        else if (gd.LevelToLoad == 53)
        {
            levelMatrix = new int[,]
            {
                { 1, 1, 2, 3, 2, 2, 3, 3, 1, 3},
                { 1, 1, 2, 3, 2, 2, 1, 2, 1, 3},
                { 3, 2, 3, 1, 1, 3, 1, 2, 3, 1},
                { 2, 1, 2, 3, 3, 3, 2, 1, 3, 1},
                { 3, 2, 3, 1, 2, 1, 1, 1, 2, 2},
                { 3, 2, 1, 1, 1, 3, 2, 2, 3, 1},
                { 2, 3, 3, 3, 2, 1, 2, 1, 2, 1},
                { 1, 2, 2, 2, 1, 2, 3, 2, 1, 3},
                { 2, 3, 3, 3, 1, 3, 2, 3, 2, 3},
                { 1, 2, 2, 1, 2, 3, 1, 3, 1, 1},
            };
            cornerX = -3.375f;
            cornerY = 3.5f;
            tileSize = 0.75f;
            levelHeight = 10;
            levelWidth = 10;
        }
        else if (gd.LevelToLoad == 54)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 2, 3, 1, 2, 1, 2, 3, 2},
                { 3, 1, 3, 2, 3, 2, 1, 3, 1, 2},
                { 3, 1, 3, 2, 3, 3, 2, 3, 1, 1},
                { 2, 3, 1, 3, 2, 2, 3, 1, 3, 2},
                { 2, 3, 1, 2, 1, 3, 2, 1, 2, 1},
                { 3, 1, 3, 1, 2, 1, 1, 3, 3, 1},
                { 3, 1, 3, 2, 1, 2, 3, 2, 1, 2},
                { 1, 2, 1, 1, 2, 1, 3, 1, 2, 3},
                { 3, 1, 3, 2, 3, 2, 2, 2, 1, 3},
                { 1, 2, 3, 1, 2, 1, 1, 3, 2, 2},
            };
            cornerX = -3.375f;
            cornerY = 3.5f;
            tileSize = 0.75f;
            levelHeight = 10;
            levelWidth = 10;
        }
        else if (gd.LevelToLoad == 55)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3, 4, 2, 4, 2, 4, 3, 4},
                { 1, 3, 3, 3, 1, 1, 2, 4, 1, 3},
                { 2, 4, 4, 3, 2, 2, 3, 2, 1, 3},
                { 1, 1, 4, 2, 2, 1, 3, 2, 3, 2},
                { 3, 2, 1, 3, 4, 1, 4, 4, 1, 1},
                { 3, 2, 4, 3, 1, 4, 3, 4, 3, 3},
                { 2, 3, 1, 4, 4, 2, 3, 1, 2, 2},
                { 1, 1, 4, 3, 2, 4, 2, 4, 3, 2},
                { 3, 3, 2, 3, 2, 1, 1, 1, 4, 1},
                { 4, 4, 2, 1, 4, 2, 2, 4, 3, 1},
            };
            cornerX = -3.375f;
            cornerY = 3.5f;
            tileSize = 0.75f;
            levelHeight = 10;
            levelWidth = 10;
        }
        else if (gd.LevelToLoad == 56)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 1, 3, 2},
                { 1, 3, 3, 1, 2},
                { 3, 3, 3, 3, 3},
                { 3, 2, 2, 1, 3},
                { 1, 1, 3, 1, 1},
                { 3, 2, 2, 2, 1},
                { 3, 1, 3, 1, 2},
                { 2, 3, 2, 1, 3},
                { 1, 3, 3, 3, 3},
                { 2, 1, 2, 2, 1},
            };
            cornerX = -3f;
            cornerY = 3f;
            tileSize = 1.5f;
            levelHeight = 10;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 57)
        {
            levelMatrix = new int[,]
            {
                { 1, 1, 2, 1, 2,},
                { 3, 4, 1, 4, 1,},
                { 2, 1, 2, 4, 1,},
                { 2, 3, 3, 3, 3,},
                { 3, 1, 1, 3, 4,},
                { 3, 2, 3, 4, 4,},
                { 4, 2, 3, 4, 1,},
                { 4, 4, 4, 1, 3,},
                { 1, 2, 2, 1, 3,},
                { 1, 3, 3, 3, 1,},
                { 3, 4, 1, 3, 4,},
                { 1, 4, 3, 2, 4,},
                { 4, 2, 2, 4, 3,},
                { 4, 1, 1, 4, 2,},
                { 1, 3, 3, 2, 1,},
                { 4, 1, 1, 4, 1,},
                { 1, 2, 2, 4, 2,},
                { 1, 3, 3, 3, 1,},
                { 2, 1, 1, 2, 1,},
                { 2, 4, 4, 3, 3,},
            };
            cornerX = -3f;
            cornerY = 3f;
            tileSize = 1.5f;
            levelHeight = 20;
            levelWidth = 5;
        }
        else if (gd.LevelToLoad == 58)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 2, 3, 2, 1, 4, 4, 1, 4, 1, 2, 3, 1, 2, 1, 4, 4, 2, 1},
                { 2, 2, 3, 1, 4, 1, 1, 3, 1, 4, 2, 4, 2, 4, 3, 1, 3, 1, 3, 1},
                { 2, 1, 3, 1, 2, 2, 4, 4, 2, 1, 2, 4, 2, 1, 3, 4, 3, 4, 1, 3},
                { 1, 3, 2, 4, 3, 4, 1, 3, 2, 4, 3, 2, 3, 1, 2, 4, 2, 3, 1, 4},
                { 1, 3, 1, 1, 4, 2, 3, 4, 3, 1, 3, 1, 1, 4, 3, 3, 2, 1, 2, 3},
            };
            cornerX = -7f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 20;
        }
        else if (gd.LevelToLoad == 59)
        {
            levelMatrix = new int[,]
            {
                { 1, 1, 5, 1, 4, 4, 1, 1, 2, 3, 5, 2, 1, 5, 1, 1, 4, 5, 3, 4},
                { 2, 4, 5, 4, 3, 1, 2, 3, 3, 2, 2, 3, 1, 3, 5, 1, 4, 3, 2, 4},
                { 2, 1, 2, 4, 3, 1, 2, 1, 3, 3, 5, 2, 3, 2, 3, 2, 2, 2, 3, 2},
                { 3, 2, 3, 3, 2, 5, 4, 5, 4, 1, 4, 4, 5, 2, 4, 1, 1, 3, 1, 2},
                { 3, 1, 2, 2, 4, 4, 1, 5, 4, 2, 1, 3, 1, 1, 4, 2, 2, 5, 3, 3},
            };
            cornerX = -7f;
            cornerY = 3.5f;
            tileSize = 1.5f;
            levelHeight = 5;
            levelWidth = 20;
        }
        else if (gd.LevelToLoad == 60)
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 2, 3},
                { 1, 1, 2, 1},
                { 4, 4, 5, 1},
                { 1, 3, 6, 3},
                { 1, 4, 6, 5},
                { 5, 4, 1, 3},
                { 3, 6, 4, 3},
                { 1, 5, 2, 6},
                { 2, 5, 4, 6},
                { 2, 1, 5, 4},
                { 3, 3, 2, 4},
                { 6, 2, 1, 1},
            };
            cornerX = -3f;
            cornerY = 3f;
            tileSize = 2f;
            levelHeight = 12;
            levelWidth = 4;
        }
        else
        {
            levelMatrix = new int[,]
            {
                { 1, 2, 3},
                { 4, 5, 6},
                { 7, 8, 1}
            };
            cornerX = -2f;
            cornerY = 3.5f;
            tileSize = 2;
            levelHeight = 3;
            levelWidth = 3;
        }
        
    }

    // Creates the game board based on the colors in the level matrix
    private void BuildLevel()
    {
        tiles = new Transform[levelHeight, levelWidth];

        for (int x = 0; x < levelWidth; x++)
            for (int y = 0; y < levelHeight; y++)
            {
                tiles[y, x] = Instantiate(gameTile, new Vector3(cornerX + x * tileSize, cornerY - y * tileSize), Quaternion.identity);
                GameTileScript gts = tiles[y, x].GetComponent<GameTileScript>();
                Sprite sprite;
                GameData gd = GameObject.Find("GameData").GetComponent<GameData>();
                if (gd.TileStyle == 0)
                    sprite = tileSprite[0];
                else sprite = tileSprite[levelMatrix[y, x]];
                gts.Setup(tileSize, colorsArray[levelMatrix[y, x]], sprite);
            }
    }

    // Checks whether the mouse is over any game tile stores the position of the tile
    private bool FindTileMouseOver()
    {
        bool found = false;

        for (int x = 0; x < levelWidth; x++)
            for (int y = 0; y < levelHeight; y++)
            {
                if (levelMatrix[y, x] > 0)
                {
                    if (tiles[y, x])
                    {
                        GameTileScript currentTile = tiles[y, x].GetComponent<GameTileScript>();
                        currentTile.GetComponent<SpriteRenderer>().color = currentTile.tileColor;

                        if (currentTile.Hover)
                        {
                            currentX = x;
                            currentY = y;
                            found = true;
                        }
                    }
                }
            }

        return found;
    }

    // Highlight the tiles adjacent to the tile the mouse is over if they have the same color
    private void HighlightTiles(int x, int y)
    {
        tiles[y, x].GetComponent<SpriteRenderer>().color = Color.white;
        // Up
        if (y > 0 && levelMatrix[y - 1, x] == levelMatrix[y, x])
            if (tiles[y - 1, x].GetComponent<SpriteRenderer>().color != Color.white)
                HighlightTiles(x, y - 1);
        // Left
        if (x > 0 && levelMatrix[y, x - 1] == levelMatrix[y, x])
            if (tiles[y, x - 1].GetComponent<SpriteRenderer>().color != Color.white)
                HighlightTiles(x - 1, y);
        // Right
        if (x < levelWidth - 1 && levelMatrix[y, x + 1] == levelMatrix[y, x])
            if (tiles[y, x + 1].GetComponent<SpriteRenderer>().color != Color.white)
                HighlightTiles(x + 1, y);
        // Down
        if (y < levelHeight - 1 && levelMatrix[y + 1, x] == levelMatrix[y, x])
            if (tiles[y + 1, x].GetComponent<SpriteRenderer>().color != Color.white)
                HighlightTiles(x, y + 1);
    }

    // If there is at least one adjacent tile of the same color we can destroy the tiles
    private bool CanDestroyTile(int x, int y)
    {

        if (!tiles[y, x])
            return false;

        if (levelMatrix[y, x] == 0)
            return false;

        if (y > 0 && levelMatrix[y - 1, x] != 0 && !tiles[y - 1, x])
            return false;

        if (x > 0 && levelMatrix[y, x - 1] != 0 && !tiles[y, x - 1])
            return false;

        if (x < levelWidth - 1 && levelMatrix[y, x + 1] != 0 && !tiles[y, x + 1])
            return false;

        if (y < levelHeight - 1 && levelMatrix[y + 1, x] != 0 && !tiles[y + 1, x])
            return false;

        // Up
        if (y > 0 && levelMatrix[y - 1, x] == levelMatrix[y, x])
            return true;
        // Left
        if (x > 0 && levelMatrix[y, x - 1] == levelMatrix[y, x])
            return true;
        // Right
        if (x < levelWidth - 1 && levelMatrix[y, x + 1] == levelMatrix[y, x])
            return true;
        // Down
        if (y < levelHeight - 1 && levelMatrix[y + 1, x] == levelMatrix[y, x])
            return true;

        return false;
    }

    // Destroy the tile the mouse is over and the adjacent tiles of the same color
    private void DestroyTiles(int x, int y)
    {
        levelMatrix[y, x] = 0;

        // Up
        if (y > 0 && levelMatrix[y - 1, x] != 0)
            if (tiles[y - 1, x].GetComponent<SpriteRenderer>().color == tiles[y, x].GetComponent<SpriteRenderer>().color)
                DestroyTiles(x, y - 1);
        // Left
        if (x > 0 && levelMatrix[y, x - 1] != 0)
            if (tiles[y, x - 1].GetComponent<SpriteRenderer>().color == tiles[y, x].GetComponent<SpriteRenderer>().color)
                DestroyTiles(x - 1, y);
        // Right
        if (x < levelWidth - 1 && levelMatrix[y, x + 1] != 0)
            if (tiles[y, x + 1].GetComponent<SpriteRenderer>().color == tiles[y, x].GetComponent<SpriteRenderer>().color)
                DestroyTiles(x + 1, y);
        // Down
        if (y < levelHeight - 1 && levelMatrix[y + 1, x] != 0)
            if (tiles[y + 1, x].GetComponent<SpriteRenderer>().color == tiles[y, x].GetComponent<SpriteRenderer>().color)
                DestroyTiles(x, y + 1);

        tiles[y, x].GetComponent<GameTileScript>().Fade = true;
    }

    // Handles the falling and the sliding of the game tiles
    void AnimateGameBoard()
    {
        // Camera rotation for certain levels
        GameData gd = GameObject.Find("GameData").GetComponent<GameData>();
        if (gd.LevelToLoad == 50)
            StartCoroutine(RotateGameBoard(true, .025f, 0f));
        else if(gd.LevelToLoad == 51)
            StartCoroutine(RotateGameBoard(false, 9f, 90f));
        else if(gd.LevelToLoad == 52)
        {
            if (moves % 2 == 1)
                StartCoroutine(RotateGameBoard(true, 3f, 90f));
            else StartCoroutine(RotateGameBoard(true, 6f, 180f));
        }
        else if(gd.LevelToLoad == 53)
        {
            StartCoroutine(RotateGameBoard(false, 10, 450));
        }
        else if(gd.LevelToLoad == 54)
        {
            if (moves % 2 == 1)
                StartCoroutine(RotateGameBoard((Random.value > 0.5f), 3, 45));
            else StartCoroutine(RotateGameBoard((Random.value > 0.5f), 3, 90));
        }
        else if(gd.LevelToLoad == 55)
        {
            StartCoroutine(RotateGameBoard((Random.value > 0.5f), Random.Range(1, 5), Random.Range(1, 270)));
        }
    }

    private void FixedUpdate()
    {
        if (!AnimatingGameBoard)
            return;

        if (falls > 0 || slides > 0)
            return;

        if (!TilesFall() && !TilesSlide())
            AnimatingGameBoard = false;

    }

    // Handles the falling of the game tiles and triggers the falling animation
    private bool TilesFall()
    {
        bool fall = false;

        for (int x = 0; x < levelWidth; x++)
            for (int y = levelHeight - 2; y >= 0; y--)
                if (levelMatrix[y, x] != 0 && levelMatrix[y + 1, x] == 0)
                {
                    fall = true;
                    falls++;
                    levelMatrix[y + 1, x] = levelMatrix[y, x];
                    levelMatrix[y, x] = 0;
                    tiles[y, x].GetComponent<GameTileScript>().Fall = true;
                    tiles[y + 1, x] = tiles[y, x];
                }

        return fall;
    }

    // Handles the sliding of the game tiles and triggers the sliding animation
    private bool TilesSlide()
    {
        bool slide = false;

        for (int x = 1; x < levelWidth; x++)
            for (int y = 0; y < levelHeight; y++)
                if (levelMatrix[y, x] != 0 && levelMatrix[y, x - 1] == 0 && levelMatrix[levelHeight - 1, x - 1] == 0)
                {
                    slide = true;
                    slides++;
                    levelMatrix[y, x - 1] = levelMatrix[y, x];
                    levelMatrix[y, x] = 0;
                    tiles[y, x].GetComponent<GameTileScript>().Slide = true;
                    tiles[y, x - 1] = tiles[y, x];
                }

        return slide;
    }

    // Animates the rotation of the game board
    IEnumerator RotateGameBoard(bool left, float speed, float degrees)
    {
        float rotationSpeed = left ? speed : -speed;
        Transform board = GameObject.Find("GameBoard").GetComponent<Transform>();
        // Constant rotation
        if (degrees == 0)
            while (!levelWon)
            {
                board.Rotate(Vector3.forward, rotationSpeed);
                yield return null;
            }
        // Rotation by set angle
        else
        {
            float angle = degrees / speed;
            for (int i = 1; i <= angle; i++)
            {
                board.Rotate(Vector3.forward, rotationSpeed);
                yield return null;
            }
        }
    }

    IEnumerator TextFadeIn()
    {
        TextMeshPro text = GameObject.Find("Instructions").GetComponent<TextMeshPro>();
        for (float i = 0f; i < 1; i += 0.01f)
        {
            text.alpha = i;
            yield return null;
        }
        yield return new WaitForSeconds(3);
        text = GameObject.Find("TextSettings").GetComponent<TextMeshPro>();
        for (float i = 0f; i < 1; i += 0.01f)
        {
            text.alpha = i;
            yield return null;
        }
        yield return new WaitForSeconds(3);
        text = GameObject.Find("TextReset").GetComponent<TextMeshPro>();
        for (float i = 0f; i < 1; i += 0.01f)
        {
            text.alpha = i;
            yield return null;
        }
    }
}
