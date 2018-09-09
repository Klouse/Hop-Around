using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
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
  // Color per powerup
  public Color[] itemColors;

	// Ball sliding speed in X Axis.
  private float slidingSpeed;

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

	// A reference to the GamePlay UI Controller script.
	public GamePlayUIController uiController;

  // Store our current scene
  public Scene currentScene;

  public Material blackMaterial;
  public Material whiteMaterial;

  public float fadeTimeWait;
  public float fadeAmount;
	// Game Step tracker
	private int steps = -1;

  // Overall game score
  public int score = 0;
  // Score multiplier
  public float scrMultiplier = 1;
  // Score per platform
  private int platformScore = 10;
  // Score per powerup
  private int powerUpScore = 150;

  // Score UI increment
  public float scoreUpdateDuration = 0.2f;


	#endregion


  #region Debug section

  // slider Speed - sliderbar
  public void Slider_Changed(float newSpeed)
  {
    slidingSpeed = newSpeed;
    uiController.updateSliderUITexts(newSpeed);
    PlayerPrefs.SetFloat("Slider Speed", newSpeed);
  }


  #endregion


	#region Unity Callbacks

	void Start ()
	{
    currentScene = SceneManager.GetActiveScene();
    if(currentScene.name == "GamePlay")
    {
      // Turn off gravity until the Player starts to play.
  		Physics.gravity = new Vector3(0, 0, 0);
      // Update Debug UI items
      float speed = PlayerPrefs.GetFloat("Slider Speed");
      uiController.updateSliderUITexts(speed);
      uiController.updateSliderUISlider(speed);
    }

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
  			uiController.onGameOver();
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
				transform.Translate(touchDeltaPosition.x * slidingSpeed * Time.deltaTime, 0, 0);
			}
			else if (useKeyboard)
			{
				// Control Sliding using Keyboard right and left arrow buttons.
				float moveHorizontal = Input.GetAxis ("Horizontal");
				transform.Translate(moveHorizontal * slidingSpeed * 0.3f, 0, 0);
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
          // increment steps
          incrementSteps();
          // update score with platform score
          updateScore(platformScore);
          // play sound
          playSound();
          // Make the player jump
          jump(col.gameObject);
            
          // Show floating text
          uiController.ShowFloatingText(col.gameObject, platformScore.ToString(), itemColors[3]);
        }
        else if (currentScene.name == "Start Screen" && col.gameObject.tag == "cube")
        {
          jumpInPlace(col.gameObject);
        }
        else if (col.tag == "Gem_Item")
        {
            // Collision detected with a pick up (Gem) Object.

            // update current score with gem value
            var s = powerUpScore + 50;
            updateScore(s);
            
            // Show floating text
            uiController.ShowFloatingText(col.gameObject, s.ToString(), itemColors[1]);

            // Deactivate The Gem Object.
            col.gameObject.SetActive(false);

            // Increment number of Gems.
            int numberOfPickUps = PlayerPrefs.GetInt("NumberOfPickUps");
            numberOfPickUps++;
            PlayerPrefs.SetInt("NumberOfPickUps", numberOfPickUps);

            // Instantiate Gem Explosion in the same position of the picked Gem.
            GameObject gemExplosionObject = Instantiate(gemsExplosion, new Vector3(transform.position.x, gemsExplosion.transform.position.y, transform.position.z), gemsExplosion.transform.rotation) as GameObject;

            // Destroy Gem Explosion Object.
            Destroy(gemExplosionObject, 1f);
        }
        else if (col.tag == "Shield_Item")
        {
            // Show floating text
            uiController.ShowFloatingText(col.gameObject,"Shield", itemColors[0]);

            // Collision detected with a Shield Object.
            // Deactivate The Shield Object.
            col.gameObject.SetActive(false);
            
            // Call powerup handler
            enableShield();
        }
        else if(col.tag == "Crate_Item")
        {
            // Collision detected with an obstacle. (Crate)
            // Hitting the wall or obstacle will slow the player back to the starting speed.

            // Show floating text
            uiController.ShowFloatingText(col.gameObject, "Slow", itemColors[2]);
            
            // Deactivate The Crate Object.
            col.gameObject.SetActive(false);
            resetSteps();
        }
        else if (col.tag == "Ring_Item")
        {
            // Collision detected with a pick up (Gem) Object.

            // update current score with gem value
            var s = powerUpScore + 20;
            updateScore(s);
            
            // Show floating text
            uiController.ShowFloatingText(col.gameObject, s.ToString(), itemColors[4]);

            // Instantiate Gem Explosion in the same position of the picked Gem.
            GameObject gemExplosionObject = Instantiate(gemsExplosion, new Vector3(transform.position.x, gemsExplosion.transform.position.y, transform.position.z), gemsExplosion.transform.rotation) as GameObject;

            // Destroy Gem Explosion Object.
            Destroy(gemExplosionObject, 1f);
        }
    }

    void jump(GameObject go)
    {
      // Default Y position if the Player is touching the top of a Cube.
      float defultYPos = GetComponentInChildren<Collider>().bounds.size.y / 2 + go.gameObject.GetComponent<Collider>().bounds.size.y / 2;

      // Correct any position error due to late collision detection.
      transform.position = new Vector3(transform.position.x, defultYPos, go.gameObject.transform.position.z);
        if (go.tag == "cube")
        {
      		if (PlayerPrefs.GetString("dark") == "Off")
      		{
            //go.gameObject.transform.GetChild(1).gameObject.GetComponent<Renderer>().material = blackMaterial;
            //StartCoroutine(fadeCube(go));
      		}
      		else
      		{
            //go.gameObject.transform.GetChild(1).gameObject.GetComponent<Renderer>().material = whiteMaterial;
            //StartCoroutine(fadeCube(go));
      		}
          // Show Boundary Cube.
          //go.gameObject.transform.GetChild(1).gameObject.SetActive(true);

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

    void jumpInPlace(GameObject go)
    {
      // Default Y position if the Player is touching the top of a Cube.
      float defultYPos = GetComponent<Collider>().bounds.size.y / 2 + go.gameObject.GetComponent<Collider>().bounds.size.y / 2;

      // Correct any position error due to late collision detection.
      transform.position = new Vector3(transform.position.x, defultYPos, go.gameObject.transform.position.z);
        if (go.tag == "cube")
        {
          if (PlayerPrefs.GetString("dark") == "Off")
          {
            go.gameObject.transform.GetChild(1).gameObject.GetComponent<Renderer>().material = blackMaterial;
            StartCoroutine(fadeCube(go));
          }
          else
          {
            //go.gameObject.transform.GetChild(1).gameObject.GetComponent<Renderer>().material = whiteMaterial;
            //StartCoroutine(fadeCube(go));
          }
          // Show Boundary Cube.
          //go.gameObject.transform.GetChild(1).gameObject.SetActive(true);

        }


// Get the current gravity value.
      float g = Physics.gravity.magnitude;

// Calculate the total time required to jump with specific Height.
      float totalTime = Mathf.Sqrt(jumpHeight * 7 / g);

// Calculate the vertical speed required to jump with specific Height.
      float vSpeed = totalTime * g / 2;
// Calculate the forward speed required to jump specific Distance.
      float fSpeed = 0;


      // launch the Ball with the calculated speed.
      Vector3 v = new Vector3(0, vSpeed, fSpeed);
      GetComponent<Rigidbody>().velocity = v;
    }

    // increment distance traveled
    void incrementSteps()
    {
      steps++;                             // Increminting the step count.
      controlGravity();                   // Calculate the new gravity according to the new score.
    }

    // Reset Steps will kill the speed of the player and reset it back to starting speed
    // This is used by the crate item that "slows" the player
    void resetSteps()
    {
      steps = -1; // reset the steps taken
      // Reset Gravity
      Physics.gravity = new Vector3(0, minGravity, 0);
    }

    // Update score with provided value
    void updateScore(int scoreAddition)
    {
      score += (int)(scoreAddition * scrMultiplier);
      // Send score to countTo
      UICountTo(score);
    }

    // UI Count to will animate the increment of score on screen up to the current real score
    void UICountTo(int target)
    {
      StopCoroutine ("CountTo");
      StartCoroutine ("CountTo", target);
    }

    // Counts up to the target int and updates the on screen UI accordingly
    IEnumerator CountTo (int target)
    {
         int start = uiController.score;
         int tmpScore;
         for (float timer = 0; timer < scoreUpdateDuration; timer += Time.deltaTime) {
             float progress = timer / scoreUpdateDuration;
             tmpScore = (int)Mathf.Lerp (start, target, progress);
             uiController.score = tmpScore;      // Update score variable of uiController script.
             uiController.updateScoreUITexts();  // Update all Score UI Texts with the current score.
             yield return null;
         }
         uiController.score = target;
         uiController.updateScoreUITexts();  // Update all Score UI Texts with the current score.
     }



    // plays sound if sound is enabled
    void playSound()
    {
  		string sound = PlayerPrefs.GetString("sound");
      if (sound == "On"){
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
      }
    }

    // fade out boundary cube
    IEnumerator fadeCube(GameObject cube)
  	{
      var material = cube.gameObject.transform.GetChild(1).gameObject.GetComponent<Renderer>().material;
      var color = material.color;
      while(color.a > 0.0f)
      {
        yield return new WaitForSeconds(fadeTimeWait);
        color = material.color;
        material.color = new Color(color.r, color.g, color.b, color.a - (fadeAmount * Time.deltaTime));
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
		if (steps >0 && steps % gravityRate == 0 && newYGravity > maxGravity)
		{
			Physics.gravity = new Vector3(0, newYGravity, 0);
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
  // Build the list of power ups and set them to false
  // This helps control if a certain item will spawn or not (In cube controller)
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
    // Turn on shield effect on player (blue glow)
    transform.GetChild(0).gameObject.SetActive(true);
    // update current score because of the pickup
    updateScore(powerUpScore);
  }
  void useShield()
  {
    // Make the player jump "off" of closest cube z position
    jump(findClosestCube());
    // Instantiate Gem Explosion in the same position of the picked Gem.
    GameObject shieldExplosionObject = Instantiate(shieldExplosion, new Vector3(transform.position.x,(transform.position.y - GetComponent<Collider>().bounds.size.y / 2), transform.position.z), shieldExplosion.transform.rotation) as GameObject;

    // Destroy Gem Explosion Object.
    Destroy(shieldExplosionObject, 2f);

    // set shield to false
    curPowers["Shield"] = false;
    // Turn off shield effect on player (blue glow)
    transform.GetChild(0).gameObject.SetActive(false);
  }

  // Returns true or false on whether a power effect is currently applied to the player
  public bool isPowerActive(string power)
  {
    try{
      string p = power.Remove(power.Length - 5);
      if (curPowers[p] == true)
      {
        return true;
      }else
      {
        return false;
      }
    }catch {
      return false;
    }
  }
  #endregion
}
