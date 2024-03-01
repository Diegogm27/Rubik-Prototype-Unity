using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
	private VisualElement _rootElement;
	private UIDocument _pauseMenu;

	private void Awake()
	{
		if (GetComponent<UIDocument>() != null)		_rootElement = GetComponent<UIDocument>().rootVisualElement;
		if (GameObject.Find("PauseMenu") != null)	_pauseMenu = GameObject.Find("PauseMenu").GetComponent<UIDocument>();

		Button startButton		=	_rootElement.Q<Button>("StartButton");
		Button resumeButton		=	_rootElement.Q<Button>("ResumeButton");
		Button restartButton	=	_rootElement.Q<Button>("RestartButton");
		Button menuButton		=	_rootElement.Q<Button>("MenuButton");
		Button openMenuButton	=	_rootElement.Q<Button>("OpenMenuButton");
		Button exitButton		=	_rootElement.Q<Button>("ExitButton");

		if (startButton != null)	startButton.clicked += StartButtonPressed;
		if (restartButton != null)	restartButton.clicked += StartButtonPressed;
		if (resumeButton != null)	resumeButton.clicked += ResumeButtonPressed;
		if (menuButton != null)		menuButton.clicked += MenuButtonPressed;
		if (exitButton != null)		exitButton.clicked += ExitButtonPressed;
		if (openMenuButton != null)	openMenuButton.clicked += OpenMenuButtonPressed;
	}

	void StartButtonPressed()
	{
		SceneManager.LoadScene("MainScene");
	}
	
	private void OpenMenuButtonPressed()
	{
		_pauseMenu.rootVisualElement.style.display = DisplayStyle.Flex;
		GameObject.Find("InputController").GetComponent<InputController>().playerInputEnabled = false;
	}

	void ResumeButtonPressed()
	{
		_pauseMenu.rootVisualElement.style.display = DisplayStyle.None;
        GameObject.Find("InputController").GetComponent<InputController>().playerInputEnabled = true;
	}

	void MenuButtonPressed()
	{
		SceneManager.LoadScene("MainMenu");
	}

	private void ExitButtonPressed()
	{
		Application.Quit();
	}
}