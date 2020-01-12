using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cookie : MonoBehaviour
{
	int score;
	bool isPCookie;

	public void Init(int _score, bool _isPCookie)
	{
		score = _score;
		isPCookie = _isPCookie;
	}

	public void Collide()
	{
		InGameManager.Instance.EatCookie(score, isPCookie);
	}
}
