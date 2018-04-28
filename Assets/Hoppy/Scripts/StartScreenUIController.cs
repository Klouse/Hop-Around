using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


// Written By Abdalla Tawfik & Fehr Hassan


public class StartScreenUIController : MonoBehaviour {

	#region Variables Declaration

	// A reference to the Player Game Object.
	public GameObject player;

	// Game Score (steps).
	[HideInInspector]
	public int score;

	// A reference to Number of Gems UI Text.
	public Text numberOfGemsText;

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
	// Game Name UI Text
	public Text gameNameText;

	// A reference to all game over panels (Rewarded Video, Free Gifts, New Model, Rate Us and Promotion).
	public RectTransform[] gameOverPanels;
	// A reference to the No Enough Gems Group of Elemnts (On New Model Panel).
	public GameObject noEnoughGems;
	// A reference to the Enough Gems Group of Elemnts (On New Model Panel).
	public GameObject EnoughGems;
	// A reference to Number of Remaining Gems UI Text.
	public Text numberOfRemainingGemsText;

	public Button settingsButton;
	public Sprite settingsUnPressed;
	public Sprite settingsPressed;
	public Sprite settingsDarkUnPressed;
	public Sprite settingsDarkPressed;

	public Button submitButton;
	public Sprite submitUnPressed;
	public Sprite submitPressed;
	public Sprite submitDarkUnPressed;
	public Sprite submitDarkPressed;

	public Button playButton;
	public Sprite playSprite;
	public Sprite playDarkSprite;

	public Camera camera;  // used for background colors
	public Color [] colors;

	#endregion



	#region Unity Callbacks

	void Start()
	{
		checkLaunchStatus ();
	}

	void Awake ()
	{
		// Activate the Start menu Only and deactivate the others.
		startMenu.SetActive (true);
		gameMenu.SetActive (false);
		gameOverMenu.SetActive (false);


		// Update Best Score Texts.
		int bestScore = PlayerPrefs.GetInt("Best Score");
		updateBestScoreUITexts (bestScore);

		// Update Number of Gems displayed.
		updateNumberOfGemsUITexts ();

		// Update colors for dark Model
		updateDarkMode();
	}

	#endregion

	#region Supporting Methods

	void checkLaunchStatus()
	{
		string hasLaunchedBefore = PlayerPrefs.GetString("HasLaunchedBefore");
		if (hasLaunchedBefore != "Yes")
		{
			// First time to launch the Game!
			// Initialize All Player Preferences.

			// Some of the following Player Preference may be very useful for analytics data.

			// A string Shows if the game has been launched before or not.
			PlayerPrefs.SetString("HasLaunchedBefore", "Yes");

			// An integer shows how many times the player has launched the game.
			PlayerPrefs.SetInt("LaunchCounter", 1);
			// An integer shows how many times the player has played.
			PlayerPrefs.SetInt("GamesPlayed", 0);
			// A string shows if the user has enabled the sound or not.
			PlayerPrefs.SetString("sound", "On");
			// An integer shows the best score the player got so far.
			PlayerPrefs.SetInt("Best Score", 0);
			// An integer shows how many pick ups the player has.
			PlayerPrefs.SetInt("NumberOfPickUps", 0);
			// Dark mode toggle -- starts off
			PlayerPrefs.SetString("dark", "Off");
		}
		else
		{
			// Game has been launched before!

			// Increasing number of Lanuches.
			int launchCounter = PlayerPrefs.GetInt("LaunchCounter");
			launchCounter++;
			PlayerPrefs.SetInt("LaunchCounter", launchCounter);
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
			PlayerPrefs.SetInt ("Best Score", score);
			updateBestScoreUITexts (score);
		}
		else
		{
			updateBestScoreUITexts (bestScore);
		}


		// Activate the Game Over menu Only and deactivate the others.
		startMenu.SetActive (false);
		gameMenu.SetActive (false);
		gameOverMenu.SetActive (true);
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

	public void updateNumberOfGemsUITexts ()
	{
		// Update Number of Gems displayed.
		int numberOfGems = PlayerPrefs.GetInt ("NumberOfPickUps");
		numberOfGemsText.text = "" + numberOfGems;
	}

	void updateDarkMode()
	{
		string darkMode = PlayerPrefs.GetString("dark");
		if (darkMode == "Off")
		{
			changeButtonSprites(settingsButton, settingsUnPressed, settingsPressed);
			// set background color
			camera.backgroundColor = colors[0];
			// set both score and best score texts for menu and game over menu
			// color should always be opposite of background
			for (int i = 0; i < scoreTexts.Length; i++)
			{
				scoreTexts[i].color = colors[1];
				bestScoreTexts[i].color = colors[1];
			}
			// set gem text color -- always opposite of backgound
			numberOfGemsText.color = colors[1];
			// set game name text color -- always opposite of background
			gameNameText.color = colors[1];
			// set submit button colors
			changeButtonSprites(submitButton, submitUnPressed, submitPressed);
			changeButtonSprites(playButton, playSprite, playSprite);
		}
		else
		{
			changeButtonSprites(settingsButton, settingsDarkUnPressed, settingsDarkPressed);
			// set background color
			camera.backgroundColor = colors[1];
			// set both score and best score texts for menu and game over menu
			// color should always be opposite of background
			for (int i = 0; i < scoreTexts.Length; i++)
			{
				scoreTexts[i].color = colors[0];
				bestScoreTexts[i].color = colors[0];
			}
			// set gem text color -- always opposite of backgound
			numberOfGemsText.color = colors[0];
			// set game name text color -- always opposite of background
			gameNameText.color = colors[0];
			// set submit button colors
			changeButtonSprites(submitButton, submitDarkUnPressed, submitDarkPressed);
			changeButtonSprites(playButton, playDarkSprite, playDarkSprite);
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

// This is where Start Skillz will go
// OnSkillzWillExit will also redirect to StartScreen Scene
	public void onPlayButtonClicked ()
	{
		SceneManager.LoadScene("GamePlay");
	}

	#endregion
}
