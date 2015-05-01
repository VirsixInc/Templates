using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeckSelect : MonoBehaviour {

	CardManager manager;
	public TextAsset aCSV;
	public List<string[]> myCards;

	// Use this for initialization
	void Start () {
		manager = GameObject.Find("CardManager").GetComponent<CardManager>();
		transform.localScale = Vector3.one;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown(){
		//manager.myCSV = aCSV;

		manager.cards = myCards;
	}

	public void setCSV(){
		//manager.myCSV = aCSV;
		manager.cards = myCards;
	}
}
