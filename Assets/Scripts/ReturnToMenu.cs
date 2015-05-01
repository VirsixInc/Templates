using UnityEngine;
using System.Collections;

public class ReturnToMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GoToMenu(){
		Application.LoadLevel(0);
	}
}
