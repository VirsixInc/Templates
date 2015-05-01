using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadDeckBuilder(){
		Application.LoadLevel(1);
	}
	public void LoadReviewMode(){
		Application.LoadLevel(2);
	}
	public void LoadCardsGame(){
		Application.LoadLevel(3);
	}


}
