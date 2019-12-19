using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterObject : MonoBehaviour {
	

	// Color of the cubes.
	public Color currentColorBall;
	// Array of the colors for the cubes.
	public Color[] colorsOfTheBall;
	// color multiplier
	public float colorMultiplierBall;
	// color modifier
	public float colorModifierBall;
	// List of the colors.
	private List<Color> colorsListBall;
	// Intialize the counter that is used to change the color of the cubes.


	// Intialize the z-axis point for the first ball.
	float actualZPosition = 0.0f;
	// Intialize the y-axis point for the first ball.
	float actualYPosition = -6f;
	// Initialize the maximum y-axis point
	float maximumYPosition = -4.75f;
	// Initialize the movement variables for bubbleUp
	public float MoveSpeed = 5.0f;
	
	public float frequency = 0.001f;
	public float magnitude = 0.028f;

	// Margins for spawn positions
	public float x_high_margin;
	public float x_low_margin;

	public float z_high_margin;
	public float z_low_margin;

	// offset vertically to be below the water
	private float offset;
	public Collider[] colliders;
	public float radius;
	
	// Speed for bubbling up the balls
	public float bubbleSpeed;

	// number of floating ball
	public int numberOfInstantiatedBalls;

	// Array of the flaoting ball
	private GameObject[] instantiatedBalls;

	// Floating Game Object
	public GameObject floating_ball;
	// A boolean to control if it is time to spawn new bubbles.
	private bool timeToSpawn = true;
	// Timer length
	public float maxBalls;

	// Intialize the counter that is used to change the color of the cubes.
	private int counterForSpawnBalls = 1;
	public GameObject player;
	// starting position
	//private Vector3
	// Use this for initialization
	void Start () {	
		// Intialize the color list.
		colorsListBall = new List<Color>(colorsOfTheBall.Length);

		// Displacement in z-axis constant between this game object and player.
		offset = transform.position.z - player.transform.position.z;
		// Create an array of ball and assign the first ball in the array.
		instantiatedBalls = new GameObject[numberOfInstantiatedBalls+1];
		instantiatedBalls[0] = floating_ball;

		// Fill the list of the colors from a color array.
		FillColors();
		// Intialize and create ball
		StartCoroutine(OnStart());
	}

	void LateUpdate()
	{
		// To keep the displacement in z-axis between this object and the player game object constant.
		if (player != null){
			transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z + offset);
		}
	}

	#region Placing floating ball in a random positions and moving them

	IEnumerator OnStart()
	{
		// get array of locations
		Vector3[] ballLocations = ballPositions(numberOfInstantiatedBalls);
		for (int i = 0; i < numberOfInstantiatedBalls; i++)
		{				
			// Spawn the balls in random locations within the clamp
			Vector3 place = ballLocations[i];

			// Instantiate a ball and place it in the pre-determined position.
			GameObject instantiatedBall = Instantiate(floating_ball, place, Quaternion.identity) as GameObject;

			// Change the name of the ball in the hierarchy.
			instantiatedBall.name = "Ball";
			// Set the Default current color to the ball.
			instantiatedBall.transform.GetComponent<Renderer>().material.color = currentColorBall;

			// Access the next element in the array.
			instantiatedBalls[i + 1] = instantiatedBall;
		}

		yield return new WaitForSecondsRealtime(0.1f);

		// Call a coroutine which is responsible for "bubbling" the ball up from under the water
		for (int i = 1; i <= numberOfInstantiatedBalls; i++)
		{
			// set the ball state variables
			BallState state = instantiatedBalls[i].GetComponent(typeof(BallState)) as BallState;
			state.setMoving(true);

			StartCoroutine(bubbleUp(instantiatedBalls[i]));
		}
	}

	#endregion

	#region Spawn balls, and move them from down to up and horizontally

	// will take the number of balls that could fit in the clamp e.g. 1-3
	public void spawnBalls (int numBall)
	{
		// Counter for Changing the color of the balls.
		counterForSpawnBalls++;

		// Check if the Color will be Changed or not.
		// this should be increased greatly
		if (counterForSpawnBalls > (10 * colorMultiplierBall))
		{
			changeColor();
			colorMultiplierBall = colorMultiplierBall * colorModifierBall;
		}

		// Build array of balls to be spawned
		GameObject[] ballsToSpawn = new GameObject[numBall];
		// get array of locations
		Vector3[] ballLocations = ballPositions(numBall);

		for (int i = 0; i < numBall; i++)
		{
			// Spawn the balls in random locations within the clamp
			Vector3 place = ballLocations[i];

			// create a ball and place it in the pre-determined position.
			ballsToSpawn[i] = Instantiate(floating_ball, place, Quaternion.identity) as GameObject;
			// Name the ball
			ballsToSpawn[i].name = "Ball";
			// Set the Default current color to the ball.
			ballsToSpawn[i].GetComponent<Renderer>().material.color = currentColorBall;
			ballsToSpawn[i].SetActive(true);
		}
		// Call a coroutine which is responsible for "bubbling" the ball up from under the water
		for (int i = 0; i < ballsToSpawn.Length; i++)
		{
			// set the ball state variables
			BallState state = ballsToSpawn[i].GetComponent(typeof(BallState)) as BallState;
			state.setMoving(true);

			StartCoroutine(bubbleUp(ballsToSpawn[i]));
		}
	
	}

	IEnumerator bubbleUp(GameObject instantiatedBall)
	{
		// A variable changes the speed according to the height of the ball
		float relativeSpeed;
		
		BallState state = instantiatedBall.GetComponent(typeof(BallState)) as BallState;
		state.setFrequency(frequency);
		state.setMagnitude(magnitude);
		float freq = state.getFrequency();
		float mag = state.getMagnitude();
		if (state.getMoving())
		{
			while (instantiatedBall != null && instantiatedBall.transform.position.y<= maximumYPosition)
			{
				// Change the speed according to the height of the ball.
				if (instantiatedBall.transform.position.y <= 0.5f)
					relativeSpeed = 0.5f;
				else relativeSpeed = instantiatedBall.transform.position.y;



				// Sliding the ball.
				instantiatedBall.transform.Translate(Vector3.up * 2 * relativeSpeed * bubbleSpeed * Time.deltaTime);
				yield return null;
			}

			while(instantiatedBall != null && mag > 0)
			{
				freq = state.getFrequency();
				mag = state.getMagnitude();
				// Set the new y position based off the current			
				float yPos = instantiatedBall.transform.position.y + Mathf.Cos (freq) * mag;

				// Apply the new y position to the ball
				instantiatedBall.transform.position = new Vector3(instantiatedBall.transform.position.x,
				yPos,
				instantiatedBall.transform.position.z
				);

				// Increase the frequency so the ball will change position next loop
				//freq += 0.021f;
				freq += 0.252f;
				freq += Mathf.Abs(2f * Mathf.PI);
				state.setFrequency(freq);
				// Slowly decrease the magnitude till it drops below 0
				//mag -= 0.000035f;
				mag -= 0.00042f;
				state.setMagnitude(mag);

				yield return null;
			}

			// Ball has now stopped moving, set the state
			state.setMoving(false);
		}else
		{
			// Do nothing if the ball is not moving
			yield return null;
		}
	}

	Vector3[] ballPositions(int numPositions)
	{
		// create an array of positions that will be sent to the ball spawner
		// add a bit of randomness to this. will need to choose up to x number of locaitons available
		Vector3[] positions = new Vector3[numPositions];
		for (int i = 0; i < numPositions; i++)
		{
			bool canSpawnHere;
			// Choose an x and z position at random
			// Skillz Random
			float actualXPosition = player.transform.position.x + UnityEngine.Random.Range(x_low_margin, x_high_margin);
			float actualZPosition = player.transform.position.z + UnityEngine.Random.Range(z_low_margin, z_high_margin);

			// Determine the position of the ball from the previous calculations.
			Vector3 place = new Vector3
				(actualXPosition,
				actualYPosition,
				actualZPosition
				);
				canSpawnHere = PreventSpawnOverlap(place);
			positions[i] = place;
		}
		return positions;
	}

	

	bool PreventSpawnOverlap(Vector3 place)
	{
		// A method to prevent a ball from spawning inside of another ball
		colliders = Physics.OverlapSphere(transform.position, radius);
		for (int i=0; i < colliders.Length; i++){
			Vector3 centerPoint = colliders[i].bounds.center;
			float width = colliders[i].bounds.extents.x;
			float length = colliders[i].bounds.extents.z;

			float leftExtent = centerPoint.x - width;
			float rightExtent = centerPoint.x + width;
			float lowerExtent = centerPoint.z - length;
			float upperExtent = centerPoint.z + length;

			
		}
		return true;
	}

	#endregion

	#region Colors' functions

	void changeColor()
	{
		Color randomColor;
		// Remove the current color from the list to choose another random color.
		colorsListBall.Remove(currentColorBall);
		// Fill the list if the list is empty
		if (colorsListBall.Count <= 0) FillColors();
		// choosing the new color
		do
		{
			// Skillz Random
			int selectRandomColor = UnityEngine.Random.Range(0, colorsListBall.Count);
			randomColor = colorsListBall[selectRandomColor];
		} while (randomColor == currentColorBall);
		currentColorBall = randomColor;

		GameObject[] activeCubes = GameObject.FindGameObjectsWithTag("cubeModel");
		// Changing the color of the cubes.
		foreach(GameObject cube in activeCubes)
		{
			cube.GetComponent<Renderer>().material.color = currentColorBall;
		}
	}

	// Fill the list from the array.
	void FillColors()
	{
		if (colorsListBall.Count > 0)
			colorsListBall.Clear();
		for (int i = 0; i <= 2; i++)
		{
			colorsListBall.Add(colorsOfTheBall[i]);
		}
	}

	#endregion

	#region Helpers

	public GameObject findClosestBall(GameObject currentBall)
	{
		GameObject[] balls;
		balls = GameObject.FindGameObjectsWithTag("Ball");
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = currentBall.transform.position;

		foreach (GameObject b in balls)
		{
			float diff = b.transform.position.z - position.z;
			if (Mathf.Abs(diff) < distance)
			{
				closest = b;
				distance = Mathf.Abs(diff);
			}
		}
		return closest;
	}

	#endregion

	

	#region Collision function to enqueue the ball

	void OnTriggerEnter(Collider other)
	{
		// Check if this object (the wall) collides with a ball.
		if (other.tag == "MovingEnvironment")
		{
			// Reset the position of the ball.
			other.gameObject.transform.position = new Vector3(0, 0, 0);

			// stop the movement coroutine if it exists
			BallState state = other.gameObject.GetComponent(typeof(BallState)) as BallState;
			/* if(state.getMoving())
			{
				IEnumerator move = state.getMoveCoroutine();
				StopCoroutine(move);
			} */
			if (other.gameObject.name == "Ball")
			{
				// Destroy the ball game object.
				Destroy(other.gameObject);
			}
			
			int totalBalls = GameObject.FindGameObjectsWithTag("MovingEnvironment").Length;

			// Spawn cubes if interval has passed
			if (totalBalls < maxBalls)
			{
				// Call "spawnBalls" function to spawn some 1 new ball.
				spawnBalls(1);
			}
		}
	}

	#endregion

}
