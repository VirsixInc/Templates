using UnityEngine;
using System.Collections;

public class RotateInfinite : MonoBehaviour {

	public float rotateSpeed = 3f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (0f, 0f, rotateSpeed);
	}
}
