using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameItem{
	int consecutiveCorrectAnswers = 0;
	bool mastered = false;
	public GameObject itemObject;
	public string objectName = "";
	public int initIndex; //might not be necesary for this game but better safe than sorry i guess
	public bool keyboardPhase = false;

	public GameItem(string name, GameObject obj, int index){
		objectName = name;
		itemObject = obj;
		initIndex = index;
	}

	public void IncrementMastery(){
		consecutiveCorrectAnswers++;
		if(consecutiveCorrectAnswers >= 6 && keyboardPhase == false){
			mastered = true;
		}
		else if(consecutiveCorrectAnswers >= 8 && keyboardPhase == true){
			mastered = true;
		}
	}
	public void ZeroMastery(){
		consecutiveCorrectAnswers = 0;
	}
	public int GetConsecutiveAnswers(){
		return consecutiveCorrectAnswers;
	}

	public bool getMastered(){
		return mastered;
	}

	public void SetMastered(bool setting){
		mastered = setting;
	}
	public int GetCorrectAnswers(){return consecutiveCorrectAnswers;}

}

public class HotspotsManager : MonoBehaviour {

	enum HotspotStates{setup, reset, play, typingSetup, typingReset, typingPlay, correctMistake, typingCorrectMistake, win};
	HotspotStates myState;
	public string selected = "";
	List<GameItem> items;
	List<GameItem> unmasteredItems;

	GameItem currentItem;
	public Text currTerm;

	int numOfMisdirections = 2;

	public Scrollbar timerScrollbar;
	public Timer timer;
	public Slider masteryMeter;
	float totalMastery;	
	float currentMastery = 0f;
	public InputField myInputField;

	// Use this for initialization
	void Start () {
		items = new List<GameItem>();
		unmasteredItems = new List<GameItem>();
		GenerateGameItemList();
		CopyItemList();
		DeactivateAllItems();
		totalMastery = items.Count * 8f;
		myInputField.gameObject.SetActive(false);
		myState = HotspotStates.setup;
	}
	
	// Update is called once per frame
	void Update () {
		timerScrollbar.value = 1f-(timer.elapsedTime/timer.startTime);
		print(DisplayDebugText());
		switch(myState){
			case HotspotStates.setup:
				SetTerm();
				ActivateMisdirectionItems(numOfMisdirections);
				timer.pause = false;
				myState = HotspotStates.play;
				break;
			case HotspotStates.play:
				if(selected != ""){
					if(CheckForSelection()){
						selected = "";
						myState = HotspotStates.reset;
					}
					else{
						timer.pause = true;
						myState = HotspotStates.correctMistake;
					}
				}
				break;
			case HotspotStates.reset:
				masteryMeter.value = CountMastery()/totalMastery;
				timer.Reset();
				timer.pause = false;
				if(unmasteredItems.Count == 0){
					myState = HotspotStates.typingSetup;
					break;
				}
				MasteryCheck();
				DeactivateAllItems();
				SetTerm();
				SetNumOfMisdirection();
				ActivateMisdirectionItems(numOfMisdirections);
				myState = HotspotStates.play;
				break;
			case HotspotStates.correctMistake:
				if(selected == currTerm.text){
					selected = "";
					myState = HotspotStates.reset;
				}
				break;
			case HotspotStates.typingSetup:
				currTerm.text = "type the correct phrase:";
				myInputField.gameObject.SetActive(true);
				InitializzeKeyboardPhase();
				SetTerm();
				myState = HotspotStates.typingPlay;
				break;
			case HotspotStates.typingPlay:
				break;
			case HotspotStates.typingReset:
				masteryMeter.value = CountMastery()/totalMastery;
				timer.Reset();
				MasteryCheck();
				SetTerm();
				myState = HotspotStates.typingPlay;
				break;
			case HotspotStates.typingCorrectMistake:
				break;
			case HotspotStates.win:
				break;
		}
	}


	//change currentItem on when a selection is made in play mode. 
	void SetTerm(){
		currentItem = items[unmasteredItems[Random.Range(0, unmasteredItems.Count)].initIndex]; //i can probably simplify this entire game by making the "unamstereItems" list just a list of indexes...hm...
		if(myState != HotspotStates.typingSetup || myState != HotspotStates.typingReset || myState != HotspotStates.typingPlay){
			currTerm.text = currentItem.objectName;
		}
		currentItem.itemObject.SetActive(true);
	}
		
	void GenerateGameItemList(){
		int count = 0;
		GameObject[] tempArray = GameObject.FindGameObjectsWithTag("GameItem");
		for(int i = 0; i < tempArray.Length; i++){
			print(tempArray.Length);
			GameItem temp = new GameItem(tempArray[i].name, tempArray[i].gameObject, i);
			print(temp.objectName);
			items.Add(temp);
			count++;
		}
		print ("There are " + items.Count + "items in the list");
	}

	//need to copy the item list to remove terms from once they are mastered
	//NEVER adjust values on this copy, instead adjust iterm[unmansteredItems[initIndex]]
	void CopyItemList(){
		foreach(GameItem item in items){
			unmasteredItems.Add(item);
		}
	}

