﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using System;


// Written By Abdalla Tawfik & Fehr Hassan


public class GamePlayUIController : MonoBehaviour {

	#region Variables Declaration

	// A reference to the Player Game Object.
	public GameObject player;

	// Game Score (steps).
	[HideInInspector]
	public int score;

	// Starting Menu - Shown before the game is started.
	public GameObject startMenu;
	// Game Menu - Shown during the game is played.
	public GameObject gameMenu;
	// Game Over Menu - Shown after the game is over.
	public GameObject gameOverMenu;

	// Current Score UI Texts (On Game menu and Game Over menu).
	public Text[] scoreTexts;
	// Best Score UI Texts (On Starting menu and Game Over menu).
	public Text[] bestScoreTexts;
	// Results screen - game over Texts
	public Text[] gameOverTexts;
	// Game Name UI Text
	public Text gameNameText;
	// Pop Up Text
	public GameObject floatingTextPrefab;

	// A reference to the Player script.
	public PlayerController playerScript;

	public Button settingsButton;
	public Sprite settingsUnPressed;
	public Sprite settingsPressed;
	public Sprite settingsDarkUnPressed;
	public Sprite settingsDarkPressed;

	public Image handImage;
	public Sprite handSprite;
	public Sprite handDarkSprite;

	public Button submitButton;
	public Sprite submitUnPressed;
	public Sprite submitPressed;
	public Sprite submitDarkUnPressed;
	public Sprite submitDarkPressed;

	public Camera camera;  // used for background colors
	public Color [] colors;

	// Debug
	public GameObject debugMenu;
	public Button debugButton;
	public Text sliderText;
	public Slider slider;

	// Post-process blur
	 public PostProcessVolume postProcessVolume;


	#endregion



	#region Unity Callbacks

	void Awake ()
	{
		// Activate the Start menu Only and deactivate the others.
		startMenu.SetActive (true);
		gameMenu.SetActive (false);
		gameOverMenu.SetActive (false);


		// Update Best Score Texts.
		int bestScore = PlayerPrefs.GetInt("Best Score");
		updateBestScoreUITexts (bestScore);

		// Update debug slider speed
		float sliderSave = PlayerPrefs.GetFloat("Slider Speed");
		updateSliderUITexts (sliderSave);

		// Update colors for dark Model
		updateDarkMode();
	}

	#endregion

	#region Debug section

	// Toggle Debug Menu
	public void toggleDebugMenu()
	{
		if (debugMenu.activeInHierarchy)
		{
			debugMenu.SetActive(false);
		}else{
			debugMenu.SetActive(true);
		}
	}

	#endregion


	#region Game States Methods

	public void onGameStarted ()
	{
		// Activate the Game menu Only and deactivate the others.
		startMenu.SetActive (false);
		gameMenu.SetActive (true);
		gameOverMenu.SetActive (false);

		// Increment the number of Games Played.
		int gamesPlayed = PlayerPrefs.GetInt ("GamesPlayed");
		gamesPlayed++;
		PlayerPrefs.SetInt ("GamesPlayed", gamesPlayed);
	}

	public void onGameOver ()
	{
		// Update Best Score Texts according to the new Score.
		int bestScore = PlayerPrefs.GetInt("Best Score");
		if (score > bestScore)
		{
			PlayerPrefs.SetInt ("Best Score", playerScript.score);
			updateBestScoreUITexts (playerScript.score);
		}
		else
		{
			updateBestScoreUITexts (bestScore);
		}


		// Activate the Game Over menu Only and deactivate the others.
		startMenu.SetActive (false);
		gameMenu.SetActive (false);
		gameOverMenu.SetActive (true);
		DepthOfField depthOfField;
		postProcessVolume.profile.TryGetSettings(out depthOfField);
		if (depthOfField != null) {depthOfField.enabled.Override(true);}

	}

	#endregion



	#region Updating UI Elements Data

	public void updateScoreUITexts ()
	{
		// Update all Score UI Texts with the current score.
		for (int i = 0; i < scoreTexts.Length; i++)
		{
			scoreTexts [i].text = "" + score;
		}
	}

	public void updateBestScoreUITexts (int bestScore)
	{
		// Update all Best Score UI Texts with the best score.
		for (int i = 0; i < bestScoreTexts.Length; i++)
		{
			bestScoreTexts[i].text = "BEST: " + bestScore;
		}
	}

