using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


// Written By Abdalla Tawfik & Fehr Hassan


public class SplashController : MonoBehaviour {

	#region Variables Declaration & Initialization

	private float setTime = 3;            // The waiting Time in the Splash Screen before Loading the Next Scene.
	float timer = 0.0f;                   // Timer to count until the setTime is finished.

	#endregion



	#region Unity Callbacks

	void Start ()
	{
		// Super Janky?
		// might need to check this one out
		// Try different values for both Android and iOS and choose the best.
		#if UNITY_ANDROID
		setTime = 3;
		#elif UNITY_IPHONE
		setTime = 6;
		#endif

		checkLanucheStatus ();
	}

	void Update ()
	{
		// Increasing timer until it is greater than or equal to the setTime.
		timer += Time.deltaTime;
		if (timer >= setTime)
			loadNextScene ();
	}

	#endregion


	#region Supporting Methods

	void checkLanucheStatus()
	{
		string hasLauncedBefore = PlayerPrefs.GetString("HasLauncedBefore");
		if (hasLauncedBefore != "Yes")
		{
			// First time to launch the Game!
			// Initialize All Player Preferences.

			// Some of the following Player Preference may be very useful for analytics data.

			// A string Shows if the game has been launched before or not.
			PlayerPrefs.SetString("HasLauncedBefore", "Yes");

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

	void loadNextScene ()
	{
		// Load the next scene.
		SceneManager.LoadScene ("GamePlay");
	}

	#endregion
}
