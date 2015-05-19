using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public enum HotSpotPhase
{
	Elements,
	Typing,
	Groups}
;

public enum HotSpotGameState
{
	Config,
	SetPhase,
	ConfigKeyboard,
	Display,
	Playing,
	CheckMastery,
	NextQuestion,
	Win}
;

public class HotSpotsGame : MonoBehaviour
{
	public PopUpGraphic redX, greenCheck;
	public Slider masteryMeter;
	public Text promptText, promptText2, correctSpellingText;
	public static HotSpotsGame s_instance;
	public GameObject winningSlide;
	HotSpotPhase curPhase = HotSpotPhase.Elements;
	HotSpotGameState curState = HotSpotGameState.Config;
	GameObject[] individualElements, groups, elementsShorthand;
	//unmasterItems is the copy of each of the phaseObjs depending on curPhase
	public List<ItemToBeMastered> phaseOneObjs, phaseTwoObjs, phaseThreeObjs, unmasteredItems;
	int currentIndex;
	string currentCorrectAnswer;
	List<Image> currentlyActivatedImages;
	List<GameObject> currentlyActivatedGameObjects;
	bool hasAnsweredCorrect = false, masteryChecked = false;
	float totalTerms, completedTerms;
	//Keyboard members
	private bool handleCardPress, firstPress, handleKeyboardSubmit, firstSubmit, isLoadingMainMenu;
	public InputField keyboardText;

	//timer
	public Image CircleMaterial;
	public Slider mastery;
	bool isPrexistingData;
	bool isExiting = false;

	//UI Meters etc...
	[SerializeField]
	Color start;
	[SerializeField]
	Color end;

	void Awake ()
	{
		s_instance = this;
	}	

	//state machine
	void Update ()
	{
//		print (curPhase);
//		print (curState);
		switch (curState) {
		case HotSpotGameState.Config: 
			ConfigGameData ();
			curState = HotSpotGameState.SetPhase;
			break;
		case HotSpotGameState.SetPhase:
			SetPhase ();
			if (curPhase == HotSpotPhase.Typing) {
				curState = HotSpotGameState.ConfigKeyboard;
			} else {
				curState = HotSpotGameState.Display;
			}
			break;

		case HotSpotGameState.ConfigKeyboard: 
			ConfigKeyboard ();
			curState = HotSpotGameState.Display;
			break;


		case HotSpotGameState.Display: 
			print (unmasteredItems.Count);
			DisplayQuestion ();
			curState = HotSpotGameState.Playing;
			break;
		case HotSpotGameState.Playing:
			if (hasAnsweredCorrect) {
				hasAnsweredCorrect = false;
				curState = HotSpotGameState.CheckMastery;
			}

			break;
		case HotSpotGameState.CheckMastery:
			//if CheckForM returns true, 

			if (CheckForMastery ()) {
				if (curPhase != HotSpotPhase.Groups) {
					print ("next phase");
					if (curPhase == HotSpotPhase.Typing) {
						keyboardText.enabled = false;
					}
					curPhase++;
					curState = HotSpotGameState.SetPhase;
				}
				else if (curPhase == HotSpotPhase.Groups) {
					winningSlide.SetActive(true);
					curState = HotSpotGameState.Win;
				}
			} else {
				curState = HotSpotGameState.Display;
			}
			break;
		case HotSpotGameState.Win:
			int masteryOutput = Mathf.CeilToInt(mastery.value*100);
//			AppManager.s_instance.saveAssignmentMastery(AppManager.s_instance.currentAssignments [AppManager.s_instance.currIndex], masteryOutput);
			if (isExiting == false){
				StartCoroutine("LoadMain");
				if(SoundManager.s_instance!=null)SoundManager.s_instance.PlaySound(SoundManager.s_instance.m_win);
				isExiting = true;

			}
			break;

		}
	}

	IEnumerator LoadMain() {
		yield return new WaitForSeconds (5f);
		Application.LoadLevel ("AssignmentMenu");
	}

	void SetPhase ()
	{
		switch (curPhase) {
		case HotSpotPhase.Elements:
			unmasteredItems.Clear ();
			unmasteredItems = new List<ItemToBeMastered> (phaseOneObjs);
			break;
		case HotSpotPhase.Typing:
			unmasteredItems.Clear ();
			unmasteredItems = new List<ItemToBeMastered> (phaseTwoObjs);
			break;

		case HotSpotPhase.Groups:
			unmasteredItems.Clear ();
			unmasteredItems = new List<ItemToBeMastered> (phaseThreeObjs);
			print (phaseThreeObjs.Count);
			break;

		}
		//clear curList
		//copy other list

	}