	//deactivate every item;
	void DeactivateAllItems(){
		foreach(GameItem item in items){
			item.itemObject.SetActive(false);
		}
	}

	//activate any 2,5,8 items that arent the term. 
	void ActivateMisdirectionItems(int numToActivate){
		List<GameItem> allShuffled = SuffleItems(items);
		for(int i = 0; i < numToActivate; i++){
			if(allShuffled[i].itemObject.activeSelf){
				numToActivate++;
			}
			allShuffled[i].itemObject.SetActive(true);
		}
	}

	//shuffle gameItems, should be able to apply this to list of any type, but i wasnt sure how to initialize a variable of a variable type. 
	//Use on items to make a temporrary randomized version of it
	public List<GameItem> SuffleItems(List<GameItem> list){
		List<GameItem> tempList = new List<GameItem>(list);
		for(int i =0;i<tempList.Count;i++){
			GameItem temp = tempList[i];
			int randomIndex = Random.Range(i,tempList.Count);
			tempList[i] = tempList[randomIndex];
			tempList[randomIndex] = temp;
		}
		return tempList;
	}

	bool CheckForSelection(){
		if(selected != ""){
			if(selected == currentItem.objectName){
				currentItem.IncrementMastery();
				timer.TrackWinLoss(true);
				SoundManager.s_instance.PlaySound(SoundManager.s_instance.m_correct);
				return true;
				//record win in timer
			}
			else{
				currentItem.ZeroMastery();
				timer.TrackWinLoss(false);
				SoundManager.s_instance.PlaySound(SoundManager.s_instance.m_wrong);
				return false;
				//record loss in timer
			}
			selected = ""; //setting this back to null here so this function doesnt get called twice that would be wack. 
			//myState = HotspotStates.reset;
		}
		else return false;
	}

	void SetNumOfMisdirection(){
		if(currentItem.GetConsecutiveAnswers() >= 2 && currentItem.GetConsecutiveAnswers() < 4){
			numOfMisdirections = 5;
		}
		else if(currentItem.GetConsecutiveAnswers() >= 4){
			numOfMisdirections = 8;
		}
		else{
			numOfMisdirections = 2;
		}
	}

	//go through the unmastered list, check the main list for mastery, remove from unmastered if that item is "mastered"
	//removing while iterating through this for each loop is bad do it over ;)
	//since i call this every reset, only one item could posibly be mastered at a time. kill them as they come. 
	void MasteryCheck(){
		for(int i = 0; i < unmasteredItems.Count; i++){
			if(items[unmasteredItems[i].initIndex].getMastered()){
				unmasteredItems.Remove(unmasteredItems[i]);
				break;
			}
		}
	}
	float CountMastery(){
		float tempMastery = 0;
		foreach(GameItem item in items){
			tempMastery += item.GetConsecutiveAnswers();
		}
		return tempMastery;
	}

	string DisplayDebugText(){
		string debugText = "";
		debugText += "" + myState + "\n";
		foreach(GameItem item in items){
			string temp = "";
			temp += item.objectName + ": " + item.GetConsecutiveAnswers() + " - " + item.getMastered() + " - Index: " + item.initIndex + "\n";
			debugText += temp;
		}
		debugText += "Unmastered: " + unmasteredItems.Count;
		return debugText;
	}

	//set each item.keyboardphase to true, each items.mastered to false, and add back to unmastered list. 
	void InitializzeKeyboardPhase(){
		for(int i = 0; i < items.Count; i++){
			items[i].keyboardPhase = true;
			items[i].SetMastered(false);
			unmasteredItems.Add (items[i]);
		}
	}

	public void LoadMainMenu() {
		Application.LoadLevel("AssignmentMenu");
		
	}

	//called by input field on submit
	public void CheckTypingAnswer(){
		if(myState != HotspotStates.typingCorrectMistake){
			if(myInputField.text == currentItem.objectName){
				//true
				timer.TrackWinLoss(true);
				SoundManager.s_instance.PlaySound(SoundManager.s_instance.m_correct);
				currentItem.IncrementMastery();
				myState = HotspotStates.typingReset;
			}
			else{
				//enter correct your answer phase
				myInputField.text = "";
				currTerm.text = "Incorrect answer. The correct answer is " + currentItem.objectName + ". Please type the correct answer to procede!";
				timer.TrackWinLoss(false);
				SoundManager.s_instance.PlaySound(SoundManager.s_instance.m_wrong);
				timer.pause = true;
				myState = HotspotStates.typingCorrectMistake;
			}
		}
		else{
			if(myInputField.text == currentItem.objectName){
				currTerm.text = "type the correct phrase:";
				timer.pause = false;
				SoundManager.s_instance.PlaySound(SoundManager.s_instance.m_snap);
				myState = HotspotStates.typingReset;
			}
			else{
				//play bad soumd -> erase answer
				myInputField.text = "";
			}
		}

	}

}
