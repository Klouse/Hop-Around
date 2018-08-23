using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterObject : MonoBehaviour {
	// Intialize the z-axis point for the first ball.
	float actualZPosition = 0.0f;
	// Intialize the y-axis point for the first ball.
	float actualYPosition = -1.5f;
	// Margins for spawn positions
	public float x_high_margin;
	public float x_low_margin;

	public float z_high_margin;
	public float z_low_margin;

	// offset vertically to be below the water
	private float offset;
	
	// Speed for bubbling up the items
	public float bubbleSpeed;

	// number of floating balls
	int numberOfInstantiatedBalls;

	// Array of the flaoting balls
	private GameObject[] instantiatedBalls;

	// Floating Game Object
	public GameObject floating_ball;
	// A reference to a player game object.
	public GameObject player;
	// starting position
	//private Vector3
	// Use this for initialization
	void Start () {		
		// Displacement in z-axis constant between this game object and player.
		offset = transform.position.z - player.transform.position.z;
		// Create an array of cubes and assign the first cube in the array.
		instantiatedBalls = new GameObject[numberOfInstantiatedBalls+1];
		instantiatedBalls[0] = floating_ball;

		// Intialize and create balls
		StartCoroutine(OnStart());
	}

	void LateUpdate()
	{
		// To keep the displacement in z-axis between this object and the player game object constant.
		if (player != null){
			transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z + offset);
		}
	}

	



	#region Placing floating balls in a random positions and moving them

	IEnumerator OnStart()
	{
		for (int i = 0; i < numberOfInstantiatedBalls; i++)
		{
			// For the first set of cubes, spawn in rows of three
			// Choose an x and z position at random
			float actualXPosition = player.transform.position.x + UnityEngine.Random.Range(-x_low_margin, x_high_margin);
			float actualZPosition = player.transform.position.x + UnityEngine.Random.Range(-z_low_margin, z_high_margin);

			// intialize y value of the cube.
			float yPosition = (((i + 1f) * numberOfInstantiatedBalls) / numberOfInstantiatedBalls);

			// Determine the position of the cube from the previous calculations.
			Vector3 place = new Vector3
				(actualXPosition,
					yPosition,
					actualZPosition
				);

			// create a cube and place it in the pre-determined position.
			GameObject instantiatedBall = Instantiate(floating_ball, place, Quaternion.identity) as GameObject;

			// Change the name of the ball in the hierarchy.
			instantiatedBall.name = "Ball";
			// Set the Default current color to the cube.
			//instantiatedBall.transform.GetChild(0).GetComponentInChildren<Renderer>().material.color = currentColor;

			// Access the next element in the array.
			instantiatedBalls[i + 1] = instantiatedBall;
		}

		yield return new WaitForSecondsRealtime(0.1f);

		// Sliding the cubes from up to down.
		for (int i = 1; i <= numberOfInstantiatedBalls; i++)
		{
			StartCoroutine(bubbleUp(instantiatedBalls[i]));
		}
	}

	IEnumerator bubbleUp(GameObject instantiatedBall)
	{
		// A variable changes the speed according to the height of the cube
		float relativeSpeed;
			while (instantiatedBall.transform.position.y >= 0.1f)
			{
				// Change the speed according to the height of the cube.
				if (instantiatedBall.transform.position.y <= 0.5f)
					relativeSpeed = 0.5f;
				else relativeSpeed = instantiatedBall.transform.position.y;

				// Sliding the cube.
				instantiatedBall.transform.Translate(Vector3.down * 2 * relativeSpeed * bubbleSpeed * Time.deltaTime);
				yield return null;
			}

			// Adjust the postion of the cube
			instantiatedBall.transform.position = new Vector3(instantiatedBall.transform.position.x,
				0,
				instantiatedBall.transform.position.z
			);
	}

	#endregion
}
