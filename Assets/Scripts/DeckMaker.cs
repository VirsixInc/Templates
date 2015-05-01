using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class DeckMaker : MonoBehaviour {

	public GameObject cardPrefab;
	public GameObject currentCard;
	public float sideCardStep;
	float sideCarCounter = 0f;
	Card cardTextfield;
	Deck newDeck;

	//output variables
	string fileName;

	public List<GameObject> decks;

	// Use this for initialization
	void Start () {
		newDeck = GameObject.Find("Deck").GetComponent<Deck>();
		decks = new List<GameObject>();
		DontDestroyOnLoad(this.gameObject);

	}
	
	// Update is called once per frame
	void Update () {
		if(currentCard != null){
			WriteToCard();
		}
	}

	public void newCard(){
		if(currentCard == null){
			currentCard = (GameObject)Instantiate(cardPrefab, new Vector3(0f,0f,0f), Quaternion.identity);
			currentCard.transform.parent = GameObject.Find("Canvas").transform;
		}

	}

	void WriteToCard(){
		cardTextfield = currentCard.GetComponent<Card>();
		if(cardTextfield.myState == Card.cardState.front){
			cardTextfield.questionAnswer[0] += Input.inputString;
		}
		else if(cardTextfield.myState == Card.cardState.back){
			cardTextfield.questionAnswer[1] += Input.inputString;
		}

	}

	public void SaveCurrentCard(){
		//MoveCardToSide();
		newDeck.addCard(currentCard);
		currentCard.SetActive(false);
		currentCard = null;
	}

	void MoveCardToSide(){
		currentCard.transform.parent = GameObject.Find("SideCardParent").transform;
		currentCard.gameObject.GetComponent<RectTransform>().position = new Vector3(0f, sideCarCounter, 0f);
		sideCarCounter+=sideCardStep;
	}

	public void SaveAndQuit(){
		decks.Add(newDeck.gameObject);
		Application.LoadLevel(0);
	}

//	public TextAsset OutputDeckToFile(string fileName){
//		string dir = "Assets/Resources/" + fileName + ".csv";
//
//	}
	

}
