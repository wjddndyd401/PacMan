using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Node
{
	public Vector2Int coord;
	public int f;
	public int g;
	public int h;
	public bool isOpened;
	public Vector2Int parent;
}

public class PathFinder
{
	Node[,] nodes;
	List<Node> queue;
	TileMap tileMap;
	int xLength, yLength;

	public PathFinder(TileMap _tileMap)
	{
		tileMap = _tileMap;
		Tile[,] tiles = tileMap.GetTiles();
		nodes = new Node[tiles.GetLength(0), tiles.GetLength(1)];
		xLength = tiles.GetLength(1);
		yLength = tiles.GetLength(0);
		queue = new List<Node>();
		Reset();
	}

	public void Reset()
	{
		queue.Clear();
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				Vector2Int coord = new Vector2Int(j, i);
				nodes[i, j].isOpened = !(tileMap.CheckTileType(coord, Tile.Obstacle) || tileMap.CheckTileType(coord, Tile.PrisonWall));
				nodes[i, j].g = -1;
				nodes[i, j].parent.x = j;
				nodes[i, j].parent.y = i;
			}
		}
	}

	public List<Vector2Int> FindPath(Vector2Int current, Vector2Int target, Direction direction = Direction.End, bool canReturn = false)
	{
		Reset();

		Vector2Int nodeCoord = current;
		Enqueue(current, -1, current, target);

		if (!canReturn)
		{
			Vector2Int behindDirection = Global.directions[(int)Global.Opposition(direction)];
			Vector2Int behind = InGameManager.Instance.CoordInRange(current + behindDirection);
			nodes[behind.y, behind.x].isOpened = false;
		}

		int minDistance = Mathf.Abs(current.x - target.x) + Mathf.Abs(current.y - target.y);
		Vector2Int closestPoint = current;
		while (queue.Count != 0)
		{
			Node node = Dequeue();
			nodeCoord = node.coord;
			nodes[nodeCoord.y, nodeCoord.x].isOpened = false;

			if (nodeCoord == target)
				break;

			int tileDistance = Mathf.Abs(nodeCoord.x - target.x) + Mathf.Abs(nodeCoord.y - target.y);
			if(minDistance > tileDistance)
			{
				minDistance = tileDistance;
				closestPoint = nodeCoord;
			}

			Enqueue(new Vector2Int(nodeCoord.x, nodeCoord.y + 1), node.g, nodeCoord, target);
			Enqueue(new Vector2Int(nodeCoord.x + 1, nodeCoord.y), node.g, nodeCoord, target);
			Enqueue(new Vector2Int(nodeCoord.x, nodeCoord.y - 1), node.g, nodeCoord, target);
			Enqueue(new Vector2Int(nodeCoord.x - 1, nodeCoord.y), node.g, nodeCoord, target);
		}

		List<Vector2Int> result = new List<Vector2Int>();
		if (nodeCoord == target)
		{
			result.Insert(0, nodeCoord);
		} else if(closestPoint != current)
		{
			result.Insert(0, closestPoint);
		}

		while (result.Count > 0)
		{
			Node node = nodes[result[0].y, result[0].x];
			if (node.parent == node.coord)
				break;
			result.Insert(0, node.parent);
		}
		return result;
	}

	void Enqueue(Vector2Int coord, int g, Vector2Int parent, Vector2Int target)
	{
		int x = (coord.x + nodes.GetLength(1)) % nodes.GetLength(1);
		int y = (coord.y + nodes.GetLength(0)) % nodes.GetLength(0);
		if (IsCoordInRange(new Vector2Int(x, y)) && nodes[y, x].isOpened)
		{
			bool isInQueue = nodes[y, x].g >= 0;
			nodes[y, x].coord.x = x;
			nodes[y, x].coord.y = y;
			if (nodes[y, x].g < 0 || nodes[y, x].g > g)
			{
				nodes[y, x].g = g + 1;
				nodes[y, x].parent = parent;
			}
			nodes[y, x].h = Global.GetTileDistance(new Vector2Int(x, y), target, new Vector2Int(xLength, yLength));
			nodes[y, x].f = nodes[y, x].g + nodes[y, x].h;

			int i = queue.Count;
			while (i > 0 && queue[i - 1].f > nodes[y, x].f)
			{
				i--;
			}
			if(!isInQueue)
				queue.Insert(i, nodes[y, x]);
		}
	}

	Node Dequeue()
	{
		Node result = queue[0];
		queue.RemoveAt(0);
		return result;
	}

	public bool IsCoordInRange(Vector2Int coord)
	{
		return coord.x >= 0 && coord.y >= 0 && coord.x < nodes.GetLength(1) && coord.y < nodes.GetLength(0);
	}
}
