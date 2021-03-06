﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Written By Fehr Hassan & Abdalla Tawfik


public class CubeController : MonoBehaviour {

	#region Variables Declaration & Initialization

	// A reference to the game play cube.
	public GameObject cube;
	// The number of the cubes that will appear on the scene.
	public int numberOfInstantiatedCubes;

	// Array of the cubes
	private GameObject[] instantiatedCubes;
	// Queue of the cubes
	private Queue<GameObject> queueOfCubes = new Queue<GameObject>();

	// A reference to the gem game object.
	public GameObject gems;
	// Queue of gems.
	private Queue<GameObject> queueOfGems = new Queue<GameObject>();

	// Color of the cubes.
	public Color currentColor;
	// Array of the colors for the cubes.
	public Color[] colorsOfTheCube;

	// Speed for moving the cube horizontally.
	public float horizontalSpeed;
	// Speed for sliding the cube from up to down.
	public float slidingSpeed;

	// Margin.
	public float margin;
	// Array of the values to be used in the x positions.
	public int[] xPositions;
	// Displacement in z-axis between cubes.
	public int lengthOfTheCubes;
	// Intialize the z-axis point for the first cube.
	float actualZPosition = 0.0f;
	// Initialize the row counter
	int row = 0;
	// List of the colors.
	private List<Color> colorsList;
	// Intialize the counter that is used to change the color of the cubes.
	private int counterForSpawnCubes = 1;

	// A random variable to decide if the cubes will be moved horizontally or not.
	private int randomForMovingTheCubeInXaxis;

	// A reference to a player game object.
	public GameObject player;
	// A float number which is used to keep the displacement between this game object and the the player constant.
	private float offset;

	#endregion



	#region Unity CallBacks

	void Start ()
	{
		// Displacement in z-axis constant between this game object and player.
		offset = transform.position.z - player.transform.position.z;
		// Intialize the color list.
		colorsList = new List<Color>(colorsOfTheCube.Length);

		// Create an array of cubes and assign the first cube in the array.
		instantiatedCubes = new GameObject[numberOfInstantiatedCubes+1];
		instantiatedCubes[0] = cube;

		// Fill the list of the colors from a color array.
		FillColors();
		// Intialize and create cubes.
		StartCoroutine(OnStart());

		// Intialize and create gems.
		for (int i= 0;i <= 9; i++)
		{
			GameObject instantiatedGems = Instantiate(gems) as GameObject;
			instantiatedGems.SetActive(false);
			instantiatedGems.name = "Gem";
			queueOfGems.Enqueue(instantiatedGems);
		}
	}

