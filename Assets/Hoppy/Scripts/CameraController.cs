using UnityEngine;
using System.Collections;


// Written By Abdalla Tawfik & Fehr Hassan


public class CameraController : MonoBehaviour {


	#region Variables Declaration & Initialization

	// A reference to the Player Game Object.
	public GameObject player;

	public GameObject gameUIController;

	// The distance between Camera position and the Player Position.
	private Vector3 offset;
	// The magnitude of the limit of Camera X position.
	private float xPosLimit = 1.5f;

	#endregion



	#region Unity Callbacks

	void Start ()
	{
		// The distance between Camera position and the Player Position. 
		offset = transform.position - player.transform.position;
	}
	
	void LateUpdate ()
	{
		// LateUpdate is called after all Update functions have been called.
		// A follow camera should always be implemented in LateUpdate because:
		// It tracks objects (Player) that might have moved inside Update.


		
		// Make the Camera follows the Player position on X and Z Axis.
		if (player != null) {
			// move the camera with the player and have the gameUIController follow it to keep the collider in place to de-spawn the cubes
			transform.position = new Vector3 (player.transform.position.x + offset.x, transform.position.y, player.transform.position.z + offset.z);
			gameUIController.transform.position = new Vector3(transform.position.x, gameUIController.transform.position.y, gameUIController.transform.position.z);
		}

		
	}

	#endregion
}
