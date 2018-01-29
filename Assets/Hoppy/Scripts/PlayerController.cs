using UnityEngine;
using System.Collections;


// Written By Abdalla Tawfik & Fehr Hassan


public class PlayerController : MonoBehaviour {

    #region Variables Declaration & Initialization

	// A reference to Gems Explosion.
    public GameObject gemsExplosion;

	// Ball sliding speed in X Axis.
    public float slidingSpeed;
	// The magnitude of the limit of Player X position.
	private float xPosLimit = 5f;
	// Jump distance in Z Axis.
	private float jumpDistance = 4f;
	// Jump Height in Y Axis.
	private float jumpHeight = 2.0f;
	// Minimum gravity -> minimum speed.
	private float minGravity = -35;
	// Maximum gravity -> maximum speed.
	private float maxGravity = -85;
	// After how many steps the gravity will be increased.
	private int gravityRate = 10;
	// How much the gravity will be increased after each speed step.
	private int gravityStep = 10;

	// Represent Game Started state.
	private bool gameStarted = false;
	// Represent Game Over state.
	private bool gameOver = false;

	// Determine whether or not keyboard will be used.
	private bool useKeyboard = true;

	// Game Score (steps).
	private int score = -1;

	// A reference to the GamePlay UI Controller script.
	public GamePlayUIController uiController;

	#endregion



	#region Unity Callbacks

	void Start ()
	{
		// Turn off gravity until the Player starts to play.
		Physics.gravity = new Vector3(0, 0, 0);
	}

	void Update ()
	{
		// Control sliding using touch input and keyboard.
		controlSliding ();
		// Control player rotation.
		controlBallRotation ();

		// Check whether game is over or not.
		checkGameOver ();
	}

	#endregion



	#region Game States Control

	public void startGame ()
	{
		if (!gameStarted)
		{
			// Turn on gravity with min gravity value.
			Physics.gravity = new Vector3(0, minGravity, 0);

			// Update UI and gameStarted boolean.
			uiController.onGameStarted ();
			gameStarted = true;
		}
	}

	void checkGameOver ()
	{
		if (transform.position.y < 0 && !gameOver)
		{
			// Game is Over.

			// Stop all forces.
			GetComponent<Rigidbody>().velocity = Vector3.zero;

			// Increase the gravity.
			Physics.gravity = new Vector3(0, maxGravity-1100.0f, 0);


			// Update UI and gameOver boolean.
			uiController.onGameOver ();
			gameOver = true;


			// Destroy Player Game Object after 1 seconds.
			Destroy (gameObject, 1f);
		}
	}

	#endregion



	#region Ball Control

	void controlSliding ()
	{
		// While the game is started and is not over yet.
		if (gameStarted && !gameOver)
		{
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				// Control sliding using touch input.
				Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
				transform.Translate(touchDeltaPosition.x * slidingSpeed, 0, 0);
			}
			else if (useKeyboard)
			{
				// Control Sliding using Keyboard right and left arrow buttons.
				float moveHorizontal = Input.GetAxis ("Horizontal");
				transform.Translate(moveHorizontal * slidingSpeed * 14, 0, 0);
			}

			// Clamp the Player's X position between - xPosLimit and xPosLimit.
			transform.position = new Vector3 (Mathf.Clamp(transform.position.x, - xPosLimit, xPosLimit), transform.position.y, transform.position.z);
		}
	}

    public void OnTriggerEnter(Collider col)
    {
        // While the game is started and is not over yet.
        if (col.gameObject.tag == "cube" && gameStarted && !gameOver)
        {
            // Collision detected with a tile (Cube) Object.

            score++;                             // Increminting the score.
            uiController.score = score;          // Update score variable of uiController script.
            uiController.updateScoreUITexts();  // Update all Score UI Texts with the current score.


            controlGravity();                   // Calculate the new gravity according to the new score.


            // Default Y position if the Player is touching the top of a Cube.
            float defultYPos = GetComponent<Collider>().bounds.size.y / 2 + col.gameObject.GetComponent<Collider>().bounds.size.y / 2;

            // Correct any position error due to late collision detection.
            transform.position = new Vector3(transform.position.x, defultYPos, col.gameObject.transform.position.z);

            // Show Boundary Cube.
            col.gameObject.transform.GetChild(0).gameObject.SetActive(true);


			// Get the current gravity value.
            float g = Physics.gravity.magnitude;

			// Calculate the total time required to jump with specific Height.
            float totalTime = Mathf.Sqrt(jumpHeight * 8 / g);

			// Calculate the vertical speed required to jump with specific Height.
            float vSpeed = totalTime * g / 2;
			// Calculate the forward speed required to jump specific Distance.
            float fSpeed = jumpDistance / totalTime;


            // launch the Ball with the calculated speed.
            GetComponent<Rigidbody>().velocity = new Vector3(0, vSpeed, fSpeed);
        }
        if (col.tag == "Pick Up")
        {
            // Collision detected with a pick up (Gem) Object.

            // Deactivate The Gem Object.
            col.gameObject.SetActive(false);

            // Increment number of Gems.
            int numberOfPickUps = PlayerPrefs.GetInt("NumberOfPickUps");
            numberOfPickUps++;
            PlayerPrefs.SetInt("NumberOfPickUps", numberOfPickUps);

            // Update Number of Gems displayed.
            uiController.updateNumberOfGemsUITexts();

            // Instantiate Gem Explosion in the same position of the picked Gem.
            GameObject gemExplosionObject = Instantiate(gemsExplosion, new Vector3(transform.position.x, gemsExplosion.transform.position.y, transform.position.z), gemsExplosion.transform.rotation) as GameObject;

            // Destroy Gem Explosion Object.
            Destroy(gemExplosionObject, 1f);
        }
    }

    void controlBallRotation ()
	{
		// While the game is started and is not over yet.
		if (gameStarted && !gameOver)
			// Rotating the player around X Axis.
			transform.Rotate(Vector3.right * 500 *Time.deltaTime, Space.World);
	}

	void controlGravity ()
	{
		// Calculate the new gravity.
		float newYGravity = Physics.gravity.y - gravityStep;

		// Increase the gravity after each speed steps with the gravit step until it reach the maximum gravity.
		if (score >0 && score % gravityRate == 0 && newYGravity > maxGravity)
		{
			Physics.gravity = new Vector3(0, newYGravity, 0);
		}
	}

	#endregion
}
