using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public enum HotSpotPhase {Elements,Typing,Groups};
public enum HotSpotGameState {Config, SetPhase, Display, Playing, CheckMastery, NextQuestion, Win}

public class HotSpotsGame : MonoBehaviour {

	public Text promptText;
	public static HotSpotsGame s_instance;
	HotSpotPhase curPhase = HotSpotPhase.Elements;
	HotSpotGameState curState = HotSpotGameState.Config;
	GameObject[] individualElements, groups;
	List<ItemToBeMastered> phaseOneObjs, phaseTwoObjs, phaseThreeObjs, unmasteredItems;
	int currentIndex;
	string currentCorrectAnswer;
	List<Image> currentlyActivatedImages;
	bool hasAnsweredCorrect = false, masteryChecked = false;
	void Awake () {
		s_instance = this;
	}	

	void Update () {
	
		switch (curState) {
		case HotSpotGameState.Config : 
			ConfigGameData();
			curState = HotSpotGameState.SetPhase;
			break;

			//set phase should be set at the beginning of each phase, so at the beginning and each time the unmasteredItem count is 0
		case HotSpotGameState.SetPhase :
			SetPhase();
			curState = HotSpotGameState.Display;
			break;

		case HotSpotGameState.Display : 
			DisplayQuestion();
			curState = HotSpotGameState.Playing;
			break;
		case HotSpotGameState.Playing :
			if (hasAnsweredCorrect){
				hasAnsweredCorrect = false;
				curState = HotSpotGameState.CheckMastery;
			}

			break;
		case HotSpotGameState.CheckMastery :
			//if CheckForM returns true, 
			if (CheckForMastery() && curPhase!=HotSpotPhase.Groups) {
				curPhase++;
				curState = HotSpotGameState.SetPhase;
			}
			else if (CheckForMastery() && curPhase==HotSpotPhase.Groups){
				//win
			}
			else {
				curState = HotSpotGameState.Playing;
			}
			break;
		}
	}



	bool CheckForMastery() {
		if (currentIndex >= unmasteredItems.Count)
			currentIndex = 0; //loop around to beginning of list
		while (unmasteredItems[currentIndex].sequenceMastery==1f && unmasteredItems.Count != 0) { //skip over completed 
			unmasteredItems.Remove(unmasteredItems[currentIndex]);
			if (unmasteredItems.Count > currentIndex+1) {
				currentIndex++;
			}
			else 
				currentIndex = 0;
		}
		if (unmasteredItems.Count == 0) {
			return true; //phase complete
		} else {
			return false;
		}
	}
	void SetPhase() {
		switch (curPhase) {
		case HotSpotPhase.Elements :
			unmasteredItems.Clear();
			unmasteredItems = new List<ItemToBeMastered>(phaseOneObjs);
			break;
		case HotSpotPhase.Typing :
			unmasteredItems.Clear();
			unmasteredItems = new List<ItemToBeMastered>(phaseTwoObjs);
			break;

		case HotSpotPhase.Groups :
			unmasteredItems.Clear();
			unmasteredItems = new List<ItemToBeMastered>(phaseThreeObjs);
			break;

		}
		//clear curList
		//copy other list

	}

	void ConfigGameData() {
		phaseOneObjs = new List<ItemToBeMastered> ();
		phaseTwoObjs = new List<ItemToBeMastered> ();
		phaseThreeObjs = new List<ItemToBeMastered> ();
		unmasteredItems = new List<ItemToBeMastered> ();
		individualElements = GameObject.FindGameObjectsWithTag("elements");
		groups = GameObject.FindGameObjectsWithTag("groups");
		foreach (GameObject go in individualElements){
			ItemToBeMastered item = new ItemToBeMastered(0f, go);
			phaseOneObjs.Add(item);
			phaseTwoObjs.Add(item);
		}
		foreach (GameObject go in groups){
			ItemToBeMastered item = new ItemToBeMastered(0f, go);
			phaseThreeObjs.Add(item);
		}
		
		//sort all the lists alphabetically
		List<ItemToBeMastered> tempList = new List<ItemToBeMastered>();
		tempList = phaseOneObjs.OrderBy(item => item.itemGameObject.name).ToList();
		phaseOneObjs = new List<ItemToBeMastered>(tempList);
		
		List<ItemToBeMastered> tempList2 = new List<ItemToBeMastered>();
		tempList2 = phaseTwoObjs.OrderBy(item => item.itemGameObject.name).ToList();
		phaseTwoObjs = new List<ItemToBeMastered>(tempList2);
		
		List<ItemToBeMastered> tempList3 = new List<ItemToBeMastered>();
		tempList3 = phaseThreeObjs.OrderBy(item => item.itemGameObject.name).ToList();
		phaseThreeObjs = new List<ItemToBeMastered>(tempList3);
	}

	void DisplayQuestion(){
		currentCorrectAnswer = phaseOneObjs[currentIndex].itemGameObject.name; //correct answer is gameobject name at index in list of items
		promptText.text = currentCorrectAnswer;
		List<int> randIndexList = new List<int>(); //to avoid duplicates
		currentlyActivatedImages = new List<Image> (); //to clear at end

		switch (curPhase) {
		case HotSpotPhase.Elements :
			currentlyActivatedImages.Add (unmasteredItems[currentIndex].itemGameObject.GetComponent<Image>()); // add correct answer image to list

			for (int i = 0; i < phaseOneObjs.Count; i++){
				randIndexList.Add (i); //generate a list of numbers
			}
			randIndexList.Remove(currentIndex);//remove that int so it cant be chosen again

			if (unmasteredItems[currentIndex].sequenceMastery < 0.5f){
				for (int i = 0; i < 2; i++) { //choose 2 additional items to be displayed as wrong answers
					int randomInt = Random.Range(0, randIndexList.Count);
					currentlyActivatedImages.Add (phaseOneObjs[randIndexList[randomInt]].itemGameObject.GetComponent<Image>()); //add in random wrong answer
					randIndexList.Remove(randIndexList[randomInt]); //make sure it doesnt get added twice
				}
			}
			else {
				for (int i = 0; i < 4; i++) { //choose 4 additional items to be dispayed as wrong answer
					int randomInt = Random.Range(0, randIndexList.Count);
					currentlyActivatedImages.Add (phaseOneObjs[randIndexList[randomInt]].itemGameObject.GetComponent<Image>()); //add in random wrong answer
					randIndexList.Remove(randIndexList[randomInt]); //make sure it doesnt get added twice
				}
			}

			//display elements that can be clicked on
			foreach (Image image in currentlyActivatedImages) {
				image.enabled = true;
			}

			break;
		
		//TYPING
			
		case HotSpotPhase.Typing :
			currentCorrectAnswer = phaseThreeObjs[currentIndex].itemGameObject.name;
			
			
			break;

		//GROUPS
		
		case HotSpotPhase.Groups :
			currentCorrectAnswer = phaseTwoObjs[currentIndex].itemGameObject.name;

			break;
		
		

		
		}
	}

	public void SubmitAnswer (string answer) {
		if (answer == currentCorrectAnswer) {
			AnswerCorrect();
		}

		else {
			AnswerWrong();
		}

	}

	void AnswerCorrect(){
		unmasteredItems [currentIndex].sequenceMastery += .5f;
		ClearGUIObjects ();
		hasAnsweredCorrect = true;

	}

	void AnswerWrong(){}

	void ClearGUIObjects() {
		foreach (Image x in currentlyActivatedImages) {
			x.enabled = false;
		}
		currentlyActivatedImages.Clear ();
	}
}