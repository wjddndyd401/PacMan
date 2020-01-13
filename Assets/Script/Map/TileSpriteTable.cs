using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile Sprite", menuName = "Scriptable Object/Tile Sprite", order = int.MaxValue)]
public class TileSpriteTable : ScriptableObject
{
	[SerializeField] Sprite none = null;
	public Sprite NoneSprite { get { return none; } }
	[SerializeField] ObstacleSprite[] obstacleSprites = null;
	public ObstacleSprite[] ObstacleSprites { get { return obstacleSprites; } }
	[SerializeField] ObstacleSprite[] prisonSprites = null;
	public ObstacleSprite[] PrisonSprites { get { return prisonSprites; } }
	[SerializeField] Sprite prisonEntranceSprite = null;
	public Sprite PrisonEntranceSprite { get { return prisonEntranceSprite; } }
	[SerializeField] Sprite cookie = null;
	public Sprite CookieSprite { get { return cookie; } }
	[SerializeField] Sprite pCookie = null;
	public Sprite PCookieSprite { get { return pCookie; } }
	[SerializeField] Sprite player = null;
	public Sprite PlayerSprite { get { return player; } }
	[SerializeField] Sprite[] alphabetSprites = null;
	public Sprite[] AlphabetSprites { get { return alphabetSprites; } }
	[SerializeField] Sprite[] numberSprites = null;
	public Sprite[] NumberSprites { get { return numberSprites; } }
}