	public void updateSliderUITexts (float speed)
	{
		// Update the debug slider Speed text
		sliderText.text = speed.ToString("0.00");
	}
	public void updateSliderUISlider (float value)
	{
		// Update the slider value
		slider.value = value;
	}
	public void ShowFloatingText(GameObject g, String copy, Color color)
	{
		var go = Instantiate(floatingTextPrefab, g.transform.position, Quaternion.identity);
		go.GetComponent<TextMesh>().color = color;
		go.GetComponent<TextMesh>().text = copy;
	}

	void updateDarkMode()
	{
		string darkMode = PlayerPrefs.GetString("dark");
		if (darkMode == "Off")
		{
			changeButtonSprites(settingsButton, settingsUnPressed, settingsPressed);
			handImage.sprite = handSprite;
			// set background color
			camera.backgroundColor = colors[0];
			// set both score and best score texts for menu and game over menu
			// color should always be opposite of background
			for (int i = 0; i < scoreTexts.Length; i++)
			{
				scoreTexts[i].color = colors[1];
				bestScoreTexts[i].color = colors[1];
				gameOverTexts[i].color = colors[1];
			}
			// set game name text color -- always opposite of background
			gameNameText.color = colors[1];
			// set submit button colors
			changeButtonSprites(submitButton, submitUnPressed, submitPressed);
		}
		else
		{
			changeButtonSprites(settingsButton, settingsDarkUnPressed, settingsDarkPressed);
			handImage.sprite = handDarkSprite;
			// set background color
			camera.backgroundColor = colors[1];
			// set both score and best score texts for menu and game over menu
			// color should always be opposite of background
			for (int i = 0; i < scoreTexts.Length; i++)
			{
				scoreTexts[i].color = colors[0];
				bestScoreTexts[i].color = colors[0];
				gameOverTexts[i].color = colors[0];
			}
			// set game name text color -- always opposite of background
			gameNameText.color = colors[0];
			// set submit button colors
			changeButtonSprites(submitButton, submitDarkUnPressed, submitDarkPressed);
		}
	}

	void changeButtonSprites(Button button, Sprite unpressedImage, Sprite pressedImage)
	{
		// Update the sprites of the image component and the pressedSprite of a Button.

		button.GetComponent<Image>().sprite = unpressedImage;

		SpriteState st = new SpriteState();
		st.pressedSprite = pressedImage;
		button.spriteState = st;
  }



	#endregion



	#region UI Animation

	IEnumerator animatePanel (RectTransform animatingPanel)
	{
		// Calculate the Canvas Size.
		Vector2 canvasSize = GetComponent<RectTransform> ().sizeDelta;

		// Move the Animating Panel to the left of the screen.
		animatingPanel.anchoredPosition = new Vector3 (-canvasSize.x, animatingPanel.anchoredPosition.y, 0);

		// Target Position to be reached after Animation is finished.
		Vector3 targetPosition = new Vector3 (0, animatingPanel.anchoredPosition.y, 0);

		// Animation step in X Axis.
		float xPosStep = 20;

		while (true)
		{
			// Add a step in X position each frame.
			animatingPanel.anchoredPosition = new Vector3 (animatingPanel.anchoredPosition.x + xPosStep, animatingPanel.anchoredPosition.y, 0);

			if (animatingPanel.anchoredPosition.x >= targetPosition.x)
			{
				// Target is Reached or exceeded.

				// Set the position of the Panel with the Targeted Position.
				animatingPanel.anchoredPosition = targetPosition;

				// Stop the Coroutine.
				yield break;
			}

			// Otherwise, continue next frame
			yield return null;
		}
	}

	#endregion



	#region Buttons onClicked Methods

	public void onSettingsButtonClicked ()
	{
		string currentScene = SceneManager.GetActiveScene().name;
		PlayerPrefs.SetString("LastScene", currentScene);
		PlayerPrefs.Save();
		// Load Settings Scene.
		SceneManager.LoadScene ("Settings");
	}

	public void onModelShopButtonClicked ()
	{
		// Load Models Shop Scene.
	}

	public void onSubmitButtonClicked ()
	{
		SceneManager.LoadScene ("GamePlay");
	}

	#endregion
}
