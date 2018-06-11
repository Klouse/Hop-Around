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
	// Queue of coroutines
	Queue<IEnumerator> movements = new Queue<IEnumerator>();

	// A reference to the items game object.
	public GameObject[] items;
	// List of items
	private List<GameObject> listOfItems = new List<GameObject>();
	// Dictionary of all obtainable items and if they are active
  private Dictionary<string, bool> itemState = new Dictionary<string, bool>();

	// Color of the cubes.
	public Color currentColor;
	// Array of the colors for the cubes.
	public Color[] colorsOfTheCube;
	// color multiplier
	public float colorMultiplier;
	// color modifier
	public float colorModifier;

	// Speed for moving the cube horizontally.
	public float horizontalSpeed;
	// Speed for sliding the cube from up to down.
	public float slidingSpeed;

	// Margin.
	public float margin;
	// Array of the values to be used in the x positions.
	public float[] xPositions;

	// Displacement in z-axis between cubes.

	public float xSpiralMult;
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
	// A reference to the player controller script
	public PlayerController playerController;
	// A float number which is used to keep the displacement between this game object and the the player constant.
	private float offset;

	private Vector3 lastRowPosition = new Vector3(1,0,0);

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

		// Intialize and create pickup items.
		for (int i= 0;i < items.Length; i++)
		{
			GameObject instantiatedItem = Instantiate(items[i]);
			// Set game object inactive
			instantiatedItem.SetActive(false);
			// add items to the available items list
			listOfItems.Add(instantiatedItem);
			// setup the item state manager
			itemState.Add(instantiatedItem.tag, false);
		}

	}

	void LateUpdate()
	{
		// To keep the displacement in z-axis between this object and the player game object constant.
		if (player != null)
			transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z + offset);

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
			float currentXPosition = xPositions[positionIncrement];

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
			instantiatedCube.transform.GetChild(0).GetComponentInChildren<Renderer>().material.color = currentColor;

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
			StartCoroutine(slidingDownTheCubes(instantiatedCubes[i]));
		}
	}

	#endregion



	#region Spawn cubes and powers, and move them from up to down and horizontally

	// will take the number of cubes that could fit in the clamp e.g. 1-3
	public void spawnCubes (int numCubes)
	{
		// Counter for Changing the color of the cubes.
		counterForSpawnCubes++;
		row++;

		// Check if the Color will be Changed or not.
		// this should be increased greatly
		if (counterForSpawnCubes > (10 * colorMultiplier))
		{
			changeColor();
			colorMultiplier = colorMultiplier * colorModifier;
		}

		// Build array of cubes to be spawned
		GameObject[] cubesToSpawn = new GameObject[numCubes];
		// get array of locations
		Vector3[] cubeLocations = cubePositions(numCubes);

		// calculate the next offset for the row based on the previousRowPosition multiplied by the constant (xSpiralConstant currently)
		// this will basically shift the entire row to the left or right of the previous row
		// need to make a split path somehow too -- this might be trickier with the amount of cubes spawning per "row" since the row itself would be split
		float xOffset = 0.0f;
		xOffset = xSpiralMult*(lastRowPosition.x);

		for (int i = 0; i < numCubes; i++)
		{
			// Spawn the cubes in random locations with an offset based on the previous row
			Vector3 place = cubeLocations[i] - new Vector3(xOffset, 0, 0);

			// create a cube and place it in the pre-determined position.
			cubesToSpawn[i] = Instantiate(cube, place, Quaternion.identity) as GameObject;
			if (i == 0)
			{
				cubesToSpawn[i].name = "newCube";
			}else{
				cubesToSpawn[i].name = "skipCube";
			}
			// Set the Default current color to the cube.
			cubesToSpawn[i].transform.GetChild(0).GetComponentInChildren<Renderer>().material.color = currentColor;
			cubesToSpawn[i].SetActive(true);

			// Check if the power will be turned on or not.
			// Skillz Random
			int randomNumberToSpawnPowers = UnityEngine.Random.Range(0,100);
			if (randomNumberToSpawnPowers <= 15)
			{
				GameObject item = itemPick();
				if (item != null)
				{
					item.transform.position = new Vector3(cubesToSpawn[i].transform.position.x, cubesToSpawn[i].transform.position.y + 0.6f, cubesToSpawn[i].transform.position.z);
					item.transform.parent = cubesToSpawn[i].transform;
					item.SetActive(true);
				}
			}
		}

		// Skillz Random
		// This is what decides whether or not the row will start to move to the left or right
		int randomDirection = UnityEngine.Random.Range(0,3);
		if (randomDirection <= 1) {
			// move the row left
			lastRowPosition.x += -1;
		} else {
			// move the row right
			lastRowPosition.x += 1;
		}
		// Call a coroutine which is responsible for moving the cube and the power from up to down.
		for (int i = 0; i < cubesToSpawn.Length; i++)
		{
			StartCoroutine(slidingDownTheCubes(cubesToSpawn[i]));
		}
	}

	Vector3 placeCube(bool firstCube)
	{
		// Adjust  the position for the cubes that will be spawned.
		// Skillz Random
		actualZPosition = actualZPosition + lengthOfTheCubes;
		int randomSelectionForXPosition = UnityEngine.Random.Range(0, xPositions.Length);
		float currentXPosition = xPositions[randomSelectionForXPosition];
		float actualXPosition = currentXPosition + UnityEngine.Random.Range(-margin, margin);
		Vector3 place = new Vector3
			(actualXPosition,
			// height of cubes hard coded
				5.0f,
				actualZPosition
			);
			return place;
	}

	Vector3[] cubePositions(int numPositions)
	{
		// create an array of positions that will be sent to the cube spawner
		// add a bit of randomness to this. will need to choose up to x number of locaitons available
		Vector3[] positions = new Vector3[numPositions];

		// set the new z position
		// might need to be multiplied by 2?
		actualZPosition = actualZPosition + lengthOfTheCubes;
		// make availablePositions list from xPosition Array
		List<float> availablePositions = new List<float>(xPositions);

		for (int i = 0; i < numPositions; i++)
		{
			//Skillz random
			// grab random X position from Positions
			int randomSelectionForXPosition = UnityEngine.Random.Range(0, availablePositions.Count);
			// Choose a x position from the pre-determined x Positions.
			float currentXPosition = availablePositions[randomSelectionForXPosition];
			// Remove the position just chosen from the availablePositions
			availablePositions.RemoveAt(randomSelectionForXPosition);
			// Add a margin to the choosen position.
			float actualXPosition = currentXPosition + UnityEngine.Random.Range(-margin, margin);

			// Determine the position of the cube from the previous calculations.
			Vector3 place = new Vector3
				(actualXPosition,
					5.0f,
					actualZPosition
				);
			positions[i] = place;
		}
		return positions;
	}

	IEnumerator slidingDownTheCubes(GameObject instantiatedCube)
	{
		// A variable changes the speed according to the height of the cube
		float relativeSpeed;
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
			CubeState state = instantiatedCube.GetComponent(typeof(CubeState)) as CubeState;
			if (randomForMovingTheCubeInXaxis >= 8 & row > 5)
			{
				state.setMoving(true);
				IEnumerator coroutine = moveCube(instantiatedCube, null, instantiatedCube.transform.position.x, instantiatedCube.transform.position.z);
				StartCoroutine(coroutine);
				state.setMoveCoroutine(coroutine);
			}
	}

	// Coroutine which is responsible for moving the cube and the power horizontally.
	IEnumerator moveCube(GameObject cubeWillMove, GameObject powerWillMove, float displacementInXAxis, float displacementInZAxis)
	{
		if (cubeWillMove != null)
		{
		float z = cubeWillMove.transform.position.z;
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

	#region item
	GameObject itemPick()
	{
		// Pick an item in the list
		// Skillz random
		int randomItem = UnityEngine.Random.Range(0, listOfItems.Count);
		GameObject g;
		g = listOfItems[randomItem];
		// if power is active on player, or item is active for pickup, don't spawn it
		if (playerController.isPowerActive(g.tag) == true || itemState[g.tag] == true)
		{
			return null;
		}else
		{
			itemActivate(g);
			return g;
		}
	}

	void itemActivate(GameObject g)
	{
		// Item is active on a cube on screen
		itemState[g.tag] = true;
	}

	void itemDeactivate(GameObject g)
	{
		// Item is no longer on screen
 		itemState[g.tag] = false;
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

		GameObject[] activeCubes = GameObject.FindGameObjectsWithTag("cubeModel");
		// Changing the color of the cubes.
		foreach(GameObject cube in activeCubes)
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
		// Check if this object (the wall) collides with a cube.
		if (other.tag == "cube")
		{
			// Reset the position of the cube.
			other.gameObject.transform.position = new Vector3(0, 0, 0);
			if (other.gameObject.name == "pCube")
			Destroy(other.gameObject);

			// Disable all of the children of the cube too.
			for(int i=1; i<other.gameObject.transform.childCount; i++)
			{
				var child = other.gameObject.transform.GetChild(i).gameObject;
				if(child != null)
				{
					// put all pickup items back in the inactive list before disabling
					if(child.tag.Substring(child.tag.Length-7) == "_Pickup")
					{
						itemDeactivate(child);
						child.transform.parent = null;
					}
					child.SetActive(false);
				}
			}
			// stop the movement coroutine if it exists
			CubeState state = other.gameObject.GetComponent(typeof(CubeState)) as CubeState;
			if(state.getMoving())
			{
				IEnumerator move = state.getMoveCoroutine();
				StopCoroutine(move);
			}
			// Skillz Random
			int spawnRandomCubes = UnityEngine.Random.Range(1, 4);
			// Call "spawnCubes" function to spawn some number of new cubes.
			if (other.gameObject.name == "Cube")
			{
				// Disable the cube game object.
				other.gameObject.SetActive(false);
				spawnCubes(spawnRandomCubes);
			}else if (other.gameObject.name == "newCube") {
				spawnCubes(spawnRandomCubes);
				Destroy(other.gameObject);
			}else if(other.gameObject.name == "skipCube") {
				Destroy(other.gameObject);
			}
		}
	}

	#endregion
}
