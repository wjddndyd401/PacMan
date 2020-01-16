using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
	Direction direction;
	Animator animator;
	SpriteRenderer spriteRenderer;
	[SerializeField] float speed = 1.0f;
	[SerializeField] float vulnerableDuration = 3.0f;
	[SerializeField] float warningTime = 2.0f;
	[SerializeField] GhostPattern pattern = GhostPattern.Pinky;
	float aliveTime;

	Vector2Int targetPosition;
	Vector2Int currentPosition;

	GhostState state;

	bool canReturn;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void Init()
	{
		direction = Direction.Right;
		canReturn = false;

		Vector3 position = transform.position;
		currentPosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
		position.x = currentPosition.x;
		position.y = currentPosition.y;
		transform.position = position;
		targetPosition = currentPosition;

		SetState(GhostState.Normal);
	}

	void Update()
	{
		Vector2 position = transform.position;
		if (position.Approximately(currentPosition))
		{
			FindNextPosition();
		}

		if (state == GhostState.Vulnerable)
		{
			Blink();
		}

		Move();
	}

	void Move()
	{
		if (currentPosition != targetPosition)
		{
			Vector2 position = transform.position;
			animator.SetFloat("MoveX", Global.directions[(int)direction].x);
			animator.SetFloat("MoveY", Global.directions[(int)direction].y);
			
			for (int i = 0; i < Global.directions.Length; i++)
			{
				if (InGameManager.Instance.CoordInRange(currentPosition + Global.directions[i]) == targetPosition)
					direction = (Direction)i;
			}

			if ((targetPosition - position).GetDirection() == direction)
			{
				position = Vector2.MoveTowards(position, targetPosition, Time.deltaTime * speed);
			}
			else
			{
				position = Vector2.MoveTowards(position, position + Global.directions[(int)direction], Time.deltaTime * speed);
			}

			transform.position = position;
			if (InGameManager.Instance.OutOfTileMap(transform.position))
			{
				transform.position = InGameManager.Instance.OppositePosition(transform.position);
			}

			if(position.Approximately(targetPosition))
			{
				currentPosition = targetPosition;
			}
		}
	}

	void FindNextPosition()
	{
		if (state == GhostState.Normal) {
			targetPosition = InGameManager.Instance.GetNextTileToPlayer(pattern, currentPosition, direction, canReturn);
		}
		else if (state == GhostState.Vulnerable)
		{
			targetPosition = InGameManager.Instance.GetNextTileForWander(pattern, currentPosition, direction, canReturn);
		}
		else
		{
			targetPosition = InGameManager.Instance.GetNextTileToPrison(currentPosition, direction, canReturn);
			if (InGameManager.Instance.IsPrisonEntrance(currentPosition))
			{
				SetState(GhostState.Normal);
			}
		}

		if (targetPosition == currentPosition)
			targetPosition = FindNextTileForStraight();

		canReturn = false;
	}

	Vector2Int FindNextTileForStraight()
	{
		Direction[] orderedDirections = new Direction[(int)Direction.End];
		orderedDirections[0] = direction;
		if (Random.Range(0, 2) == 0)
		{
			orderedDirections[1] = (Direction)((int)(direction + 1) % (int)Direction.End);
			orderedDirections[2] = (Direction)((int)(direction + (int)Direction.End - 1) % (int)Direction.End);
		}
		else
		{
			orderedDirections[1] = (Direction)((int)(direction + (int)Direction.End - 1) % (int)Direction.End);
			orderedDirections[2] = (Direction)((int)(direction + 1) % (int)Direction.End);
		}
		orderedDirections[3] = Global.Opposition(direction);

		for (int i = 0; i < orderedDirections.Length; i++)
		{
			Vector2Int newTarget = InGameManager.Instance.CoordInRange(currentPosition + Global.directions[(int)orderedDirections[i]]);
			if (!InGameManager.Instance.IsObstacle(newTarget))
			{
				direction = orderedDirections[i];
				return newTarget;
			}
		}
		return currentPosition;
	}

	public void SetState(GhostState _state)
	{
		if (_state == GhostState.Normal)
		{
			if (state != GhostState.Normal)
				animator.SetTrigger("Alive");
			GetComponent<Collider>().enabled = true;
		}
		else if (state != GhostState.Death && _state == GhostState.Vulnerable)
		{
			if (state == GhostState.Normal)
				animator.SetTrigger("Vulnerable");
			aliveTime = Time.time + vulnerableDuration;
		}
		else if (_state == GhostState.Death)
		{
			animator.SetTrigger("Death");
			GetComponent<Collider>().enabled = false;
		}
		else
		{
			return;
		}
		state = _state;

		canReturn = true;
		Color color = spriteRenderer.color;
		color.a = 1.0f;
		spriteRenderer.color = color;
	}

	public bool CanKillPacman()
	{
		return state == GhostState.Normal;
	}

	void Blink()
	{
		if (aliveTime - Time.time < warningTime)
		{
			Color color = spriteRenderer.color;
			if (color.a >= 0.5f)
				color.a = 0.0f;
			else
				color.a = 1.0f;
			spriteRenderer.color = color;
		}
		if (aliveTime <= Time.time)
		{
			SetState(GhostState.Normal);
		}
	}
}
