using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
	[SerializeField] GameObject mainMenu = null;
	[SerializeField] GameObject option = null;
	[SerializeField] ToggleGroup optionGroup = null;
	[SerializeField] OptionToggle[] toggles = null;

	private void Start()
	{
		OpenMainMenu();
	}

	public void OpenMainMenu()
	{
		mainMenu.SetActive(true);
		option.SetActive(false);
	}

	public void OpenOption()
	{
		mainMenu.SetActive(false);
		option.SetActive(true);
	}

	public void StartGame()
	{
		SceneManager.LoadScene("GameScene");
	}
}
