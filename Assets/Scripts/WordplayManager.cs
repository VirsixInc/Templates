using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class WordplayManager : MonoBehaviour {

	public enum wordplayStates {waiting, pickACard, checkTheMatch, reset1, keyboardReset, keyboardPhase, keyboardCheck, fadeTime, keyboardFade};
	public wordplayStates myState;
	GameObject Selected;
	int difficulty = 2;
	List<MatchingCard> termHolder, defHolder;
	Timer timer;
	List<MatchingCard> allMatches;

	public GameObject selected1;
	int termIndex;

	int levelOneThresh = 0;
	int levelTwoThresh = 2;
	int levelThreeThresh = 4;

	string myTerm, myAnswer;

	MatchingCard[,] defDisplay = new MatchingCard[3,3];

	public CardManager myCardManager;
	List<string[]> cardList;
	
	GameObject incorrectBackdrop, correctBackdrop, movingBackdrop;
	float backdropDistance;
	Vector3 backdropStartPos;
	GameObject phase1;
	//PHASE2
	GameObject phase2;
	InputField myInputField;
	public string inputText;
	MatchingCard keyboardTerm;
	public string keyboardAnswer;
	
	Slider masteryBar;
	GameObject masteryFill;
	Sprite buttonImage;

	Scrollbar timerScrollbar;

	public TextAsset myCSV;


	//for keyboard fadein/lerp
	float keyboardFadeTimer = 2f;
	Color keyboardTermStartColor;
	Outline keyboardTermOutline;
	GameObject incorrectKeyboardPrompt;
	public GameObject keyboardX;
	public GameObject keyboardCheck;
	bool keyboardContinue = false;
	// Use this for initialization
	void Start () {
		myCardManager = GameObject.Find("CardManager").GetComponent<CardManager>();
		allMatches = new List<MatchingCard>();
		termHolder = new List<MatchingCard>();
		defHolder = new List<MatchingCard>();
		myState = wordplayStates.waiting;
		buttonImage = GameObject.Find("Terms").GetComponent<Image>().sprite;
		
//		incorrectBackdrop = GameObject.Find("LoseScreen");
//		correctBackdrop = GameObject.Find("WinScreen");
//		movingBackdrop = GameObject.Find("OpacitySlider");
		
//		backdropStartPos = movingBackdrop.transform.position;
//		backdropDistance = Vector3.Distance(movingBackdrop.transform.position, Vector3.zero);

		phase1 = GameObject.Find("Phase1");
		masteryBar = GameObject.Find("Mastery1").GetComponent<Slider>();
		masteryFill = GameObject.Find("Fill");
		masteryFill.SetActive(false);
		timerScrollbar = GameObject.Find("Scrollbar").GetComponent<Scrollbar>();
		//PHASE2
		phase2 = GameObject.Find("Phase2");
		phase2.SetActive(false);
		keyboardX.SetActive(false);
		keyboardCheck.SetActive(false);

	}
	
	// Update is called once per frame
	void Update () {
		//timer graphic
		if(timer != null){
			timerScrollbar.value = 1 - timer.elapsedTime/timer.startTime;
		}
		switch(myState){
		case wordplayStates.waiting:
			if(myCardManager.wordplayIsSetUp){
				if(myCardManager.wordplayIsSetUp){
					cardList = parseCSV(myCSV);
					//cardList = myCardManager.cards;
					//fillTermsAndDefs();
					timer = GameObject.Find("Timer").GetComponent<Timer>();
					
					SetMatches();
					fillHolder();
					GameObject.Find("Defs").SetActive(true);
					//Get 2d array of gameobjects
					//note: this is probably horrible
					for(int i = 0; i < defDisplay.GetLength(0); i++){
						string temp = ("" + i);
						print ("" + i);
						MatchingCard[] array = GameObject.Find(temp).GetComponentsInChildren<MatchingCard>();
						for(int j = 0; j < defDisplay.GetLength(1); j++){
							defDisplay[i,j] = array[j];
						}
					}
					myState = wordplayStates.reset1;
				}
			}
			break;
		case wordplayStates.fadeTime:
			timer.pause = true;
			int fadeCounter = 0;
			foreach(MatchingCard card in defDisplay){
				if(card.GetComponentInChildren<FadeIn>() != null){
					fadeCounter++;
				}
			}
			if(fadeCounter == 0){
				timer.Reset();
				myState = wordplayStates.reset1;
			}

			break;
		case wordplayStates.pickACard:
			if(selected1 != null){
				myState = wordplayStates.checkTheMatch;
			}
			if(timer.timerIsElapsed){
				myState = wordplayStates.checkTheMatch;
			}
			break;
		case wordplayStates.checkTheMatch:

			foreach(MatchingCard card in defDisplay){
				if(card.gameObject.activeSelf){
					if(card.myWords == myAnswer){
						card.GraphicFadeIn(true); 
					}else{
						card.GraphicFadeIn(false); 
					}
				}

			}

			if(selected1 != null){

				if(selected1.GetComponent<MatchingCard>().myWords == myAnswer){
					allMatches[termIndex].consecutiveCorrectMatches++;
					timer.TrackWinLoss(true);
					SoundManager.s_instance.PlaySound(SoundManager.s_instance.m_correct);
					selected1 = null;
					//timer.Reset();
					myState = wordplayStates.fadeTime;
					break;
				}
				else{
					if(allMatches[termIndex].consecutiveCorrectMatches < 6){
						allMatches[termIndex].consecutiveCorrectMatches = 0;
					}
					timer.TrackWinLoss(false);
					SoundManager.s_instance.PlaySound(SoundManager.s_instance.m_wrong);
					selected1 = null;
					//timer.Reset();
					myState = wordplayStates.fadeTime;
					break;
				}
			}
			else{
				if(allMatches[termIndex].consecutiveCorrectMatches < 6){
					allMatches[termIndex].consecutiveCorrectMatches = 0;
				}
				timer.TrackWinLoss(false);
				SoundManager.s_instance.PlaySound(SoundManager.s_instance.m_wrong);
				selected1 = null;
				timer.Reset();
				myState = wordplayStates.fadeTime;
				break;
			}
			
			break;
		case wordplayStates.reset1:
			//make randomness happen
			//ResetAllMatchingCards();
			timer.pause = false;
			if(!phase1.activeSelf){
				phase1.SetActive(true);
			}
			if(ReturnMastery() >= 1f){
				myState = wordplayStates.keyboardReset;
				break;
			}
			Shfl(allMatches);
			//after allmatches has been randomized, go through it in order and add the ones with less than 6 consecutive correct matches to a new list. 
			//use this second list to select the term, not the old one. everything else should be the same? yeah?

			List<MatchingCard> notYetMastered = new List<MatchingCard>();
			foreach(MatchingCard card in allMatches){
				if(card.consecutiveCorrectMatches < 6){
					notYetMastered.Add(card);
					print (card.myWords + "        " + card.myMatch);

				}
			}

			if(ReturnMastery() > 0f){
				masteryFill.SetActive(true);
				masteryBar.value = ReturnMastery();
			}




			//set the card we will be matching
			print(notYetMastered.Count);
			termIndex = UnityEngine.Random.Range(0, notYetMastered.Count);
			myTerm = notYetMastered[termIndex].myWords;
			myAnswer = notYetMastered[termIndex].myMatch;

			if(allMatches[termIndex].consecutiveCorrectMatches >= levelOneThresh){
				difficulty = 0;
			}
			if(allMatches[termIndex].consecutiveCorrectMatches >= levelTwoThresh){
				difficulty = 1;
			}
			if(allMatches[termIndex].consecutiveCorrectMatches >= levelThreeThresh){
				difficulty = 2;
			}

			List<int> indexes = new List<int>();
			while(indexes.Count < ((1+difficulty)*3)){
				indexes.Add (indexes.Count);
			}
			int termDisplayIndex = UnityEngine.Random.Range(0, indexes.Count);
			indexes.Remove(indexes[termDisplayIndex]);

			//display the card term to be matched
			GameObject termDisplay = GameObject.Find("Terms");
			termDisplay.SetActive(true);
			termDisplay.GetComponent<MatchingCard>().myWords = myTerm;
			//Set dificulty form atching cards cnsecutive matches
			//CODE CODE CODE THIS NEEDS CODE
			//activate correct number of rows based on difficulty

			//turn on all the definitions. this is probably chill to stay. 
			for(int i = 0; i < defDisplay.GetLength(0); i++){
				for(int j = 0; j < defDisplay.GetLength(1); j++){
					//print(i + "      " + j);
					if(i > difficulty){
						defDisplay[i,j].transform.gameObject.SetActive(false);
					}
					else{
						defDisplay[i,j].transform.gameObject.SetActive(true);
					}
				}
			}

			List<MatchingCard> defDisplayList = new List<MatchingCard>();
			for(int i = 0; i < defDisplay.GetLength(0); i++){
				for(int j = 0; j < defDisplay.GetLength(1); j++){
					if(defDisplay[i,j].gameObject.activeSelf){
						defDisplayList.Add(defDisplay[i,j]);
					}
				}
			}

			//print (allMatches.Count);	
			//print (defDisplayList.Count);
			defDisplayList[termDisplayIndex].myWords = myAnswer;

			for(int i = 0; i < defDisplayList.Count-1; i++){
				int temp = UnityEngine.Random.Range(0,indexes.Count);
				//print (allMatches[indexes[temp]].myWords);
				//print (indexes[temp]);

				defDisplayList[indexes[temp]].myWords = allMatches[indexes[temp]].myMatch;// + "  " + allMatches[indexes[temp]].consecutiveCorrectMatches;
				indexes.RemoveAt(temp);

//				string str = "";
//				foreach(int num in indexes){
//					str = str + ", " + num;
//				}
//				print (str);
//				str = "";

			}

			myState = wordplayStates.pickACard;

			break;

		case wordplayStates.keyboardReset:
			//get to keyboard phase. Turn on the gameobjects needed. Probably turn off phase 1
			if(phase2.activeSelf == false){
				GameObject.Find("Phase1").SetActive(false);
				phase2.SetActive(true);
				timer = GameObject.Find("Timer2").GetComponent<Timer>();
				myInputField = GameObject.Find("InputField").GetComponent<InputField>();
				inputText = GameObject.Find("InputText").GetComponent<Text>().text; //might not be the right reference
				keyboardTerm = GameObject.Find("KeyboardTerm").GetComponent<MatchingCard>();
				masteryBar = GameObject.Find("Mastery2").GetComponent<Slider>();
				keyboardTermOutline = keyboardTerm.gameObject.GetComponent<Outline>();
				keyboardTermStartColor = keyboardTermOutline.effectColor;
				incorrectKeyboardPrompt = GameObject.Find("Incorrect");
				incorrectKeyboardPrompt.SetActive(false);
			}
			timer.pause = false;
			Shfl(allMatches); //randomize
			int keyboardTermIndex = UnityEngine.Random.Range(0, allMatches.Count); //the index of our term we are matching (typing)
			keyboardTerm.myWords = allMatches[keyboardTermIndex].myWords;
			keyboardAnswer = allMatches[keyboardTermIndex].myMatch;
			print (inputText);
			myInputField.text = ""; //this might be the wrong reference. 
			myState = wordplayStates.keyboardPhase;
			break;

		case wordplayStates.keyboardPhase:
//			if(myInputField.text == keyboardAnswer){
//				timer.TrackWinLoss(true);
//				timer.Reset();
//				myState = wordplayStates.keyboardReset;
//			}
			if(timer.elapsedTime > timer.startTime){
				wrongKeyboardInput();
			}
			break;

		case wordplayStates.keyboardFade:


			if(keyboardFadeTimer == 2f){
				GameObject.Find("Solution").GetComponent<Text>().text = keyboardAnswer;
				if(timer.returnMostRecentWinLoss()){
					keyboardTerm.gameObject.GetComponent<Outline>().effectColor = Color.green;
					keyboardCheck.SetActive(true);
				}
				else if(!timer.returnMostRecentWinLoss()){
					incorrectKeyboardPrompt.SetActive(true);
					keyboardX.SetActive(true);
					keyboardTerm.gameObject.GetComponent<Outline>().effectColor = Color.red;
				}
			}

			keyboardFadeTimer -= Time.deltaTime;

			if(keyboardFadeTimer <= 0f && timer.returnMostRecentWinLoss()){
				incorrectKeyboardPrompt.SetActive(false);
				keyboardCheck.SetActive(false);
				keyboardFadeTimer = 2f;
				GameObject.Find("Solution").GetComponent<Text>().text = "";
				timer.Reset();
				keyboardTerm.gameObject.GetComponent<Outline>().effectColor = keyboardTermStartColor; 
				myState = wordplayStates.keyboardReset;
			}
			else if(!timer.returnMostRecentWinLoss()){
				if(myInputField.text == keyboardAnswer){
					incorrectKeyboardPrompt.SetActive(false);
					keyboardFadeTimer = 2f;
					keyboardX.SetActive(false);
					GameObject.Find("Solution").GetComponent<Text>().text = "";

					keyboardTerm.gameObject.GetComponent<Outline>().effectColor = keyboardTermStartColor; 
					timer.Reset();
					myState = wordplayStates.keyboardReset;
				}
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
		
		for(int i = 0; i < listToReturn.Count; i++){
			for(int j = 0; j < listToReturn[i].Length; j++){
				string temp = listToReturn[i][j].Replace('|',',');
				listToReturn[i][j] = temp;
			}
		}
		
		return listToReturn;
	}

	//Phase2 Functions
	public void wrongKeyboardInput(){
		timer.pause = true;
		if(myInputField.text == keyboardAnswer){
			SoundManager.s_instance.PlaySound(SoundManager.s_instance.m_correct);
			timer.TrackWinLoss(true);
		}
		else{
			SoundManager.s_instance.PlaySound(SoundManager.s_instance.m_wrong);
			timer.TrackWinLoss(false);
		}
		myInputField.text = "";
		timer.Reset();
		myState = wordplayStates.keyboardFade;
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

	public void LoadMainMenu() {
		Application.LoadLevel("AssignmentMenu");
		
	}

	void SetMatches(){
		for(int i = 0; i < cardList.Count-1; i++){
			MatchingCard temp = new MatchingCard();
			temp.myWords = cardList[i][0];
			temp.myMatch = cardList[i][1];
			allMatches.Add (temp);
			print(allMatches[i].myWords + "       " + allMatches[i].myMatch);
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

	float ReturnMastery(){
		int cardTotal = allMatches.Count;
		float totalPossibleMastery = cardTotal * (levelThreeThresh + 2f);
		float currentMastery = 0f;
		foreach(MatchingCard card in allMatches){
			currentMastery += card.consecutiveCorrectMatches;
		}

		return (currentMastery/totalPossibleMastery);
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
		print("FADING OUT");
		float elapsedTime = 0f;
		
		Color temp = bg.GetComponent<Image>().color;
		temp = new Color(temp.r, temp.g, temp.b, alphaStart);
		bg.GetComponent<Image>().color = temp;
		print(elapsedTime + "         " + time);
		while(elapsedTime < time){
			print("colorlerp :(");
			elapsedTime += Time.deltaTime;
			bg.GetComponent<Image>().color = new Color(temp.r, temp.g, temp.b, (Mathf.Lerp(temp.a, alphaFinish, (elapsedTime/time))));
			yield return new WaitForEndOfFrame();
		}
		bg.GetComponent<Image>().color = new Color(temp.r, temp.g, temp.b, 0f);
		print("fadeout done!");
		
	}
}
