using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] int maxLife = 3;
	int score = 0;
	[SerializeField] int currentStage = 1;
	[SerializeField] float loadingTime = 3.0f;

	public int MaxLife { get { return maxLife; } private set { maxLife = value; } }
	public int CurrentStage { get { return currentStage; } private set { currentStage = value; } }
	public float LoadingTime { get { return loadingTime; } private set { loadingTime = value; } }

	public static GameManager Instance { get; private set; } = null;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);

		DontDestroyOnLoad(this);
	}

	public void GoToNextStage()
	{
		currentStage++;
	}

	public void Restart()
	{
		currentStage = 1;
	}
}
