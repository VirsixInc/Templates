using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MatchingCard : MonoBehaviour {


	WordplayManager myManager;
	public string myWords = "";
	public bool selected;
	public string myMatch = ""; //set which term or def matching this term or def
	public int ID;
	public int correctlyMatched;
	public int incorrectlyMatched;

	public int consecutiveCorrectMatches = 0;

	public GameObject x, check;
	FadeIn fadein;

	Button myButton;

	// Use this for initialization
	void Start () {
		//gameObject.GetComponent<Sprite>();
		myButton = gameObject.GetComponent<Button>();
		myButton.onClick.AddListener(() => {SendMeToManager();});
		myManager = GameObject.Find("CardManager").GetComponent<WordplayManager>();
		fadein = gameObject.GetComponent<FadeIn>();
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.GetComponentInChildren<Text>().text = myWords;

	}

	public void SendMeToManager(){
		if(myManager.myState == WordplayManager.wordplayStates.pickACard){
			if(myManager.selected1 == null){
				myManager.selected1 = this.gameObject;
			}
			
		}
		
//		if(myManager.myState == MatchingManager.matchingGameStates.matchACard){
//			if(myManager.selected2 == null){
//				myManager.selected2 = this.gameObject;
//			}
//		}
	}

	public void GraphicFadeIn(bool win){
		GameObject objToInstantiate;
		if(win){
			objToInstantiate = (GameObject)Instantiate(check);
			//Instantiate(check);
			

		} else {
			objToInstantiate = (GameObject)Instantiate(x);
			//Instantiate(x);
		}


		objToInstantiate.transform.parent = this.transform;
		objToInstantiate.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f,0f);
		objToInstantiate.GetComponent<FadeIn1>().SetOutline();
		



//		print(sprite.name);
//		fadein.image.sprite = sprite;
//		fadein.StartFade();
	}

	public void Reset(){
		myWords = null;
		selected = false;
		myMatch = null;

	}
}
