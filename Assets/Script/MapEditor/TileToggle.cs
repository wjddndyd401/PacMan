using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileToggle : MonoBehaviour
{
    [SerializeField] Tile tile = Tile.Empty;
    public Tile ToogleTile { get { return tile; } }
}