	void ConfigGameData ()
	{
		phaseOneObjs = new List<ItemToBeMastered> ();
		phaseTwoObjs = new List<ItemToBeMastered> ();
		phaseThreeObjs = new List<ItemToBeMastered> ();
		unmasteredItems = new List<ItemToBeMastered> ();
		currentlyActivatedGameObjects = new List<GameObject> ();
		currentlyActivatedImages = new List<Image> (); 
		individualElements = GameObject.FindGameObjectsWithTag ("elements");
		elementsShorthand = GameObject.FindGameObjectsWithTag ("elementShort");
		groups = GameObject.FindGameObjectsWithTag ("groups");
		print (groups.Length);
		foreach (GameObject go in individualElements) {
			ItemToBeMastered item = new ItemToBeMastered (0f, go);
			phaseOneObjs.Add (item);
		}
		foreach (GameObject go in elementsShorthand) {
			ItemToBeMastered item = new ItemToBeMastered (0f, go);
			phaseTwoObjs.Add (item);
		}

		foreach (GameObject go in groups) {
			ItemToBeMastered item = new ItemToBeMastered (0f, go);
			phaseThreeObjs.Add (item);
		}
		//sort all the lists alphabetically
		List<ItemToBeMastered> tempList = new List<ItemToBeMastered> ();
		tempList = phaseOneObjs.OrderBy (item => item.itemGameObject.name).ToList ();
		phaseOneObjs = new List<ItemToBeMastered> (tempList);
		
		List<ItemToBeMastered> tempList2 = new List<ItemToBeMastered> ();
		tempList2 = phaseTwoObjs.OrderBy (item => item.itemGameObject.name).ToList ();
		phaseTwoObjs = new List<ItemToBeMastered> (tempList2);
		
		List<ItemToBeMastered> tempList3 = new List<ItemToBeMastered> ();
		tempList3 = phaseThreeObjs.OrderBy (item => item.itemGameObject.name).ToList ();
		phaseThreeObjs = new List<ItemToBeMastered> (tempList3);

		totalTerms = phaseOneObjs.Count + phaseTwoObjs.Count + phaseThreeObjs.Count;

//		if (AppManager.s_instance != null) {
//			float previousMasteryData = AppManager.s_instance.pullAssignMastery (AppManager.s_instance.currentAssignments [AppManager.s_instance.currIndex]) / 100;
////			thresholds of data will be objs.count over totalTerms 
//			if (previousMasteryData < (phaseOneObjs.Count / totalTerms)) {
//				mastery.value = 0;
//			} else if (previousMasteryData > (phaseOneObjs.Count / totalTerms) && (previousMasteryData < (phaseOneObjs.Count + phaseTwoObjs.Count) / totalTerms)) {
//				mastery.value = (float)phaseOneObjs.Count / totalTerms;
//				curPhase = HotSpotPhase.Typing;
//			} else if (previousMasteryData > (phaseOneObjs.Count + phaseTwoObjs.Count) / totalTerms) {
//				mastery.value = (float)(phaseOneObjs.Count + phaseTwoObjs.Count) / totalTerms;
//				curPhase = HotSpotPhase.Groups;
//			}
//		}
	}

	void DisplayQuestion ()
	{
		Timer1.s_instance.Reset(15f);
		currentCorrectAnswer = unmasteredItems [currentIndex].itemGameObject.name; //correct answer is gameobject name at index in list of items
		promptText.text = currentCorrectAnswer;
		List<int> randIndexList = new List<int> (); //to avoid duplicates

		switch (curPhase) {
		case HotSpotPhase.Elements:
			currentlyActivatedImages.Add (unmasteredItems [currentIndex].itemGameObject.GetComponent<Image> ()); // add correct answer image to list

			for (int i = 0; i < phaseOneObjs.Count; i++) {
				randIndexList.Add (i); //generate a list of numbers
			}
			randIndexList.Remove (currentIndex);//remove that int so it cant be chosen again

			if (unmasteredItems [currentIndex].sequenceMastery < 0.5f) {
				for (int i = 0; i < 2; i++) { //choose 2 additional items to be displayed as wrong answers
					int randomInt = Random.Range (0, randIndexList.Count);
					currentlyActivatedImages.Add (phaseOneObjs [randIndexList [randomInt]].itemGameObject.GetComponent<Image> ()); //add in random wrong answer
					randIndexList.Remove (randIndexList [randomInt]); //make sure it doesnt get added twice
				}
			} else {
				for (int i = 0; i < 4; i++) { //choose 4 additional items to be dispayed as wrong answer
					int randomInt = Random.Range (0, randIndexList.Count);
					currentlyActivatedImages.Add (phaseOneObjs [randIndexList [randomInt]].itemGameObject.GetComponent<Image> ()); //add in random wrong answer
					randIndexList.Remove (randIndexList [randomInt]); //make sure it doesnt get added twice
				}
			}

			//display elements that can be clicked on
			foreach (Image image in currentlyActivatedImages) {
				image.enabled = true;
			}

			break;
		
		//TYPING
			
		case HotSpotPhase.Typing:
			promptText2.text = "Type the Element Name";
			currentCorrectAnswer = unmasteredItems [currentIndex].itemGameObject.transform.GetChild (0).name;
			break;

		//GROUPS
		
		case HotSpotPhase.Groups:
			promptText2.text = "Select the Atomic Group";
			keyboardText.enabled = false;
			for (int i = 0; i < phaseThreeObjs.Count; i++) {
				randIndexList.Add (i);
			}
			randIndexList.Remove (currentIndex);
			currentlyActivatedGameObjects.Add (unmasteredItems [currentIndex].itemGameObject); // add correct answer image to list

			if (unmasteredItems [currentIndex].sequenceMastery < 0.5f) {
				for (int i = 0; i < 2; i++) { //choose 2 additional items to be displayed as wrong answers
					int randomInt = Random.Range (0, randIndexList.Count);
					currentlyActivatedGameObjects.Add (phaseThreeObjs [randIndexList [randomInt]].itemGameObject); //add in random wrong answer
					randIndexList.Remove (randIndexList [randomInt]); //make sure it doesnt get added twice
				}
			} else {
				for (int i = 0; i < 4; i++) { //choose 4 additional items to be dispayed as wrong answer
					int randomInt = Random.Range (0, randIndexList.Count);
					currentlyActivatedGameObjects.Add (phaseThreeObjs [randIndexList [randomInt]].itemGameObject); //add in random wrong answer
					randIndexList.Remove (randIndexList [randomInt]); //make sure it doesnt get added twice
				}
			}
			
			//display elements that can be clicked on
			foreach (GameObject go in currentlyActivatedGameObjects) {
				//check for children due to way images are stored in inspector
				if (go.transform.childCount!=0) {
					foreach (Image im in go.transform.GetComponentsInChildren<Image>()){
						im.enabled = true;
					}
				}else{
					go.GetComponent<Image>().enabled = true;
				}
			}

			break;
		
		

		
		}
	}

