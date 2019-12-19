using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Written By Jacob Russell


public class BallState : MonoBehaviour {

	#region Variables Declaration & Initialization

	public IEnumerator movementCoroutine;
	public bool moving = false;

	// frequency and magnitude
	float frequency;
	float magnitude;

	#endregion

	#region Getters & Setters

	public void setMoving(bool move)
	{
		moving = move;
	}

	public bool getMoving()
	{
		return moving;
	}

	public void setMoveCoroutine(IEnumerator moveCo)
	{
		movementCoroutine = moveCo;
	}

	public IEnumerator getMoveCoroutine()
	{
		return movementCoroutine;
	}

	public void setFrequency(float freq)
	{
		frequency = freq;
	}

	public float getFrequency()
	{
		return frequency;
	}

	public void setMagnitude(float mag)
	{
		magnitude = mag;
	}

	public float getMagnitude()
	{
		return magnitude;
	}

	#endregion

}
