using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


// Written By Abdalla Tawfik & Fehr Hassan


public class SettingsController : MonoBehaviour {


	#region Variables Declaration

	public Button soundButton;                     // A reference to the Sound Button to change its sprites programatically.

	public Sprite soundOnUnPressed;                // A reference to the Sound Button Sprite if sound is on and button is unpressed.
	public Sprite soundOnPressed;                  // A reference to the Sound Button Sprite if sound is on and button is pressed.
	public Sprite soundOffUnPressed;               // A reference to the Sound Button Sprite if sound is off and button is unpressed.
	public Sprite soundOffPressed;                 // A reference to the Sound Button Sprite if sound is off and button is pressed.

	public Text numberOfGemsText;                  // A reference to Number of Gems UI Text.

	#endregion



	#region Unity Callbacks

	void Awake ()
	{
		// Need to check dark status as well
		// Check sound status and change Sound Button sprites accordingly.
		string sound = PlayerPrefs.GetString("sound");
		if (sound == "On")
		{
			changeButtonSprites (soundButton, soundOnUnPressed, soundOnPressed);
		}
		else if (sound == "Off")
		{
			changeButtonSprites (soundButton, soundOffUnPressed, soundOffPressed);
		}

		// Update Number of Gems displayed.
		updateNumberOfGemsUIText ();
	}

	#endregion



	#region Buttons onClicked Methods

	public void onBackButtonClicked ()
	{
		// Load the previous scene.
		SceneManager.LoadScene ("GamePlay");
	}

	public void onSoundButtonClicked ()
	{
		string sound = PlayerPrefs.GetString("sound");

		if (sound == "Off")
		{
			// If sound was previously off:
			// Then turn the sound on, change Sound Button sprites and play a SFX.

			changeButtonSprites (soundButton, soundOnUnPressed, soundOnPressed);
			PlayerPrefs.SetString("sound","On");

			// You may play sound effect here.
		}
		else if (sound == "On")
		{
			// If sound was previously on:
			// Then turn the sound off and change Sound Button sprites.

			changeButtonSprites (soundButton, soundOffUnPressed, soundOffPressed);
			PlayerPrefs.SetString("sound","Off");
		}
	}

	public void onDarkModeClick ()
	{
		// dark mode string will get changed everytime it's clicked
		// no need to keep reseting manually
		string darkMode = PlayerPrefs.GetString("dark");
		if (darkMode == "On")
		{
			// turn light mode back on
			PlayerPrefs.SetString("dark", "Off");
		}
		else
		{
			// turn dark mode on
			PlayerPrefs.SetString("dark", "On");
			if(sound == "On")
			{
				changeButtonSprites(soundButton, soundOnDarkUnpressed, soundOnDarkPressed);
			}
			else
			{
				// sound is off
				changeButtonSprites(soundButton, soundOffDarkUnpressed, soundOffDarkPressed)
			}

		}
	}

	#endregion



	#region Supporting Methods

	void updateNumberOfGemsUIText()
	{
		// Update Number of Gems displayed.
		int numberOfGems = PlayerPrefs.GetInt ("NumberOfPickUps");
		numberOfGemsText.text = "" + numberOfGems;
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
}