	void ConfigKeyboard ()
	{
		print ("SET KEYBOARD");
		keyboardText.gameObject.SetActive (true);
	}

	public void KeyboardSubmitHandler ()
	{
		SubmitAnswer (keyboardText.text.ToLower ());
		keyboardText.text = "";
	}

	public void SubmitAnswer (string answer)
	{
		if (answer.ToLower () == currentCorrectAnswer.ToLower ()) {
			AnswerCorrect ();
		} else {
			AnswerWrong ();
		}

	}

	bool CheckForMastery ()
	{ //triggered when hasAnsweredCorrect is called
		while (unmasteredItems[currentIndex].sequenceMastery==1f && unmasteredItems.Count != 0) { //skip over completed 
			completedTerms++;
			unmasteredItems.Remove (unmasteredItems [currentIndex]);
			if (unmasteredItems.Count == 0) {
				print ("return true");
				return true; //phase complete
			}
			if (currentIndex >= unmasteredItems.Count) {
				break;
			}
		}
		IterateToNextItem ();
		return false;	
	}

	void IterateToNextItem ()
	{
		if (currentIndex >= unmasteredItems.Count - 1) {
			currentIndex = 0; //loop around to beginning of list
		} else {
			currentIndex++;
		}
	}

	void AdjustMastery (bool isCorrect)
	{
		if (isCorrect && !	Timer1.s_instance.timesUp) {
			if (curPhase == HotSpotPhase.Typing) {
				unmasteredItems [currentIndex].sequenceMastery += 1.0f;
			}
			else {
				unmasteredItems [currentIndex].sequenceMastery += .5f;
			}
		} else {
			if (unmasteredItems [currentIndex].sequenceMastery > 0) {
				unmasteredItems [currentIndex].sequenceMastery -= .5f;
			}
		}
		float totalMastery = 0f;
		foreach (ItemToBeMastered x in unmasteredItems) {
			totalMastery += x.sequenceMastery;
		}
		totalMastery += completedTerms;

		masteryMeter.value = totalMastery / totalTerms;
	}

	void AnswerCorrect ()
	{
		if(SoundManager.s_instance!=null)SoundManager.s_instance.PlaySound(SoundManager.s_instance.m_correct);
		greenCheck.StartFade (); 
		ClearGUIObjects ();
		BackgroundFlash.s_instance.FadeGreen ();
		AdjustMastery (true);
		hasAnsweredCorrect = true;
	}

	void AnswerWrong ()
	{
		if(SoundManager.s_instance!=null)SoundManager.s_instance.PlaySound(SoundManager.s_instance.m_wrong);

		if (curPhase == HotSpotPhase.Typing) {
			correctSpellingText.text = currentCorrectAnswer;
			correctSpellingText.gameObject.GetComponent<Fader>().StartFadeOut();
		}
		redX.StartFade (); 
		ClearGUIObjects ();
		BackgroundFlash.s_instance.FadeRed ();
		AdjustMastery (false);
		IterateToNextItem ();
		ClearGUIObjects ();
		curState = HotSpotGameState.Display;
	}

	void ClearGUIObjects ()
	{
		if (currentlyActivatedImages.Count != 0) {
			foreach (Image x in currentlyActivatedImages) {
				x.enabled = false;
			}
			currentlyActivatedImages.Clear ();
		} else if (currentlyActivatedGameObjects.Count != 0) {
			foreach (GameObject go in currentlyActivatedGameObjects) {
				if (go.transform.childCount!=0) {
					foreach (Image im in go.transform.GetComponentsInChildren<Image>()){
						im.enabled = false;
					}
				}else{
					go.GetComponent<Image>().enabled = false;
				}
			}
			currentlyActivatedGameObjects.Clear ();

		}
	}
}