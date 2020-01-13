using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditor : MonoBehaviour
{
    [SerializeField] InputField xSizeField = null;
    [SerializeField] InputField ySizeField = null;
    [SerializeField] TileSpriteTable tileSpriteTable = null;
    [SerializeField] GameObject mapBackGround = null;
    [SerializeField] SpriteRenderer tilePrefab = null;
    [SerializeField] ToggleGroup tileMenu = null;
    Dictionary<AdjacentObstacle, Sprite> obstacleSpriteDictionary = null;
    Dictionary<AdjacentObstacle, Sprite> prisonSpriteDictionary = null;
    [SerializeField] Button saveButton = null;
    [SerializeField] Button loadButton = null;
    [SerializeField] FileSystem fileSystem = null;
    [SerializeField] MoveButton[] prisonMoveButtons = null;

    SpriteRenderer[,] spriteList;

    [SerializeField] Vector2Int minMapSize = Vector2Int.zero;
    Vector2Int mapSize;

    bool isInputMode;

    TileMap tileMap;

    // Start is called before the first frame update
    void Start()
    {
        spriteList = new SpriteRenderer[0, 0];
        xSizeField.onValueChanged.AddListener(SetXSize);
        ySizeField.onValueChanged.AddListener(SetYSize);
        mapSize = minMapSize;

        isInputMode = false;

        obstacleSpriteDictionary = new Dictionary<AdjacentObstacle, Sprite>();
        for (int i = 0; i < tileSpriteTable.ObstacleSprites.Length; i++)
        {
            obstacleSpriteDictionary.Add(tileSpriteTable.ObstacleSprites[i].adjacentObstacleDirection, tileSpriteTable.ObstacleSprites[i].sprite);
        }

        prisonSpriteDictionary = new Dictionary<AdjacentObstacle, Sprite>();
        for (int i = 0; i < tileSpriteTable.PrisonSprites.Length; i++)
        {
            prisonSpriteDictionary.Add(tileSpriteTable.PrisonSprites[i].adjacentObstacleDirection, tileSpriteTable.PrisonSprites[i].sprite);
        }

        saveButton.onClick.AddListener(() => fileSystem.Open(this, FileSystemMode.Save));
        loadButton.onClick.AddListener(() => fileSystem.Open(this, FileSystemMode.Load));

        for(int i = 0; i < prisonMoveButtons.Length; i++)
        {
            MoveButton buttonPair = prisonMoveButtons[i];
            buttonPair.button.onClick.AddListener(() => MovePrison(buttonPair.direction));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            isInputMode = true;
        }

        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            isInputMode = false;
        }

        if(isInputMode)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Input.touchCount > 0)
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            Physics.Raycast(ray, out RaycastHit hit);
            if (hit.collider != null)
            {
                Vector2Int coord = hit.collider.GetComponent<EditorTile>().Coord;
                ClickTile(coord);
            }
        }
    }

    void ClickTile(Vector2Int coord)
    {
        bool isPrison = tileMap.CheckTileType(coord, Tile.Prison) || tileMap.CheckTileType(coord, Tile.Entrance) || tileMap.CheckTileType(coord, Tile.PrisonWall);

        if (!isPrison)
        {
            Toggle activeToggle = null;
            foreach (var toggle in tileMenu.ActiveToggles())
            {
                if (toggle.isOn)
                {
                    activeToggle = toggle;
                }
            }

            if (activeToggle != null)
            {
                tileMap.SetTile(coord, activeToggle.GetComponent<TileToggle>().ToogleTile);

                spriteList[coord.y, coord.x].sprite = NewMapSprite(new Vector2Int(coord.x, coord.y));
                for (int i = 0; i < Global.directions.Length; i++)
                {
                    Vector2Int adjacentCoord = coord + Global.directions[i];
                    if (tileMap.IsCoordInRange(adjacentCoord))
                        spriteList[adjacentCoord.y, adjacentCoord.x].sprite = NewMapSprite(new Vector2Int(adjacentCoord.x, adjacentCoord.y));
                }
                for (int i = 0; i < Global.diagonals.Length; i++)
                {
                    Vector2Int adjacentCoord = coord + Global.diagonals[i];
                    if (tileMap.IsCoordInRange(adjacentCoord))
                        spriteList[adjacentCoord.y, adjacentCoord.x].sprite = NewMapSprite(new Vector2Int(adjacentCoord.x, adjacentCoord.y));
                }
            }
        }
    }

    Sprite NewMapSprite(Vector2Int coord)
    {
        Sprite returnValue;

        if (tileMap.CheckTileType(coord, Tile.Obstacle))
        {
            AdjacentObstacle spriteIndex = tileMap.IndexOfObstacleSprite(coord);
            returnValue = obstacleSpriteDictionary[spriteIndex];
        }
        else if (tileMap.CheckTileType(coord, Tile.PrisonWall))
        {
            AdjacentObstacle spriteIndex = tileMap.IndexOfPrisionSprite(coord);
            returnValue = prisonSpriteDictionary[spriteIndex];
        }
        else if (tileMap.CheckTileType(coord, Tile.Entrance))
        {
            returnValue = tileSpriteTable.PrisonEntranceSprite;
        }
        else if (tileMap.CheckTileType(coord, Tile.Cookie))
        {
            returnValue = tileSpriteTable.CookieSprite;
        }
        else if (tileMap.CheckTileType(coord, Tile.PCookie))
        {
            returnValue = tileSpriteTable.PCookieSprite;
        }
        else if(tileMap.CheckTileType(coord, Tile.PlayerPosition))
        {
            returnValue = tileSpriteTable.PlayerSprite;
        }
        else if (tileMap.CheckTileType(coord, Tile.Prison))
        {
            returnValue = tileSpriteTable.CookieSprite;
        }
        else
        {
            returnValue = obstacleSpriteDictionary[AdjacentObstacle.None];
        }
        return returnValue;
    }

    void SetXSize(string xSize)
    {
        int size = int.Parse(xSize);
        if (size < 20)
        {
            size = 20;
        }

        try
        {
            MakeEmptyMap(new Vector2Int(size, mapSize.y));
        }
        catch (FormatException) { }
    }

    void SetYSize(string ySize)
    {
        int size = int.Parse(ySize);
        if (size < 20)
        {
            size = 20;
        }

        try
        {
            MakeEmptyMap(new Vector2Int(mapSize.x, size));
        }
        catch (FormatException) { }
    }

    void MakeMap()
    {
        Tile[,] tiles = tileMap.GetTiles();

        for (int i = 0; i < spriteList.GetLength(0); i++)
        {
            for (int j = 0; j < spriteList.GetLength(1); j++)
            {
                Destroy(spriteList[i, j].gameObject);
            }
        }

        mapSize.x = tiles.GetLength(1);
        mapSize.y = tiles.GetLength(0);
        spriteList = new SpriteRenderer[tiles.GetLength(0), tiles.GetLength(1)];

        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                spriteList[i, j] = Instantiate(tilePrefab);
                spriteList[i, j].transform.SetParent(mapBackGround.transform);
                spriteList[i, j].transform.position = new Vector3(j, i, 0);
                spriteList[i, j].transform.localScale = new Vector3(1, 1, 1);
                spriteList[i, j].sprite = NewMapSprite(new Vector2Int(j, i));
                spriteList[i, j].GetComponent<EditorTile>().Init(new Vector2Int(j, i));
            }
        }

        float orthographicSize = Mathf.Max(spriteList.GetLength(1) * 0.5f * 8.0f / 5.0f / Screen.width * Screen.height, spriteList.GetLength(0) * 0.5f);
        Camera.main.orthographicSize = orthographicSize > 0 ? orthographicSize : 1;
        float cameraXSize = Camera.main.orthographicSize / Screen.height * Screen.width;

        Vector3 cameraPosition = Camera.main.transform.position;
        cameraPosition.x = (spriteList.GetLength(1) - tilePrefab.transform.localScale.x) * 0.5f + cameraXSize * 3.0f / 8.0f;
        cameraPosition.y = (spriteList.GetLength(0) - tilePrefab.transform.localScale.y) * 0.5f;
        Camera.main.transform.position = cameraPosition;
    }

    void MakeEmptyMap(Vector2Int size)
    {
        tileMap = new TileMap(size);
        MakeMap();
    }

    public void LoadNewMap(string map)
    {
        tileMap = new TileMap(map);
        MakeMap();
    }

    public string GetJson()
    {
        return tileMap.GetJson();
    }

    void MovePrison(Direction direction)
    {
        tileMap.MovePrison(direction);
        MakeMap();
    }
}
