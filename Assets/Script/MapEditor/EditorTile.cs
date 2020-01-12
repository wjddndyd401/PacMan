using UnityEngine;
using System.Collections;

public class EditorTile : MonoBehaviour
{
    Vector2Int coord;
    public Vector2Int Coord { get { return coord; } }

    public void Init(Vector2Int _coord)
    {
        coord = _coord;
    }
}