	void LateUpdate()
	{
		// To keep the displacement in z-axis between this object and the player game object constant.
		if (player != null)
			transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z + offset);

	}

	void FixedUpdate()
	{
		
	}

	#endregion



	#region Placing the cubes in a certain positions for the first time

	IEnumerator OnStart()
	{
		int positionIncrement = 0;
		for (int i = 0; i < numberOfInstantiatedCubes; i++)
		{
			// For the first set of cubes, spawn in rows of three
			// Choose a x position from the pre-determined x Positions.
			int currentXPosition = xPositions[positionIncrement];

			// Add a margin to the choosen position.
			//float actualXPosition = currentXPosition + UnityEngine.Random.Range(-margin, margin);

			// Ignore margin to the choosen position.
			float actualXPosition = currentXPosition;

			// intialize y value of the cube.
			float yPosition = (((i + 1f) * numberOfInstantiatedCubes) / numberOfInstantiatedCubes);

			// Determine the position of the cube from the previous calculations.
			Vector3 place = new Vector3
				(actualXPosition,
					yPosition,
					actualZPosition + lengthOfTheCubes
				);

			// create a cube and place it in the pre-determined position.
			GameObject instantiatedCube = Instantiate(cube, place, Quaternion.identity) as GameObject;

			// Change the name of the cube in the hierarchy.
			if (positionIncrement == 2)
			{
				instantiatedCube.name = "Cube";
			}else{
				instantiatedCube.name = "pCube";
			}
			// Set the Default current color to the cube.
			instantiatedCube.GetComponentInChildren<Renderer>().material.color = currentColor;

			// Access the next element in the array.
			instantiatedCubes[i + 1] = instantiatedCube;

			// increment specific row and postion numbers
			if (positionIncrement < 2) {
				positionIncrement++;
			}else{
				// Reset positionIncrement and increment row
				positionIncrement = 0;
				row++;
				// Calculate the position of the next row of cubes in Z axis.
				actualZPosition = actualZPosition + lengthOfTheCubes;
			}
		}

		yield return new WaitForSecondsRealtime(0.1f);

		// Sliding the cubes from up to down.
		for (int i = 1; i <= numberOfInstantiatedCubes; i++)
		{
			StartCoroutine(slidingDownTheCubes(instantiatedCubes[i], null));

		}
	}

	#endregion



	#region Spawn cubes and gems, and move them from up to down and horizontally

	public void spawnCubes ()
	{
		// Counter for Changing the color of the cubes.
		counterForSpawnCubes++;

		// Check if the Color will be Changed or not.
		if (counterForSpawnCubes == 10)
		{
			changeColor();
			counterForSpawnCubes = 0;
		}

		// Boolean value to check if the gem is instantiated or not.
		bool gemInstantiated = false;


		// Adjust  the position for the cubes that will be spawned.
		// Skillz Random
		actualZPosition = actualZPosition + lengthOfTheCubes;
		int randomSelectionForXPosition = UnityEngine.Random.Range(0, xPositions.Length);
		int currentXPosition = xPositions[randomSelectionForXPosition];
		float actualXPosition = currentXPosition + UnityEngine.Random.Range(-margin, margin);
		Vector3 place = new Vector3
			(actualXPosition,
			// height of cubes hard coded
				5.0f,
				actualZPosition
			);

		// Dequeue a cube from the queue to be used and set a color to the cube and put it in a certain position.
		GameObject instantiatedCube = queueOfCubes.Dequeue();
		instantiatedCube.transform.position = place;
		instantiatedCube.GetComponent<Renderer>().material.color = currentColor; // currentColor comes from changeColor()
		instantiatedCube.SetActive(true);

		// Check if the gem will be spawned or not.
		// Skillz Random
		int randomNumberToSpawnGems = UnityEngine.Random.Range(0,11);
		if (randomNumberToSpawnGems >= 10)
		{
			gemInstantiated = true;
			// Dequeue a gem from the queue and put it above the cube.
			GameObject gem = queueOfGems.Dequeue();
			gem.transform.position = new Vector3(place.x,(place.y + 0.6f),place.z);
			gem.SetActive(true);
			// Call a coroutine which is responsible for moving the cube and the gem from up to down.
			StartCoroutine(slidingDownTheCubes(instantiatedCube, gem));
		}

 		// Increment row counter
		row++;

		if (!gemInstantiated)
			// Call a coroutine which is responsible for moving the cube from up to down.
			StartCoroutine(slidingDownTheCubes(instantiatedCube,null));
	}

	IEnumerator slidingDownTheCubes(GameObject instantiatedCube, GameObject gem)
	{
		// A variable changes the speed according to the height of the cube
		float relativeSpeed;

		// The cube and the gem will be sliding down.
		if (gem != null)
		{
			while (instantiatedCube.transform.position.y >= 0.1f)
			{
				// Change the speed according to the height of the cube.
				if (instantiatedCube.transform.position.y <= 0.5f)
					relativeSpeed = 0.5f;
				else relativeSpeed = instantiatedCube.transform.position.y;

				// Sliding the cube and the gem.
				instantiatedCube.transform.Translate(Vector3.down * 2 * relativeSpeed * slidingSpeed * Time.deltaTime);
				gem.transform.Translate(Vector3.down * relativeSpeed * 2 * slidingSpeed * Time.deltaTime);
				yield return null;
			}

			//Adjust the postion of the cube and the gem.
			instantiatedCube.transform.position = new Vector3(instantiatedCube.transform.position.x,
				0,
				instantiatedCube.transform.position.z
			);
			gem.transform.position = new Vector3(instantiatedCube.transform.position.x,
				0.6f,
				instantiatedCube.transform.position.z
			);

			// Check if this cube and the gem will be moved horizontally or not
			randomForMovingTheCubeInXaxis = UnityEngine.Random.Range(0, 9);
			if (randomForMovingTheCubeInXaxis >= 8)
				StartCoroutine(moveCube(instantiatedCube, gem, instantiatedCube.transform.position.x, instantiatedCube.transform.position.z));
			else
			{
				float z = instantiatedCube.transform.position.z;
				while (z == instantiatedCube.transform.position.z) yield return null;
				gem.transform.position = new Vector3(0, 0, 0);
				gem.SetActive(false);
				queueOfGems.Enqueue(gem);
				yield break;
			}
		}
		// Sliding the cube only.
		else
		{
			while (instantiatedCube.transform.position.y >= 0.1f)
			{
				// Change the speed according to the height of the cube.
				if (instantiatedCube.transform.position.y <= 0.5f)
					relativeSpeed = 0.5f;
				else relativeSpeed = instantiatedCube.transform.position.y;

				// Sliding the cube.
				instantiatedCube.transform.Translate(Vector3.down * 2 * relativeSpeed * slidingSpeed * Time.deltaTime);
				yield return null;
			}

			// Adjust the postion of the cube
			instantiatedCube.transform.position = new Vector3(instantiatedCube.transform.position.x,
				0,
				instantiatedCube.transform.position.z
			);

			// Check if this cube will be moved horizontally or not.
			// Skillz Random
			randomForMovingTheCubeInXaxis = UnityEngine.Random.Range(0, 9);
			if (randomForMovingTheCubeInXaxis >= 8 & row > 5)
				StartCoroutine(moveCube(instantiatedCube, null, instantiatedCube.transform.position.x, instantiatedCube.transform.position.z));
		}
	}

	// Coroutine which is responsible for moving the cube and the gem horizontally.
	IEnumerator moveCube(GameObject cubeWillMove, GameObject gemWillMove, float displacementInXAxis, float displacementInZAxis)
	{
		float z = cubeWillMove.transform.position.z;
		if (gemWillMove != null)                                                                                                                    // Check if there is a gem attached to the cube or not.
		{
			while (z == cubeWillMove.transform.position.z)                                                                                          // Check if the cube is enqueued or not
			{

				while ((cubeWillMove.transform.position.x <= displacementInXAxis + margin) && (z == cubeWillMove.transform.position.z))             // Move the cube and the gem to the right
				{
					cubeWillMove.GetComponent<Transform>().Translate(Vector3.right * Time.deltaTime * horizontalSpeed);
					gemWillMove.transform.position = new Vector3(cubeWillMove.transform.position.x, 0.6f, gemWillMove.transform.position.z);
					yield return null;
				}


				while ((cubeWillMove.transform.position.x > displacementInXAxis - margin) && (z == cubeWillMove.transform.position.z))              // Move the cube and the gem to the left
				{
					cubeWillMove.GetComponent<Transform>().Translate(Vector3.left * Time.deltaTime * horizontalSpeed);
					gemWillMove.transform.position = new Vector3(cubeWillMove.transform.position.x, 0.6f, gemWillMove.transform.position.z);
					yield return null;
				}
			}

			// Gem will be unattached from the cube when the cube is enqueued.
			gemWillMove.transform.position = new Vector3(0, 0, 0);
			gemWillMove.SetActive(false);
			queueOfGems.Enqueue(gemWillMove);                                                                                                           // Enqueue the gem.
			yield break;
		}
		else                                                                                                                                            // The cube without the gem will be moved horizontally.
		{
			while (z == cubeWillMove.transform.position.z)
			{
				while ((cubeWillMove.transform.position.x <= displacementInXAxis + margin) && (z == cubeWillMove.transform.position.z))                 // Move the cube to the right.
				{
					cubeWillMove.GetComponent<Transform>().Translate(Vector3.right * Time.deltaTime * horizontalSpeed);
					yield return null;
				}
				while ((cubeWillMove.transform.position.x > displacementInXAxis - margin) && (z == cubeWillMove.transform.position.z))                  // Move the cube to the left.
				{
					cubeWillMove.GetComponent<Transform>().Translate(Vector3.left * Time.deltaTime * horizontalSpeed);
					yield return null;
				}
			}
			yield break;
		}

	}

	#endregion



	#region Colors' functions

	void changeColor()
	{
		Color randomColor;
		// Remove the current color from the list to choose another random color.
		colorsList.Remove(currentColor);
		// Fill the list if the list is empty
		if (colorsList.Count <= 0) FillColors();
		// choosing the new color
		do
		{
			// Skillz Random
			int selectRandomColor = UnityEngine.Random.Range(0, colorsList.Count);
			randomColor = colorsList[selectRandomColor];
		} while (randomColor == currentColor);
		currentColor = randomColor;

		// Changing the color of the cubes.
		foreach(GameObject cube in instantiatedCubes)
		{
			cube.GetComponent<Renderer>().material.color = currentColor;
		}
	}

	// Fill the list from the array.
	void FillColors()
	{
		if (colorsList.Count > 0)
			colorsList.Clear();
		for (int i = 0; i <= 2; i++)
		{
			colorsList.Add(colorsOfTheCube[i]);
		}
	}

	#endregion



	#region Collision function to enqueue the cubes

	void OnTriggerEnter(Collider other)
	{
		// Check if this object collide with the cube.
		if (other.tag == "cube")
		{
			// Reset the position of the cube.
			other.gameObject.transform.position = new Vector3(0, 0, 0);
			// Enqueue this cube to the cubes' queue.
			if (other.gameObject.name == "Cube")
			queueOfCubes.Enqueue(other.gameObject);
			// Disable the child of the cube to.
			other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
			// Disable the cube game object.
			other.gameObject.SetActive(false);
			// Call "spawnCubes" function to spawn a new cube.
			if (other.gameObject.name == "Cube")
			spawnCubes();
		}
	}

	#endregion
}
