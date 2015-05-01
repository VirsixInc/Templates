using UnityEngine;
using System.Collections;

public class Resetter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		other.transform.position = Vector3.zero;
		print ("RESET MOTHAFUCKA");
	}
}
