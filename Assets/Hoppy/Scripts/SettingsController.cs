using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


// Written By Abdalla Tawfik & Fehr Hassan


public class SettingsController : MonoBehaviour {


	#region Variables Declaration

	public Button soundButton;                     // A reference to the Sound Button to change its sprites programatically.
	public Button restorePurchasesButton;          // A reference to the Restore Purchases Button to deactivate it for Android.

	public Sprite soundOnUnPressed;                // A reference to the Sound Button Sprite if sound is on and button is unpressed.
	public Sprite soundOnPressed;                  // A reference to the Sound Button Sprite if sound is on and button is pressed.
	public Sprite soundOffUnPressed;               // A reference to the Sound Button Sprite if sound is off and button is unpressed.
	public Sprite soundOffPressed;                 // A reference to the Sound Button Sprite if sound is off and button is pressed.

	public Text numberOfGemsText;                  // A reference to Number of Gems UI Text.

	#endregion



	#region Unity Callbacks

	void Awake ()
	{

		// Deactivate Restore Purchases Button for Android.
		// All non-consumable In App Purchases are automatically restored by Google.
		#if UNITY_ANDROID
		restorePurchasesButton.gameObject.SetActive (false);
		#endif

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

	public void onRemoveAdsButtonClicked ()
	{
		// Buy the Remove Ads product.
	}

	public void onRestorePurchasesButtonClicked ()
	{
		// Restore non-consumable purchases previously made by this player for iOS only.
		#if UNITY_IPHONE

		#endif
	}

	public void onPromotionButtonClicked ()
	{
		// Open the url of the Game or Application you want to promote for both Android and iOS.
		// Change Current Links with Your Game's Links.
		#if UNITY_ANDROID
		Application.OpenURL ("market://details?id=games.fa.rollingcolor"); 
		#elif UNITY_IPHONE
		Application.OpenURL ("https://itunes.apple.com/app/id1146256287");
		#endif
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
