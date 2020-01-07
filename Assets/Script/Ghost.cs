using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
	float directionChangeTime;
	Animator animator;
	[SerializeField] float speed = 1.0f;

	Vector2Int targetPosition;
	Vector2Int currentPosition;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	void Start()
    {
		directionChangeTime = Time.time;

		Vector3 position = transform.position;
		currentPosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
		position.x = currentPosition.x;
		position.y = currentPosition.y;
		transform.position = position;
		targetPosition = currentPosition;
	}

    void Update()
	{
		Vector2 position = transform.position;

		if (position.Approximately(currentPosition))
		{
			targetPosition = GameManager.Instance.GetNextTileToPlayer(currentPosition.x, currentPosition.y);
		}

		if (currentPosition != targetPosition)
		{
			animator.SetFloat("MoveX", targetPosition.x - position.x);
			animator.SetFloat("MoveY", targetPosition.y - position.y);
			position = Vector2.MoveTowards(position, targetPosition, Time.deltaTime * speed);
			transform.position = position;
			if (position.Approximately(targetPosition))
			{
				currentPosition = targetPosition;
			}
		}
    }

	Vector2Int GetNextTileForWander()
	{
		while (true)
		{
			Direction newDirection = (Direction)Random.Range(0, (int)Direction.End);

			Vector2Int newTarget = Vector2Int.zero;
			newTarget.x = currentPosition.x + (int)Global.direction[(int)newDirection].x;
			newTarget.y = currentPosition.y + (int)Global.direction[(int)newDirection].y;

			if (!GameManager.Instance.IsObstacle(newTarget.x, newTarget.y))
			{
				return newTarget;
			}
		}
	}
}
