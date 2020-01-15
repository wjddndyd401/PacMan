using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
	Direction direction;
	Direction newDirection;
	Vector2Int targetCoord;
	Vector2Int currentCoord;
	[SerializeField] float speed = 1.0f;
	Animator animator;
	bool isDeath;
	Collider mCollider;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		mCollider = GetComponent<Collider>();
	}

	public void Init()
	{
		direction = Direction.Right;
		newDirection = direction;
		RotateToDirection();
		isDeath = false;

		Vector3 position = transform.position;
		currentCoord = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
		position.x = currentCoord.x;
		position.y = currentCoord.y;
		transform.position = position;
		targetCoord = currentCoord;

		mCollider.enabled = true;
	}

    void Update()
	{
		if (!isDeath)
		{
			SetDirectionByInput();
			RotateToDirection();
			SetCoordForMove();
			MoveFromCurrentToTargetCoord();
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

	public void SetCoordForMove()
	{
		Vector2 position = transform.position;
		if (Global.Opposition(direction) == newDirection)
		{
			direction = newDirection;
			Vector2Int swapTemp = targetCoord;
			targetCoord = InGameManager.Instance.CoordInRange(currentCoord);
			currentCoord = swapTemp;
		}
		else if (position.Approximately(targetCoord))
		{
			Vector2Int currentTarget = InGameManager.Instance.CoordInRange(currentCoord + Global.directions[(int)direction]);
			Vector2Int newTarget = InGameManager.Instance.CoordInRange(targetCoord + Global.directions[(int)newDirection]);
			if (!InGameManager.Instance.IsObstacle(newTarget) && !InGameManager.Instance.IsPrisonEntrance(newTarget))
			{
				direction = newDirection;
				currentCoord = targetCoord;
				targetCoord = newTarget;
			}
			else if (!InGameManager.Instance.IsObstacle(currentTarget) && !InGameManager.Instance.IsPrisonEntrance(currentTarget))
			{
				currentCoord = targetCoord;
				targetCoord = currentTarget;
			}
		}
	}

	public void MoveFromCurrentToTargetCoord()
	{
		Vector2 position = transform.position;
		if ((targetCoord - position).normalized == Global.directions[(int)direction])
		{
			position = Vector2.MoveTowards(position, targetCoord, Time.deltaTime * speed);
		}
		else if (!position.Approximately(targetCoord))
		{
			position = Vector2.MoveTowards(position, position + Global.directions[(int)direction], Time.deltaTime * speed);
		}
		transform.position = position;

		if (InGameManager.Instance.OutOfTileMap(transform.position))
		{
			transform.position = InGameManager.Instance.OppositePosition(transform.position);
		}
	}

	public void SetDirectionByInput()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			SetDirection(Direction.Up);
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			SetDirection(Direction.Right);
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			SetDirection(Direction.Down);
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			SetDirection(Direction.Left);
		}
	}

	public void SetDirection(Direction direction)
	{
		newDirection = direction;
		InGameManager.Instance.CheckSecretCode(newDirection);
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
				if (!InGameManager.Instance.PowerOverwhelming)
				{
					isDeath = true;
					animator.SetTrigger("Death");
				}
			} else
			{
				ghost.SetState(GhostState.Death);
				InGameManager.Instance.EatGhost(transform.position);
			}
		}
	}

	public Direction GetDirection()
	{
		return direction;
	}

	public void SetToWaitMode()
	{
		mCollider.enabled = false;
	}

	public void SetToGameMode()
	{
		mCollider.enabled = true;
	}
}
