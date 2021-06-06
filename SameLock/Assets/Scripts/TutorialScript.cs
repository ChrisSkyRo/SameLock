using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TutorialScript : MonoBehaviour
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
    private Transform[,] tiles;         // The gameobjects that make the level
    private bool AnimatingGameBoard;    // Whether the gameboard is animating or expecting user input
    private int currentX, currentY;     // Mouse position on gameboard
    private int falls;                  // Number of falling animations playing
    private int slides;                 // Number of sliding animations playing
    private float timer;

    private void Awake()
    {
        timer = 3f;
        levelWon = false;
        AnimatingGameBoard = false;
        currentX = currentY = -1;
        falls = 0;
        slides = 0;
        TextMeshPro text = GameObject.Find("Instructions").GetComponent<TextMeshPro>();
        text.alpha = 0;
        text = GameObject.Find("TextSettings").GetComponent<TextMeshPro>();
        text.alpha = 0;
        text = GameObject.Find("TextReset").GetComponent<TextMeshPro>();
        text.alpha = 0;
    }

    private void Start()
    {
        StartCoroutine(TextFadeIn());
        // Initialize the colors array
        InitializeColors();
        // Set the color of the game tiles in the matrix
        InitializeMatrix();
        // Creates the game board based on the colors in the level matrix
        BuildTutorial();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (!AnimatingGameBoard)
        {
            if (!levelWon && levelMatrix[levelHeight - 1, 0] == 0)
            {
                Destroy(GameObject.Find("Instructions"));
                GameObject.Find("SettingsButton").transform.GetChild(0).position += new Vector3(100, 0);
                GameObject.Find("ResetButton").transform.GetChild(0).position += new Vector3(100, 0);
                GameObject.Find("SFXManager").GetComponent<SFXScript>().PlaySFX("win");
                levelWon = true;
                Instantiate(win, new Vector3(0, 0, -5), Quaternion.identity);
                GameData gd = GameObject.Find("GameData").GetComponent<GameData>();
                gd.AchievementsUnlocked[0] = 1;
                PlayerPrefs.SetInt("Achievement0", 1);
                GameObject.Find("API Handler").GetComponent<APIHandler>().UnlockMedal(62021);
                if (gd.AchievementsUnlocked[14] == 0 && timer > 0)
                {
                    gd.AchievementsUnlocked[14] = 1;
                    PlayerPrefs.SetInt("Achievement14", 1);
                    GameObject.Find("API Handler").GetComponent<APIHandler>().UnlockMedal(62035);
                }
                PlayerPrefs.Save();
            }

            bool tileHover = FindTileMouseOver();

            if (!tileHover)
                return;

            HighlightTiles(currentX, currentY);

            if (Input.GetMouseButtonUp(0) && CanDestroyTile(currentX, currentY))
            {
                GameObject.Find("SFXManager").GetComponent<SFXScript>().PlaySFX("game");
                AnimatingGameBoard = true;
                DestroyTiles(currentX, currentY);
                StartCoroutine(AnimateGameBoard());
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
    }

    // Set the color of the game tiles in the matrix
    private void InitializeMatrix()
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

    // Creates the game board based on the colors in the level matrix
    private void BuildTutorial()
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
                gts.Setup(1.25f, colorsArray[levelMatrix[y, x]], sprite);
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
            if(tiles[y - 1, x].GetComponent<SpriteRenderer>().color != Color.white)
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
        if (x < 2 && levelMatrix[y, x + 1] == levelMatrix[y, x])
            return true;
        // Down
        if (y < 2 && levelMatrix[y + 1, x] == levelMatrix[y, x])
            return true;

        return false;
    }

    // Destroy the tile the mouse is over and the adjacent tiles of the same color
    private void DestroyTiles(int x, int y)
    {
        levelMatrix[y, x] = 0;

        // Up
        if (y > 0 && levelMatrix[y - 1, x] != 0)
            if (tiles[y - 1, x].GetComponent<SpriteRenderer>().color == Color.white)
                DestroyTiles(x, y - 1);
        // Left
        if (x > 0 && levelMatrix[y, x - 1] != 0)
            if (tiles[y, x - 1].GetComponent<SpriteRenderer>().color == Color.white)
                DestroyTiles(x - 1, y);
        // Right
        if (x < levelWidth - 1 && levelMatrix[y, x + 1] != 0)
            if (tiles[y, x + 1].GetComponent<SpriteRenderer>().color == Color.white)
                DestroyTiles(x + 1, y);
        // Down
        if (y < levelHeight - 1 && levelMatrix[y + 1, x] != 0)
            if (tiles[y + 1, x].GetComponent<SpriteRenderer>().color == Color.white)
                DestroyTiles(x, y + 1);

        StartCoroutine(TileFade(x, y));
    }

    // The game tile fades out then it is destroyed
    IEnumerator TileFade(int x, int y)
    {
        SpriteRenderer sr = tiles[y, x].GetComponent<SpriteRenderer>();
        Color alpha = sr.color;
        for(float f = 1f;f > 0; f -= 0.1f)
        {
            alpha.a = f;
            sr.color = alpha;
            yield return null;
        }
        Destroy(tiles[y, x].gameObject);
    }

    // Handles the falling and the sliding of the game tiles
    IEnumerator AnimateGameBoard()
    {
        while(TilesFall())
            while(falls > 0)
                yield return null;
        while (TilesSlide())
            while (slides > 0)
                yield return null;
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
                    StartCoroutine(TileFall(x, y));
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
                    StartCoroutine(TileSlide(x, y));
                }

        return slide;
    }

    // Animates the falling of a game tile
    IEnumerator TileFall(int x, int y)
    {
        float fallSpeed = tileSize / 10;
        for (float i = tileSize; i > 0; i -= fallSpeed)
        {
            tiles[y, x].position -= new Vector3(0, fallSpeed);
            yield return null;
        }
        tiles[y + 1, x] = tiles[y, x];
        falls--;
    }

    // Animates the sliding of a game tile to the left
    IEnumerator TileSlide(int x, int y)
    {
        float slideSpeed = tileSize / 10;
        for(float i = tileSize; i > 0; i -= slideSpeed)
        {
            tiles[y, x].position -= new Vector3(slideSpeed, 0);
            yield return null;
        }
        tiles[y, x - 1] = tiles[y, x];
        slides--;
    }

    IEnumerator TextFadeIn()
    {
        TextMeshPro text = GameObject.Find("Instructions").GetComponent<TextMeshPro>();
        for(float i = 0f;i < 1;i += 0.01f)
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
