﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


// Written By Abdalla Tawfik & Fehr Hassan


public class PlayerController : MonoBehaviour {

    #region Variables Declaration & Initialization

	// A reference to Gems Explosion.
    public GameObject gemsExplosion;
  // A reference to Shield Explosion.
    public GameObject shieldExplosion;
    // a reference to the Shield Model
    public GameObject shield;

  // A reference to the list of power ups available
  public Dictionary<string, bool> curPowers = new Dictionary<string, bool>();
  public string[] powerUps;

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

  public Material blackMaterial;
  public Material whiteMaterial;

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
      // build the list of powers to toggle on and off
      buildPowerUpList(powerUps);
			// Turn on gravity with min gravity value.
			Physics.gravity = new Vector3(0, minGravity, 0);

			// Update UI and gameStarted boolean.
			uiController.onGameStarted ();
			gameStarted = true;
		}
	}

	void checkGameOver ()
	{
		if (transform.position.y < (0 + findClosestCube().GetComponent<Collider>().bounds.size.y / 2) && !gameOver)
		{
      if (curPowers["Shield"])
      {
        // use your shield
        useShield();
      }
      else{
  			// Game is Over.

  			// Stop all forces.
  			GetComponent<Rigidbody>().velocity = Vector3.zero;

  			// Increase the gravity.
  			Physics.gravity = new Vector3(0, maxGravity-750.0f, 0);


  			// Update UI and gameOver boolean.
  			uiController.onGameOver ();
  			gameOver = true;

          Debug.Log(" Game Over True");


  			// Destroy Player Game Object after 1 seconds.
  			Destroy (gameObject, 1f);
      }
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
          // increment score
          incrementScore();
          // Make the player jump
          jump(col.gameObject);
        }
        if (col.tag == "Gem")
        {
            // Collision detected with a pick up (Gem) Object.

            // increment score
            incrementScore();

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
        if (col.tag == "Shield_Pickup")
        {
            // Collision detected with a Shield Object.
            // Deactivate The Shield Object.
            col.gameObject.SetActive(false);
            // Call powerup handler
            enableShield();
        }
    }

    void jump(GameObject go)
    {
      // Default Y position if the Player is touching the top of a Cube.
      float defultYPos = GetComponent<Collider>().bounds.size.y / 2 + go.gameObject.GetComponent<Collider>().bounds.size.y / 2;

      // Correct any position error due to late collision detection.
      transform.position = new Vector3(transform.position.x, defultYPos, go.gameObject.transform.position.z);
        if (go.tag == "cube" && !curPowers["Shield"])
        {
		if (PlayerPrefs.GetString("dark") == "Off")
		{
			col.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = blackMaterial;
		}
		else
		{
			col.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = whiteMaterial;
		}
          // Show Boundary Cube.
          go.gameObject.transform.GetChild(0).gameObject.SetActive(true);
	
        }


// Get the current gravity value.
      float g = Physics.gravity.magnitude;

// Calculate the total time required to jump with specific Height.
      float totalTime = Mathf.Sqrt(jumpHeight * 7 / g);

// Calculate the vertical speed required to jump with specific Height.
      float vSpeed = totalTime * g / 2;
// Calculate the forward speed required to jump specific Distance.
      float fSpeed = jumpDistance / totalTime;


      // launch the Ball with the calculated speed.
      Vector3 v = new Vector3(0, vSpeed, fSpeed);
      GetComponent<Rigidbody>().velocity = v;
    }

    // increment score by 1
    void incrementScore()
    {
      score++;                             // Increminting the score.
      uiController.score = score;          // Update score variable of uiController script.
      uiController.updateScoreUITexts();  // Update all Score UI Texts with the current score.
      controlGravity();                   // Calculate the new gravity according to the new score.
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
      Debug.Log(Physics.gravity);
		}
	}
	#endregion

  public GameObject findClosestCube()
  {
    GameObject[] cubes;
    cubes = GameObject.FindGameObjectsWithTag("cube");
    GameObject closest = null;
    float distance = Mathf.Infinity;
    Vector3 position = transform.position;
    foreach (GameObject cube in cubes)
    {
      float diff = cube.transform.position.z - position.z;
      if (Mathf.Abs(diff) < distance)
      {
        closest = cube;
        distance = Mathf.Abs(diff);
      }
    }
    return closest;
  }

  #region powerUps
  void buildPowerUpList(string[] ps)
  {
    foreach (string s in ps)
    {
      curPowers.Add(s,false);
    }
  }
  void enableShield()
  {
    // turns on powerUp
    // set sheild to true
    curPowers["Shield"] = true;
    // Turn on shield effect on player = blue glow
    transform.GetChild(0).gameObject.SetActive(true);
  }
  void useShield()
  {
    // increment score
    incrementScore();
    // Make the player jump "off" of closest cube z position
    jump(findClosestCube());
    // Instantiate Gem Explosion in the same position of the picked Gem.
    GameObject shieldExplosionObject = Instantiate(shieldExplosion, new Vector3(transform.position.x,(transform.position.y - GetComponent<Collider>().bounds.size.y / 2), transform.position.z), shieldExplosion.transform.rotation) as GameObject;

    // Destroy Gem Explosion Object.
    Destroy(shieldExplosionObject, 2f);

    // set shield to false
    curPowers["Shield"] = false;
    // Turn on shield effect on player = blue glow
    transform.GetChild(0).gameObject.SetActive(false);
  }
  #endregion
}
