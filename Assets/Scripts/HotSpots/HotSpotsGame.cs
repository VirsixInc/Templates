using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public enum HotSpotPhase {Elements,Typing,Groups};
public enum HotSpotGameState {Config, Display, Playing, AnswerSelected, CheckMastery, NextQuestion, Win}

public class HotSpotsGame : MonoBehaviour {

	public Text promptText;
	public static HotSpotsGame s_instance;
	HotSpotPhase curPhase = HotSpotPhase.Elements;
	HotSpotGameState curState = HotSpotGameState.Config;
	GameObject[] individualElements, groups;
	List<ItemToBeMastered> phaseOneObjs, phaseTwoObjs, phaseThreeObjs, currentPhase;
	int currentIndex;
	string currentCorrectAnswer;
	List<Image> currentlyActivatedImages;

	void Awake () {
	
	}	

	void Update () {
	
		switch (curState) {
		case HotSpotGameState.Config : 
			ConfigGameData();
			curState = HotSpotGameState.Display;
			break;
		case HotSpotGameState.Display : 
			DisplayQuestion();
			curState = HotSpotGameState.Playing;
			break;
		}
	}



	void CheckForMastery() {
		if (currentIndex >= currentPhase.Count)
			currentIndex = 0; //loop around to beginning of list
		while (currentPhase[currentIndex].sequenceMastery==1f && currentPhase.Count != 0) { //skip over completed 
			currentPhase.Remove(currentPhase[currentIndex]);
			if (currentPhase.Count > currentIndex+1) {
				currentIndex++;
			}
			else 
				currentIndex = 0;
		}
	}
	void SetPhase() {

		//clear curList
		//copy other list

	}

	void ConfigGameData() {
		phaseOneObjs = new List<ItemToBeMastered> ();
		phaseTwoObjs = new List<ItemToBeMastered> ();
		phaseThreeObjs = new List<ItemToBeMastered> ();
		currentPhase = new List<ItemToBeMastered> ();
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
		
		//			tempList.Clear (); //professional memory management, prob unnecessary but good to know
		//			tempList2.Clear ();
		//			tempList3.Clear ();
		//
		//			tempList.TrimExcess();
		//			tempList2.TrimExcess();
		//			tempList3.TrimExcess();
		
	}

	void DisplayQuestion(){
		List<int> randIndexList = new List<int>(); //to avoid duplicates
		currentlyActivatedImages = new List<Image> (); //to clear at end

		switch (curPhase) {
		case HotSpotPhase.Elements :
			currentlyActivatedImages.Add (phaseOneObjs[currentIndex].itemGameObject.GetComponent<Image>()); // add correct answer image to list
			currentCorrectAnswer = phaseOneObjs[currentIndex].itemGameObject.name; //correct answer is gameobject name at index in list of items
			for (int i = 0; i < phaseOneObjs.Count; i++){
				randIndexList.Add (i); //generate a list of numbers
			}
			randIndexList.Remove(currentIndex);//remove that int so it cant be chosen again

			if (phaseOneObjs[currentIndex].sequenceMastery < 0.5f){
				for (int i = 0; i < 2; i++) { //choose 2 additional items to be dispayed as wrong answers
					int randomInt = Random.Range(0, randIndexList.Count);
					currentlyActivatedImages.Add (phaseOneObjs[randIndexList[randomInt]].itemGameObject.GetComponent<Image>()); //add in random wrong answer
					randIndexList.Remove(randomInt); //make sure it doesnt get added twice
				}
			}
			else {
				for (int i = 0; i < 4; i++) { //choose 4 additional items to be dispayed as wrong answer
					int randomInt = Random.Range(0, randIndexList.Count);
					currentlyActivatedImages.Add (phaseOneObjs[randIndexList[randomInt]].itemGameObject.GetComponent<Image>()); //add in random wrong answer
					randIndexList.Remove(randomInt); //make sure it doesnt get added twice
				}
			}

			foreach (Image image in currentlyActivatedImages) {
				image.enabled = true;
			}

			break;
		case HotSpotPhase.Groups :
			currentCorrectAnswer = phaseTwoObjs[currentIndex].itemGameObject.name;

			break;
				
		case HotSpotPhase.Typing :
			currentCorrectAnswer = phaseThreeObjs[currentIndex].itemGameObject.name;


			break;

		
		}
	}

	public void SubmitAnswer (string answer) {
		print ("SUBMIT ANSWER");
		if (answer == currentCorrectAnswer) {
			AnswerCorrect();
		}

		else {
			AnswerWrong();
		}

	}

	void AnswerCorrect(){}

	void AnswerWrong(){}
}