using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour {
	public List<GameObject> deck;
	public GameObject cardPrefab;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void addCard(GameObject card){
		deck.Add(card);
	}
	
}
