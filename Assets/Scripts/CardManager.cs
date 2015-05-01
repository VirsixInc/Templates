using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class CardManager : MonoBehaviour {
	public TextAsset myCSV;
	public List<string[]> cards;
	public GameObject cardPrefab;
	public GameObject deckDisplayPrefab;
	public DirectoryInfo srcdddd;
	float cardSpacing = 0f;

	bool reviewIsSetUp = false;
	bool selectIsSetUp = false;
	public bool matchingIsSetUp = false;
	public bool wordplayIsSetUp = false;

	public enum reviewStates {selectADeck, review, matchingGame, wordplay};
	Vector3 initCameraPos;
	public reviewStates myState;
	GameObject canvas;
	GameObject cardHolder;
	GameObject sliderControls;

	string debugtext = "";

	public CardAssets myCardAssets;

	Timer timer;

	GameObject Terms;
	GameObject Defs;
	
	// Use this for initialization
	void Start () {
		debugtext += "\n CardManager"; //delete later
		Debug.Log(myCardAssets.CSVs.Count);
		//myCardAssets.Parse();
		Debug.Log(myCardAssets.parsedCSVs.Count);
		initCameraPos = Camera.main.transform.position;
		cardHolder = GameObject.Find("CardHolder");
		sliderControls = GameObject.Find("GameManager");
		Terms = GameObject.Find("Terms");
		Defs = GameObject.Find("Defs");
		if(Application.loadedLevel == 3){
			Terms.SetActive(false);
			Defs.SetActive(false);
		}

		timer = GameObject.Find("Timer").GetComponent<Timer>();
		//timer.gameObject.SetActive(true);
		timer.pause = true;


	}
	
	// Update is called once per frame
	void Update () {
//		GameObject.Find("debugText").GetComponent<Text>().text = myState.ToString() + debugtext;
		switch(myState){
			case reviewStates.selectADeck:
				if(!selectIsSetUp){
				debugtext += "\n SelectIsSetUp";
					//DisplayAllDecks();
					DisplayCardAssets();
					selectIsSetUp = true;
				}
				if(myCSV != null || cards != null){
					//canvas = GameObject.Find("Canvas");
					Transform tempTransform = cardHolder.transform;
					foreach(Transform child in tempTransform){
//						if(child.gameObject.name == "Menu" || ){
//							continue;
//						}
				//		else{
							child.gameObject.SetActive(false);
	//					}
					}
				if(Application.loadedLevelName == "Cards"){
					myState = reviewStates.review;
					cardSpacing = 0f;
					Camera.main.transform.position = initCameraPos;
				}
				if(Application.loadedLevelName == "Game"){
					myState = reviewStates.matchingGame;
					Camera.main.transform.position = initCameraPos;
					cardSpacing = 0f;
				}
				if(Application.loadedLevelName == "Wordplay" || Application.loadedLevelName == "DesignUpdate" || Application.loadedLevelName == "Wordplay5"){
					myState = reviewStates.wordplay;
					Camera.main.transform.position = initCameraPos;
					cardSpacing = 0f;
				}

				}
				break;
			case reviewStates.review:
			if(!reviewIsSetUp){
				cards = parseCSV(myCSV);
				CSVToCards();
				reviewIsSetUp = true;
			}
				break;
			case reviewStates.matchingGame: 
				if(!matchingIsSetUp){
					timer.gameObject.SetActive(true);
					Terms.SetActive(true);
					Defs.SetActive(true);
					sliderControls.GetComponent<ReviewManager>().enabled = false;
					cardSpacing = 1000f;
					cards = parseCSV(myCSV);
					matchingIsSetUp = true;
				}
				break;
		case reviewStates.wordplay:
			if(!wordplayIsSetUp){
				timer.gameObject.SetActive(true);
				Terms.SetActive(true);
				Defs.SetActive(true);
				sliderControls.GetComponent<ReviewManager>().enabled = false;
				cardSpacing = 1000f;
				//cards = parseCSV(myCSV);
				wordplayIsSetUp = true;
			}
			break;
		}
	}


	List<string[]> parseCSV(TextAsset csvToParse){
		List<string[]> listToReturn = new List<string[]>();
		string[] lines = csvToParse.text.Split('\n');
		for(int i = 0;i<lines.Length;i++){
			string[] currLine = lines[i].Split(',');
			listToReturn.Add(currLine);
		}
		return listToReturn;
	}

	//used in review mode. poorly named because the game use to only be review mode. 
	void CSVToCards() {

		foreach(string[] card in cards){
			GameObject temp = (GameObject)Instantiate(cardPrefab, new Vector3(cardSpacing,0,300), Quaternion.identity);
			temp.GetComponentInChildren<Card>().questionAnswer = card;
			temp.transform.parent = cardHolder.transform;
			RectTransform myRectTransform = (RectTransform)temp.transform;
			myRectTransform.anchoredPosition = new Vector2(myRectTransform.anchoredPosition.x, 0f);
			cardSpacing += 200f;
		}
	}

	void DisplayCardAssets(){
		for(int i = 0; i < myCardAssets.parsedCSVs.Count; i++){
			GameObject temp = (GameObject)Instantiate(deckDisplayPrefab, new Vector3(cardSpacing, 0f, 300f), Quaternion.identity);
			temp.transform.SetParent(cardHolder.transform, false);
			RectTransform myRectTransform = (RectTransform)temp.transform;
			myRectTransform.anchoredPosition = new Vector2(myRectTransform.anchoredPosition.x, 0f);
			temp.GetComponent<DeckSelect>().myCards = myCardAssets.parsedCSVs[i].Data;
			cardSpacing += 200f;
		}

//		foreach(List<string[]> csv in myCardAssets.parsedCSVs){
//			GameObject temp = (GameObject)Instantiate(deckDisplayPrefab, new Vector3(cardSpacing, 0f, 300f), Quaternion.identity);
//			temp.transform.SetParent(cardHolder.transform, false);
//			RectTransform myRectTransform = (RectTransform)temp.transform;
//			myRectTransform.anchoredPosition = new Vector2(myRectTransform.anchoredPosition.x, 0f);
//			temp.GetComponent<DeckSelect>().myCards = myCardAssets.parsedCSVs[0];
//			count++;
//
//		}
	}

	void DisplayAllDecks(){
		debugtext = debugtext + "\n DisplayAllDecksFunctionCalled";
		DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources");
		debugtext += "\n dir set to " + dir.FullName;
		debugtext += "\n" + Application.dataPath;
		debugtext += "\n persisten = " + Application.persistentDataPath;
		DirectoryInfo androidDir = new DirectoryInfo(Application.persistentDataPath);
		FileInfo[] androidInfo = androidDir.GetFiles("*");
		foreach(FileInfo file in androidInfo){
			debugtext += "\n" + file.FullName + ",  ";
		}
		FileInfo[] info = dir.GetFiles("*");
		debugtext = debugtext + "\n" + info.Length; //delete later
		foreach (FileInfo aFile in info){
			debugtext += "\n file";
			string fileName = aFile.Name;
			string[] array = fileName.Split('.');
			if(array.Length < 3){

				GameObject temp = (GameObject)Instantiate(deckDisplayPrefab, new Vector3(cardSpacing, 0f, 300f), Quaternion.identity);
				Debug.Log(temp.transform.position);
				debugtext = debugtext + "\n " + temp.transform.position; //delete later
				temp.transform.SetParent(cardHolder.transform, false);
				RectTransform myRectTransform = (RectTransform)temp.transform;
				myRectTransform.anchoredPosition = new Vector2(myRectTransform.anchoredPosition.x, 0f);
				temp.GetComponentInChildren<Text>().text = array[0];
				
				temp.GetComponent<DeckSelect>().aCSV = (TextAsset)Resources.Load(array[0]);
				cardSpacing += 1000f;
			}


		}

	}

//	void CheckParser(){
//		print(cards.Count);
//	}
}
