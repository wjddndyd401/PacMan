using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
	public static InGameManager Instance { get; private set; } = null;

	TileMap tileMap;
	[SerializeField] Canvas canvas = null;
	[SerializeField] GameObject tileMapBackground = null;
	[SerializeField] SpriteRenderer tilePrefab = null;
	[SerializeField] Pacman pacman = null;
	[SerializeField] Ghost[] ghosts = null;
	[SerializeField] SpriteRenderer cookiePrefab = null;
	[SerializeField] TileSpriteTable tileSpriteTable = null;
	Dictionary<AdjacentObstacle, Sprite> obstacleSpriteDictionary = null;
	Dictionary<AdjacentObstacle, Sprite> prisonSpriteDictionary = null;
	PathFinder pathFinder;
	[SerializeField] UiMenu scoreField = new UiMenu();
	[SerializeField] UiMenu highScoreField = new UiMenu();
	[SerializeField] UiMenu lifeField = new UiMenu();
	int score;
	int highScore;
	int currentLife;
	[SerializeField] Sprite lifeSprite = null;
	[SerializeField] Image menuObject = null;
	[SerializeField] int cookieScore = 0;
	[SerializeField] int pCookieScore = 0;
	Dictionary<GhostPattern, Vector2Int> ghostTargetInVulnerable;
	Vector2Int blinkyPosition;      // for inky
	[SerializeField] GameObject loadingScreen = null;
	[SerializeField] Text loadingText = null;
	[SerializeField] Text countText = null;
	[SerializeField] float loadingTime = 2.0f;
	[SerializeField] Text winText = null;
	[SerializeField] Text loseText = null;
	[SerializeField] Text stageClearText = null;
	string[] maps;
	int cookieCount;
	[SerializeField] int[] ghostScores = null;
	int eatenGhostCount;
	[SerializeField] FloatingText scoreValuePrefab = null;
	[SerializeField] MoveButton[] moveButtons = null;
	[SerializeField] Camera gameCamera = null;
	[SerializeField] RawImage gameScreen = null;
	RenderTexture screenTexture;

	[SerializeField] Direction[] secretCode = { Direction.Up, Direction.Up, Direction.Up, Direction.Down, Direction.Down, Direction.Down };
	int secretIndex = 0;
	[SerializeField] Text logView = null;
	public bool PowerOverwhelming { get; private set; } = false;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);

		object[] mapObjects = Resources.LoadAll("Map");
		maps = new string[mapObjects.Length];
		for (int i = 0; i < mapObjects.Length; i++)
		{
			if ((TextAsset)mapObjects[i] != null)
			{
				maps[i] = ((TextAsset)mapObjects[i]).text;
			} else
			{
				maps[i] = null;
			}
		}

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

		pacman.gameObject.SetActive(false);
	}

	void Start()
	{
		GameManager.Instance.Restart();
		currentLife = GameManager.Instance.MaxLife;

		SetUiMenuName(lifeField);

		score = 0;
		SetUiMenuName(scoreField);
		SetScoreField(scoreField, score);

		highScore = PlayerPrefs.GetInt("HighScore", 0);
		SetUiMenuName(highScoreField);
		SetScoreField(highScoreField, highScore);

		GetUiTransformWithControlMode(out Dictionary<Direction, Vector2> position, out Dictionary<Direction, Vector2> anchor, out RectTransform screen);

		gameScreen.rectTransform.offsetMin = screen.offsetMin;
		gameScreen.rectTransform.offsetMax = screen.offsetMax;

		for (int i = 0; i < moveButtons.Length; i++)
		{
			MoveButton buttonPair = moveButtons[i];
			RectTransform rt = buttonPair.button.GetComponent<RectTransform>();

			rt.anchorMin = anchor[buttonPair.direction];
			rt.anchorMax = anchor[buttonPair.direction];
			rt.anchoredPosition = position[buttonPair.direction];

			buttonPair.button.onClick.AddListener(() =>
			{
				if (pacman.isActiveAndEnabled)
				{
					pacman.SetDirection(buttonPair.direction);
				}
			});
		}

		StartNewGame();
	}

	void Update()
	{
		if (pacman.gameObject.activeSelf == false)
		{
			currentLife--;
			if (currentLife > 0)
			{
				StartCoroutine(StartCurrentGame());
			}
			else
			{
				PrintWaitScreen(loseText, () => SceneManager.LoadScene("Title"));
			}
		}
		else if (cookieCount <= 0)
		{
			pacman.SetToWaitMode();
			if(GameManager.Instance.CurrentStage < maps.Length)
			{
				PrintWaitScreen(stageClearText, () =>
				{
					GameManager.Instance.GoToNextStage();
					StartNewGame();
				});
			}
			else
			{
				PrintWaitScreen(winText, () => SceneManager.LoadScene("Title"));
			}
		}

		Vector3 cameraPosition = pacman.transform.position - new Vector3(0, 0, 10);
		Vector2Int mapSize = tileMap.MapSize();
		Vector2 screenSize = new Vector2(gameScreen.rectTransform.rect.width, gameScreen.rectTransform.rect.height);

		if (screenTexture != null)
		{
			screenTexture.Release();
		}
		screenTexture = new RenderTexture((int)gameScreen.rectTransform.rect.width, (int)gameScreen.rectTransform.rect.height, 24);
		gameCamera.targetTexture = screenTexture;
		gameScreen.texture = screenTexture;

		if (gameCamera.orthographicSize * screenSize.x / screenSize.y * 2 > mapSize.x)
		{
			cameraPosition.x = mapSize.x * 0.5f;
		}
		else if (cameraPosition.x < gameCamera.orthographicSize * screenSize.x / screenSize.y)
		{
			cameraPosition.x = gameCamera.orthographicSize * screenSize.x / screenSize.y;
		}
		else if (cameraPosition.x > mapSize.x - gameCamera.orthographicSize * screenSize.x / screenSize.y)
		{
			cameraPosition.x = mapSize.x - gameCamera.orthographicSize * screenSize.x / screenSize.y;
		}

		if(gameCamera.orthographicSize * 2 > mapSize.y)
		{
			cameraPosition.y = mapSize.y * 0.5f;
		}
		else if (cameraPosition.y < gameCamera.orthographicSize)
		{
			cameraPosition.y = gameCamera.orthographicSize;
		}
		else if (cameraPosition.y > mapSize.y - gameCamera.orthographicSize)
		{
			cameraPosition.y = mapSize.y - gameCamera.orthographicSize;
		}

		cameraPosition.x -= 0.5f;
		cameraPosition.y -= 0.5f;
		gameCamera.transform.position = cameraPosition;
	}

	void GetUiTransformWithControlMode(out Dictionary<Direction, Vector2> position, out Dictionary<Direction, Vector2> anchor, out RectTransform screen)
	{
		position = new Dictionary<Direction, Vector2>();
		anchor = new Dictionary<Direction, Vector2>();
		screen = gameScreen.rectTransform;
		if (GameManager.Instance.controlMode == ControlMode.TwoHand)
		{
			position.Add(Direction.Up, new Vector2(50, 35));
			position.Add(Direction.Right, new Vector2(-40, 0));
			position.Add(Direction.Down, new Vector2(50, -35));
			position.Add(Direction.Left, new Vector2(-110, 0));
			anchor.Add(Direction.Up, new Vector2(0f, 0.5f));
			anchor.Add(Direction.Right, new Vector2(1f, 0.5f));
			anchor.Add(Direction.Down, new Vector2(0f, 0.5f));
			anchor.Add(Direction.Left, new Vector2(1f, 0.5f));
			screen.offsetMin = new Vector2(100, 0);
			screen.offsetMax = new Vector2(-150, 0);
		}
		else if (GameManager.Instance.controlMode == ControlMode.RightHand)
		{
			position.Add(Direction.Up, new Vector2(-110, 70));
			position.Add(Direction.Right, new Vector2(-40, 0));
			position.Add(Direction.Down, new Vector2(-110, -70));
			position.Add(Direction.Left, new Vector2(-180, 0));
			anchor.Add(Direction.Up, new Vector2(1f, 0.5f));
			anchor.Add(Direction.Right, new Vector2(1f, 0.5f));
			anchor.Add(Direction.Down, new Vector2(1f, 0.5f));
			anchor.Add(Direction.Left, new Vector2(1f, 0.5f));
			screen.offsetMin = new Vector2(0, 0);
			screen.offsetMax = new Vector2(-250, 0);
		}
		else if (GameManager.Instance.controlMode == ControlMode.LeftHand)
		{
			position.Add(Direction.Up, new Vector2(110, 70));
			position.Add(Direction.Right, new Vector2(180, 0));
			position.Add(Direction.Down, new Vector2(110, -70));
			position.Add(Direction.Left, new Vector2(40, 0));
			anchor.Add(Direction.Up, new Vector2(0f, 0.5f));
			anchor.Add(Direction.Right, new Vector2(0f, 0.5f));
			anchor.Add(Direction.Down, new Vector2(0f, 0.5f));
			anchor.Add(Direction.Left, new Vector2(0f, 0.5f));
			screen.offsetMin = new Vector2(250, 0);
			screen.offsetMax = new Vector2(0, 0);
		}
	}

	void PrintWaitScreen(MonoBehaviour screen, System.Action clickAction)
	{
		screen.gameObject.SetActive(true);
		if (Input.GetMouseButton(0) || Input.touchCount > 0)
		{
			clickAction();
		}
	}

	void StartNewGame()
	{
		winText.gameObject.SetActive(false);
		loseText.gameObject.SetActive(false);
		stageClearText.gameObject.SetActive(false);

		Transform[] childList = tileMapBackground.GetComponentsInChildren<Transform>(true);
		if (childList != null)
		{
			for (int i = 0; i < childList.Length; i++)
			{
				if (childList[i] != tileMapBackground.transform)
					Destroy(childList[i].gameObject);
			}
		}

		tileMap = new TileMap(maps[GameManager.Instance.CurrentStage - 1]);
		pathFinder = new PathFinder(tileMap);
		PrintMap();

		ghostTargetInVulnerable = new Dictionary<GhostPattern, Vector2Int>();
		ghostTargetInVulnerable.Add(GhostPattern.Blinky, tileMap.MapSize());
		ghostTargetInVulnerable.Add(GhostPattern.Pinky, new Vector2Int(tileMap.MapSize().x, 0));
		ghostTargetInVulnerable.Add(GhostPattern.Inky, new Vector2Int(0, tileMap.MapSize().y));
		ghostTargetInVulnerable.Add(GhostPattern.Clyde, new Vector2Int(0, 0));

		StartCoroutine(StartCurrentGame());
	}

	IEnumerator StartCurrentGame()
	{
		pacman.gameObject.SetActive(true);
		Vector3 playerPosition = Vector3.zero;
		playerPosition.x = tileMap.PlayerPosition().x;
		playerPosition.y = tileMap.PlayerPosition().y;
		pacman.transform.position = playerPosition;
		pacman.Init();

		for (int i = 0; i < ghosts.Length; i++)
		{
			Vector3 ghostPosision = Vector3.zero;
			ghostPosision.x = tileMap.GhostPosition((GhostPattern)i).x;
			ghostPosision.y = tileMap.GhostPosition((GhostPattern)i).y;
			ghosts[i].transform.position = ghostPosision;
			ghosts[i].Init();
		}
		blinkyPosition = tileMap.PrisonPosition();

		SetLifeField();

		loadingScreen.gameObject.SetActive(true);
		loadingText.text = "Stage " + GameManager.Instance.CurrentStage + "\n\n   x " + currentLife;
		Time.timeScale = 0;		
		yield return StartCoroutine(WaitingInZeroTimeScale(loadingTime));
		loadingScreen.gameObject.SetActive(false);

		countText.text = "Ready";
		countText.gameObject.SetActive(true);
		yield return StartCoroutine(WaitingInZeroTimeScale(1.0f));

		countText.text = "Go!";
		yield return StartCoroutine(WaitingInZeroTimeScale(1.0f));
		countText.gameObject.SetActive(false);

		Time.timeScale = 1;
	}

	IEnumerator WaitingInZeroTimeScale(float second)
	{
		float countFinishTime = Time.time + second;
		float currentTime = Time.time;
		while (currentTime < countFinishTime)
		{
			yield return null;
			currentTime += Time.unscaledDeltaTime;
		}
	}

	void PrintMap()
	{
		Tile[,] tiles = tileMap.GetTiles();
		for (int i = 0; i < tiles.GetLength(0); i++)
		{
			for (int j = 0; j < tiles.GetLength(1); j++)
			{
				Vector3 position = new Vector3(j, i, 0);
				SpriteRenderer sprite = Instantiate(tilePrefab);

				sprite.transform.SetParent(tileMapBackground.transform);
				sprite.transform.localPosition = position;
				sprite.transform.localScale = new Vector3(1, 1, 1);
				sprite.sprite = NewMapSprite(j, i);

				if (tiles[i, j] == Tile.Cookie)
				{
					SpriteRenderer cookieTile = Instantiate(cookiePrefab, position, Quaternion.identity);
					cookieTile.sprite = tileSpriteTable.CookieSprite;
					cookieTile.GetComponent<Cookie>().Init(cookieScore, false);
					cookieTile.transform.SetParent(tileMapBackground.transform);
					cookieCount++;
				}
				else if (tiles[i, j] == Tile.PCookie)
				{
					SpriteRenderer cookieTile = Instantiate(cookiePrefab, position, Quaternion.identity);
					cookieTile.sprite = tileSpriteTable.PCookieSprite;
					cookieTile.GetComponent<Cookie>().Init(pCookieScore, true);
					cookieTile.transform.SetParent(tileMapBackground.transform);
					cookieCount++;
				}
			}
		}
	}

	Sprite NewMapSprite(int x, int y)
	{
		Sprite returnValue;
		Vector2Int coord = new Vector2Int(x, y);

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
		else
		{
			returnValue = obstacleSpriteDictionary[AdjacentObstacle.None];
		}
		return returnValue;
	}

	public bool IsObstacle(Vector2Int coord)
	{
		return tileMap.CheckTileType(coord, Tile.Obstacle) || tileMap.CheckTileType(coord, Tile.PrisonWall);
	}

	public bool IsPrisonEntrance(Vector2Int coord)
	{
		return tileMap.CheckTileType(coord, Tile.Entrance);
	}

	public Vector2Int GetNextTileToVector(Vector2Int current, Vector2Int target, Direction direction, bool canReturn = false)
	{
		List<Vector2Int> path = pathFinder.FindPath(current, target, direction, canReturn);
		if (path.Count > 1)
			return path[1];
		return current;
	}

	public Vector2Int GetNextTileToPlayer(GhostPattern pattern, Vector2Int current, Direction direction, bool canReturn = false)
	{
		if (pacman != null)
		{
			Vector2Int target = new Vector2Int();
			if (pattern == GhostPattern.Blinky)
			{
				target.x = Mathf.RoundToInt(pacman.transform.position.x);
				target.y = Mathf.RoundToInt(pacman.transform.position.y);
				target = CoordInRange(target);
				blinkyPosition = current;
			}
			else if (pattern == GhostPattern.Pinky)
			{
				target.x = Mathf.RoundToInt(pacman.transform.position.x) + Global.directions[(int)pacman.GetDirection()].x * 4;
				target.y = Mathf.RoundToInt(pacman.transform.position.y) + Global.directions[(int)pacman.GetDirection()].y * 4;
			}
			else if (pattern == GhostPattern.Inky)
			{
				target.x = Mathf.RoundToInt(pacman.transform.position.x) + Global.directions[(int)pacman.GetDirection()].x * 2;
				target.y = Mathf.RoundToInt(pacman.transform.position.y) + Global.directions[(int)pacman.GetDirection()].y * 2;
				target += (target - blinkyPosition);
			}
			else if (pattern == GhostPattern.Clyde)
			{
				target.x = Mathf.RoundToInt(pacman.transform.position.x);
				target.y = Mathf.RoundToInt(pacman.transform.position.y);
				if (Mathf.Abs(target.x - current.x) + Mathf.Abs(target.y - current.y) <= 8)
				{
					target = new Vector2Int(1, 1);
				}
			}
			return GetNextTileToVector(current, target, direction, canReturn);
		}
		return current;
	}

	public Vector2Int GetNextTileToPrison(Vector2Int current, Direction direction, bool canReturn = false)
	{
		return GetNextTileToVector(current, tileMap.PrisonPosition(), direction, canReturn);
	}

	public Vector2Int GetNextTileForWander(GhostPattern pattern, Vector2Int current, Direction direction, bool canReturn = false)
	{
		try
		{
			return GetNextTileToVector(current, ghostTargetInVulnerable[pattern], direction, canReturn);
		}
		catch (KeyNotFoundException) { }

		return current;
	}

	public void EatCookie(int cookieScore, bool isPCookie)
	{
		AddScore(cookieScore);

		if (isPCookie)
		{
			eatenGhostCount = 0;
			for (int i = 0; i < ghosts.Length; i++)
			{
				ghosts[i].SetState(GhostState.Vulnerable);
			}
		}

		cookieCount--;
	}

	public void EatGhost(Vector3 position)
	{
		AddScore(ghostScores[eatenGhostCount]);
		Vector2 floatingPosition = gameCamera.WorldToScreenPoint(position);
		floatingPosition.x += gameScreen.rectTransform.offsetMin.x;
		floatingPosition.y += gameScreen.rectTransform.offsetMin.y;
		FloatingText scoreValue = Instantiate(scoreValuePrefab);
		scoreValue.transform.SetParent(canvas.transform);
		scoreValue.transform.localScale = new Vector3(1, 1, 1);
		scoreValue.GetComponent<RectTransform>().anchoredPosition = floatingPosition;
		scoreValue.Print(ghostScores[eatenGhostCount].ToString());

		if (eatenGhostCount + 1 < ghostScores.Length)
			eatenGhostCount++;
	}

	void AddScore(int additionalScore)
	{
		score += additionalScore;
		SetScoreField(scoreField, score);
		if (score > highScore)
		{
			highScore = score;
			PlayerPrefs.SetInt("HighScore", highScore);
			SetScoreField(highScoreField, highScore);
		}
	}

	void SetUiMenuName(UiMenu field)
	{
		for (int i = 0; i < field.name.Length; i++)
		{
			Image newSprite = Instantiate(menuObject, field.parent.transform.position, Quaternion.identity);
			newSprite.transform.SetParent(field.parent.transform);
			newSprite.rectTransform.MoveOnXAxis(-i * (newSprite.rectTransform.sizeDelta.x + 1));
			newSprite.rectTransform.MoveOnYAxis((newSprite.rectTransform.sizeDelta.y + 1));
			newSprite.transform.localScale = new Vector3(1, 1, 1);
			newSprite.sprite = tileSpriteTable.AlphabetSprites[char.ToUpper(field.name[field.name.Length - i - 1]) - 'A'];
		}
	}

	void SetScoreField(UiMenu field, int score)
	{
		int scoreForPrint = score;
		int unit = 1;
		int digit = 0;

		if (field.contentList.Count == 0)
		{
			Image newSprite = NewMenuObject(field);
			newSprite.sprite = tileSpriteTable.NumberSprites[0];
			field.contentList.Add(newSprite);
		}

		while (scoreForPrint / unit > 0)
		{
			if (field.contentList.Count <= digit)
			{
				Image newSprite = NewMenuObject(field);
				field.contentList.Add(newSprite);
				newSprite.rectTransform.MoveOnXAxis(-digit * (newSprite.rectTransform.sizeDelta.x + 1));
			}
			field.contentList[digit].gameObject.SetActive(true);
			field.contentList[digit].sprite = tileSpriteTable.NumberSprites[(scoreForPrint / unit) % 10];
			digit++;
			unit *= 10;
		}

		for (int i = digit; i < field.contentList.Count; i++)
		{
			if (i > 0)
			{
				field.contentList[i].gameObject.SetActive(false);
			}
		}
	}

	void SetLifeField()
	{
		for (int i = 0; i < currentLife || i < lifeField.contentList.Count; i++)
		{
			if (lifeField.contentList.Count <= i)
			{
				Image newSprite = NewMenuObject(lifeField);
				newSprite.sprite = lifeSprite;
				lifeField.contentList.Add(newSprite);
				newSprite.rectTransform.MoveOnXAxis(-i * (newSprite.rectTransform.sizeDelta.x + 1));
			}

			if (i < currentLife)
				lifeField.contentList[i].gameObject.SetActive(true);
			else
				lifeField.contentList[i].gameObject.SetActive(false);
		}
	}

	Image NewMenuObject(UiMenu field)
	{
		Image newSprite = Instantiate(menuObject, field.parent.transform.position, Quaternion.identity);
		newSprite.transform.SetParent(field.parent.transform);
		newSprite.transform.localScale = new Vector3(1, 1, 1);
		return newSprite;
	}

	public bool IsCoordInRange(Vector2Int coord)
	{
		return tileMap.IsCoordInRange(coord);
	}

	public Vector2Int CoordInRange(Vector2Int coord)
	{
		Vector2Int mapSize = tileMap.MapSize();
		Vector2Int result = coord;
		result.x = (result.x + mapSize.x) % mapSize.x;
		result.y = (result.y + mapSize.y) % mapSize.y;
		return result;
	}

	public bool OutOfTileMap(Vector3 position)
	{
		return position.x <= -1 || position.y <= -1 || position.x >= tileMap.MapSize().x || position.y >= tileMap.MapSize().y;
	}

	public Vector3 OppositePosition(Vector3 position)
	{
		Vector3 result = position;
		if (result.x <= -1 || result.x >= tileMap.MapSize().x)
		{
			result.x = tileMap.MapSize().x - position.x - 1;
		}
		if (result.y <= -1 || result.y >= tileMap.MapSize().x)
		{
			result.y = tileMap.MapSize().y - position.y - 1;
		}
		return result;
	}

	public void CheckSecretCode(Direction direction)
	{
		if (direction == secretCode[secretIndex])
		{
			secretIndex++;
		}
		else
		{
			secretIndex = 0;
		}

		if (secretIndex >= secretCode.Length)
		{
			PowerOverwhelming = !PowerOverwhelming;
			if (PowerOverwhelming)
			{
				logView.text = "Cheat Mode On";
			}
			else
			{
				logView.text = "";
			}
			secretIndex = 0;
		}
	}
}
