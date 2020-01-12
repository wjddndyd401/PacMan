using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
	Direction direction;
	Direction newDirection;
	Vector2Int targetPosition;
	Vector2Int currentPosition;
	[SerializeField] float speed = 1.0f;
	Animator animator;
	bool isDeath;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public void Init()
	{
		direction = Direction.Right;
		newDirection = direction;
		RotateToDirection();
		isDeath = false;

		Vector3 position = transform.position;
		currentPosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
		position.x = currentPosition.x;
		position.y = currentPosition.y;
		transform.position = position;
		targetPosition = currentPosition;
	}

    void Update()
	{
		if (!isDeath)
		{
			SetDirectionByInput();
			RotateToDirection();
			Move();
		}
		else
		{
			AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
			if (info.IsName("Death") && info.normalizedTime >= 1.0f)
			{
				gameObject.SetActive(false);
			}
		}
	}

	public void Move()
	{
		Vector2Int newTarget = InGameManager.Instance.CoordInRange(currentPosition + Global.directions[(int)newDirection]);
		Vector2 position = transform.position;

		if (InGameManager.Instance.IsCoordInRange(currentPosition)
			&& (Global.Opposition(direction) == newDirection
			|| (position.Approximately(currentPosition) && !InGameManager.Instance.IsObstacle(newTarget) && !InGameManager.Instance.IsPrisonEntrance(newTarget))))
		{
			direction = newDirection;
			targetPosition = currentPosition;
		}
		else
		{
			targetPosition = InGameManager.Instance.CoordInRange(currentPosition + Global.directions[(int)direction]);
		}

		if (!InGameManager.Instance.IsObstacle(targetPosition))
		{
			if (targetPosition - currentPosition == Global.directions[(int)direction])
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
				currentPosition = targetPosition - Global.directions[(int)direction];
			}

			if (position.Approximately(targetPosition))
			{
				currentPosition = targetPosition;
			}
		}
	}

	public void SetDirectionByInput()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			newDirection = Direction.Up;
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			newDirection = Direction.Right;
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			newDirection = Direction.Down;
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			newDirection = Direction.Left;
		}
	}

	public void RotateToDirection()
	{
		Vector3 rotation = transform.rotation.eulerAngles;
		if (direction == Direction.Up)
		{
			rotation.z = 90;
		}
		else if (direction == Direction.Right)
		{
			rotation.y = 0;
			rotation.z = 0;
		}
		else if (direction == Direction.Down)
		{
			rotation.z = -90;
		}
		else if (direction == Direction.Left)
		{
			rotation.y = 180;
			rotation.z = 0;
		}
		transform.rotation = Quaternion.Euler(rotation);
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Cookie"))
		{
			other.GetComponent<Cookie>().Collide();
			Destroy(other.gameObject);
		} else if(other.CompareTag("Ghost"))
		{
			Ghost ghost = other.GetComponent<Ghost>();
			if (ghost.CanKillPacman())
			{
				isDeath = true;
				animator.SetTrigger("Death");
			} else
			{
				ghost.SetState(GhostState.Death);
				InGameManager.Instance.EatGhost(Camera.main.WorldToScreenPoint(transform.position));
			}
		}
	}

	public Direction GetDirection()
	{
		return direction;
	}
}
