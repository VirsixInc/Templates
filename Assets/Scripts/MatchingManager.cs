using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class MatchingManager : MonoBehaviour {

	public enum matchingGameStates {waitingForSetup, pickACard, matchACard, checkTheMatch, reset};
	public matchingGameStates myState;

	public GameObject selected1, selected2;

	int rightAnswers = 0;
	int wrongAnswers = 0;

	int difficulty = 4;
	public int defSetIndex = 0;
	public int tempSetIndex = 0;
	MatchingCard[] termHolderay, defHolderay;
	List<MatchingCard> termHolder, defHolder;
	List<string> terms, defs; 

	Timer timer;
	
	bool throwingAway;

	List<MatchingCard> allMatches;

	public CardManager myCardManager;
	List<string[]> cardList;

	GameObject incorrectBackdrop, correctBackdrop, movingBackdrop;
	float backdropDistance;
	Vector3 backdropStartPos;





	// Use this for initialization
	void Start () {
		myCardManager = GameObject.Find("CardManager").GetComponent<CardManager>();
		allMatches = new List<MatchingCard>();
		terms = new List<string>();
		defs = new List<string>();
		termHolder = new List<MatchingCard>();
		defHolder = new List<MatchingCard>();

//				termHolderay = GameObject.Find("Terms").GetComponentsInChildren<MatchingCard>();
//				defHolderay = GameObject.Find("Defs").GetComponentsInChildren<MatchingCard>();
		myState = matchingGameStates.waitingForSetup;

		incorrectBackdrop = GameObject.Find("LoseScreen");
		correctBackdrop = GameObject.Find("WinScreen");
		movingBackdrop = GameObject.Find("OpacitySlider");

		backdropStartPos = movingBackdrop.transform.position;
		backdropDistance = Vector3.Distance(movingBackdrop.transform.position, Vector3.zero);
	}
	
	// Update is called once per frame
	void Update () {
		if(timer != null && myState != matchingGameStates.reset){
			float fracJourney = timer.elapsedTime/timer.startTime;
			movingBackdrop.transform.position = Vector3.Lerp(backdropStartPos, new Vector3(backdropStartPos.x, (backdropStartPos.y/10f), backdropStartPos.z), fracJourney);
		}


		switch(myState){
		case matchingGameStates.waitingForSetup:
			if(myCardManager.matchingIsSetUp){
				cardList = myCardManager.cards;
				//fillTermsAndDefs();
				timer = GameObject.Find("Timer").GetComponent<Timer>();

				SetMatches();
				fillHolder();
				myState = matchingGameStates.reset;
			}
			break;
		case matchingGameStates.reset: 

			ResetAllMatchingCards();
			Shfl(allMatches);
//			foreach(MatchingCard card in allMatches){
//				print (card.myWords + "     r    " + card.myMatch);
//			}

			int counter = 0;
			List<int> indexes = new List<int>();
			while(indexes.Count < difficulty){
				indexes.Add (indexes.Count);
			}
			for(int i = 0; i < difficulty; i++){
				int temp = UnityEngine.Random.Range(0, indexes.Count);
				for(int j = 0; j < 4; j++){
					if(defHolder[j].ID == temp){
						termHolder[i].myWords = allMatches[i].myWords;
						termHolder[i].myMatch = allMatches[i].myMatch;
						defHolder[indexes[temp]].myWords = allMatches[i].myMatch;
						defHolder[indexes[temp]].myMatch = allMatches[i].myWords;
						indexes.Remove(indexes[temp]);
					}
				}
			}

			//print(cardList.Count);
//			for(int i = 0; i<cardList.Count-1; i++){
//				if(cardList[i].Length < 1)
//				Debug.Log(cardList[i][0] + "      " + cardList[i][1]);
//			}

			myState = matchingGameStates.pickACard;

			break;
		case matchingGameStates.pickACard:
			if(selected1 != null){
				myState = matchingGameStates.matchACard;
			}
			break;
		case matchingGameStates.matchACard:
			if(selected2 != null){
				myState = matchingGameStates.checkTheMatch;
			}
			break;
		case matchingGameStates.checkTheMatch:

			if(selected1.GetComponent<MatchingCard>().myMatch == selected2.GetComponent<MatchingCard>().myWords){
				print ("TIGHT TIGHT TIGHT");
				timer.TrackWinLoss(true);
				foreach(MatchingCard card in allMatches){
					if(card.myWords == selected1.GetComponent<MatchingCard>().myWords){
						card.consecutiveCorrectMatches++;
					}
				}
				rightAnswers++;


			}
			else{
				print("WACK WACK WACK");
				timer.TrackWinLoss(false);
				foreach(MatchingCard card in allMatches){
					if(card.myWords == selected1.GetComponent<MatchingCard>().myWords){
						card.consecutiveCorrectMatches=0;
					}
				}
				wrongAnswers++;

			}
			GameObject.Find("Terms").GetComponent<CanvasGroup>().interactable = false;
			GameObject.Find("Defs").GetComponent<CanvasGroup>().interactable = false;
			selected1 = null;
			selected2 = null;
			timer.throwingAway = true;
			throwOutCards();

			Reset();

			break;
		}
	}
	


	//not my shuffle. thx internet ;)
	void Shfl<T>(List<T> list){
		System.Random rng = new System.Random();
		int n = list.Count;
		while(n > 1){
			n--;
			int k = rng.Next(n+1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

//	void fillTermsAndDefs(){
//		terms.Clear();
//		defs.Clear();
//		foreach(string[] card in cardList){
//			if(card.Length > 1){
//				terms.Add(card[0]);
//				defs.Add(card[1]);
//			}
//		}
//	}

	void fillHolder(){

		MatchingCard[] temp = GameObject.Find("Terms").GetComponentsInChildren<MatchingCard>();
		foreach(MatchingCard obj in temp){
			termHolder.Add(obj.GetComponent<MatchingCard>());
		}

		MatchingCard[] temp2 = GameObject.Find("Defs").GetComponentsInChildren<MatchingCard>();
		foreach(MatchingCard obj in temp2){
			defHolder.Add(obj.GetComponent<MatchingCard>());
		}
	}

	void SetMatches(){
		for(int i = 0; i < cardList.Count-1; i++){
			MatchingCard temp = new MatchingCard();
			temp.myWords = cardList[i][0];
			temp.myMatch = cardList[i][1];
			allMatches.Add (temp);
			//print(allMatches[i].myWords + "       " + allMatches[i].myMatch);
		}
	}

	void ResetAllMatchingCards(){
		foreach(MatchingCard card in termHolder){
			card.Reset();
		}
		foreach(MatchingCard card in defHolder){
			card.Reset();
		}
	}

	//TODO: for some reason im not throwing out the right card. Has to do with the way list.Remove workds. 
	//I have tried giving it the instance i want to remove, and the index of that instance in the list. 
	//neither produced the right results. 
	//Appears to be acting weird now because Reset is being triggered before this function finnishes. 
	//Execution order is a bitch
	//must force reset to go after throwioutcards is finished
	//might take encapsulating everything in the reset state into a coroutine or function. 
	//execution order is wack
	void throwOutCards(){

		int index = 0;
		foreach(MatchingCard card in allMatches){
			if(card.consecutiveCorrectMatches > 3){
//				print ("I AM THROWING AWAY " + card.myWords);
//				foreach(MatchingCard card2 in allMatches){
//					print (card2.myWords + "         " + card2.myMatch);
//				}
//				print ("BEFORE       " + allMatches.Count);
				allMatches.RemoveAt(index);
//				print ("AFTER      " + allMatches.Count);
//				foreach(MatchingCard card2 in allMatches){
//					print (card2.myWords + "         " + card2.myMatch);
//				}
			}
			index++;
		}
		timer.throwingAway = false;
	}

	public void Reset(){
		
		if(timer.returnMostRecentWinLoss()){
			StartCoroutine(fadeOut(correctBackdrop, 255f, 0f, 1f));
		}
		else if (!timer.returnMostRecentWinLoss()) {
			StartCoroutine(fadeOut(incorrectBackdrop, 255f, 0f, 1f));
		}
		timer.Reset();
		movingBackdrop.transform.position = backdropStartPos;
		GameObject.Find("Terms").GetComponent<CanvasGroup>().interactable = true;
		GameObject.Find("Defs").GetComponent<CanvasGroup>().interactable = true;

		GameObject.Find("GameManager").GetComponent<MatchingManager>().myState = MatchingManager.matchingGameStates.reset;
	}

	public IEnumerator fadeOut(GameObject bg, float alphaStart, float alphaFinish, float time){
		float elapsedTime = 0f;
		
		Color temp = bg.GetComponent<Image>().color;
		temp = new Color(temp.r, temp.g, temp.b, alphaStart);
		bg.GetComponent<Image>().color = temp;
		print(elapsedTime + "         " + time);
		while(elapsedTime < time){
			elapsedTime += Time.deltaTime;
			bg.GetComponent<Image>().color = new Color(temp.r, temp.g, temp.b, (Mathf.Lerp(temp.a, alphaFinish, (elapsedTime/time))));
			yield return new WaitForEndOfFrame();
			
		}
		bg.GetComponent<Image>().color = new Color(temp.r, temp.g, temp.b, 0f);
		
	}

}
