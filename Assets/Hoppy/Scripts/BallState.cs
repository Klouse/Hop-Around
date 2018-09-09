using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Written By Jacob Russell


public class BallState : MonoBehaviour {

	#region Variables Declaration & Initialization

	public IEnumerator movementCoroutine;
	public bool moving = false;

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

	#endregion

}
