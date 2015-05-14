using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public enum HotSpotPhase {Elements,Typing,Groups};
public enum HotSpotGameState {Config,
							SetPhase,
							ConfigKeyboard,
							ConfigGroups,
							Display,
							Playing,
							CheckMastery,
							NextQuestion,
							Win};

public class HotSpotsGame : MonoBehaviour {

	public Slider masteryMeter;
	public Text promptText;
	public static HotSpotsGame s_instance;
	HotSpotPhase curPhase = HotSpotPhase.Typing;
	HotSpotGameState curState = HotSpotGameState.Config;
	GameObject[] individualElements, groups;
																			//unmasterItems is the copy of each of the phaseObjs depending on curPhase
	public List<ItemToBeMastered> phaseOneObjs, phaseTwoObjs, phaseThreeObjs, unmasteredItems;
	int currentIndex;
	string currentCorrectAnswer;
	List<Image> currentlyActivatedImages;
	bool hasAnsweredCorrect = false, masteryChecked = false;
	float totalTerms, completedTerms;
	//Keyboard members
	private bool handleCardPress, firstPress, handleKeyboardSubmit, firstSubmit;
	public InputField keyboardText;


	void Awake () {
		s_instance = this;
	}	

	void Update () {
		print (curPhase);
		print (curState);
		switch (curState) {
		case HotSpotGameState.Config : 
			ConfigGameData();
			curState = HotSpotGameState.SetPhase;
			break;
		case HotSpotGameState.SetPhase :
			SetPhase();
			if (curPhase == HotSpotPhase.Typing){
				curState = HotSpotGameState.ConfigKeyboard;
			}
			else if (curPhase == HotSpotPhase.Groups){
				curState = HotSpotGameState.ConfigGroups;
			}
			else {
				curState = HotSpotGameState.Display;
			}
			break;

		case HotSpotGameState.ConfigKeyboard : 
			ConfigKeyboard();
			curState = HotSpotGameState.Display;
			break;
		case HotSpotGameState.ConfigGroups :
			ConfigGroups();
			curState = HotSpotGameState.Display;
			break;

		case HotSpotGameState.Display : 
			print (unmasteredItems.Count);
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

			if (CheckForMastery()) {
				if (curPhase!=HotSpotPhase.Groups){
					print ("next phase");
					curPhase++;
					curState = HotSpotGameState.SetPhase;
				}
				if (curPhase==HotSpotPhase.Groups){
					//win
				}
			}

			else {
				curState = HotSpotGameState.Display;
			}
			break;
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

		totalTerms = phaseOneObjs.Count + phaseTwoObjs.Count + phaseThreeObjs.Count;
	}

	void DisplayQuestion(){
		currentCorrectAnswer = unmasteredItems[currentIndex].itemGameObject.name; //correct answer is gameobject name at index in list of items
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
			currentCorrectAnswer = unmasteredItems[currentIndex].itemGameObject.name;
			
			
			break;

		//GROUPS
		
		case HotSpotPhase.Groups :
			currentCorrectAnswer = unmasteredItems[currentIndex].itemGameObject.name;

			break;
		
		

		
		}
	}

	void ConfigGroups () {
		
	}

	void ConfigKeyboard () {
		print ("SET KEYBOARD");
		keyboardText.gameObject.SetActive (true);
	}

	public void KeyboardSubmitHandler() {
		if (keyboardText.text.ToLower () == unmasteredItems [currentIndex].itemGameObject.name.ToLower()) {
		
		}
		keyboardText.text = "";

	}

	public void SubmitAnswer (string answer) {

		if (answer == currentCorrectAnswer) {
			AnswerCorrect();
		}

		else {
			AnswerWrong();
		}

	}

	bool CheckForMastery() { //triggered when hasAnsweredCorrect is called
		while (unmasteredItems[currentIndex].sequenceMastery==1f && unmasteredItems.Count != 0) { //skip over completed 
			completedTerms++;
			unmasteredItems.Remove (unmasteredItems [currentIndex]);
			if (unmasteredItems.Count == 0) {
				print ("return true");
				return true; //phase complete
			}
			if (currentIndex >= unmasteredItems.Count){
				break;
			}
//			else if (unmasteredItems.Count > currentIndex + 1) { //if we can check the next item in the list
//				continue;
//			} else {
//				currentIndex = 0;
//			}
		}

		if (currentIndex >= unmasteredItems.Count-1) {
			currentIndex = 0; //loop around to beginning of list
		}
		else {
			currentIndex++;
		}
		return false;
		
	}

	void AdjustMastery (bool isCorrect) {
		if (isCorrect) {
			unmasteredItems [currentIndex].sequenceMastery += .5f;
		} else {
			if (unmasteredItems [currentIndex].sequenceMastery > 0) {
				unmasteredItems [currentIndex].sequenceMastery -= .5f;
			}
		}
		float totalMastery = 0f;
		foreach (ItemToBeMastered x in unmasteredItems) {
			totalMastery+=x.sequenceMastery;
		}
		totalMastery += completedTerms;

		masteryMeter.value = totalMastery/totalTerms;
		print ("mastery added");
	}

	void AnswerCorrect(){
		ClearGUIObjects ();
		BackgroundFlash.s_instance.FadeGreen ();
		AdjustMastery (true);
		print ("checkmastery from bool");
		hasAnsweredCorrect = true;



	}

	void AnswerWrong(){
		BackgroundFlash.s_instance.FadeRed ();
		AdjustMastery (false);

	}

	void ClearGUIObjects() {
		foreach (Image x in currentlyActivatedImages) {
			x.enabled = false;
		}
		currentlyActivatedImages.Clear ();
	}
}