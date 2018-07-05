using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour {

	public float DestroyTime = 0.5f;
	// offset vertically to be above the element it spawns on
	public Vector3 Offset = new Vector3(0,0.5f,0);
	// Use this for initialization
	void Start () {
		Destroy(gameObject, DestroyTime);
		// apply the offset to the text
		transform.localPosition += Offset;
	}
}
