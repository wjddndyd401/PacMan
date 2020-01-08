using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Node
{
	public int x;
	public int y;
	public int f;
	public int g;
	public int h;
	public bool isOpened;
	public int parentX;
	public int parentY;
}

public class PathFinder
{
	Node[,] nodes;
	List<Node> queue;
	Tile[,] tiles;
	public PathFinder(Tile[,] _tiles)
	{
		tiles = _tiles;
		nodes = new Node[tiles.GetLength(0), tiles.GetLength(1)];
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
				nodes[i, j].g = -1;
				nodes[i, j].isOpened = !(tiles[i, j] == Tile.Obstacle || tiles[i, j] == Tile.Prison);
				nodes[i, j].parentX = j;
				nodes[i, j].parentY = i;
			}
		}
	}

	public List<Vector2Int> FindPath(int startX, int startY, int endX, int endY)
	{
		Reset();

		int x = startX;
		int y = startY;
		Enqueue(startX, startY, -1, startX, startY, endX, endY);

		while (queue.Count != 0)
		{
			Node node = Dequeue();
			x = node.x; y = node.y;
			nodes[y, x].isOpened = false;

			if (x == endX && y == endY)
				break;

			Enqueue(x, y + 1, node.g, x, y, endX, endY);
			Enqueue(x + 1, y, node.g, x, y, endX, endY);
			Enqueue(x, y - 1, node.g, x, y, endX, endY);
			Enqueue(x - 1, y, node.g, x, y, endX, endY);
		}

		List<Vector2Int> result = new List<Vector2Int>();
		if (x == endX && y == endY)
		{
			result.Insert(0, new Vector2Int(x, y));
			while (true)
			{
				Node node = nodes[y, x];
				if (node.parentX == x && node.parentY == y)
					break;
				result.Insert(0, new Vector2Int(x, y));
				x = node.parentX;
				y = node.parentY;
			}
		}
		return result;
	}

	void Enqueue(int x, int y, int g, int parentX, int parentY, int endX, int endY)
	{
		if (IsIndexInRange(x, y) && nodes[y, x].isOpened)
		{
			bool isInQueue = nodes[y, x].g >= 0;
			nodes[y, x].x = x;
			nodes[y, x].y = y;
			if (nodes[y, x].g < 0 || nodes[y, x].g > g)
			{
				nodes[y, x].g = g + 1;
				nodes[y, x].parentX = parentX;
				nodes[y, x].parentY = parentY;
			}
			nodes[y, x].h = Mathf.Abs(endX - x) + Mathf.Abs(endY - y);
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

	bool IsIndexInRange(int x, int y)
	{
		return x >= 0 && y >= 0 && x < nodes.GetLength(1) && y < nodes.GetLength(0);
	}
}
