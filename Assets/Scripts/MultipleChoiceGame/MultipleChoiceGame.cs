using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MultipleChoiceGame : MonoBehaviour {

	public GameObject draggableGUIPrefab, GUITargetPrefab, REDX, GREENCHECKMARK, submitButton;
	public GameObject draggableHolder;
	public GameObject winningConditionPopUp;
	public GameObject prompt;
	public TextAsset csvText;
	public Image CircleMaterial;
	public Slider mastery;
	public GameObject target;
	public List<Image> pictures;
	public Image picture;
	
	GameObject parentCanvas, draggableGUIHolder;
	List<GameObject> draggables = new List<GameObject>();
	bool isSequenceComplete = false, isButtonPressed = false;
	List<List<string>> matrixOfCSVData;
	List<Sequence> listOfSequences; //listOfSequences exists during an instance of Sequencing game. Current row index accesses the current sequence
	
	GameType gameType = GameType.Text;
	GameState gameState = GameState.Config;
	bool areDistractorTerms;
	int currentRow = 0; //currentRow is the iterator that goes through the remaining sequences
	int xRandomRange, yRandomRange;
	List<string> currentSequence;
	List<float> masteryValues; //all start at 0 on first playthrough.
	float scaleFactor, numberOfDraggablesSnapped=0;
	float startTime, exitTime = 5f;
	CSVParser thisCSVParser;
	PopUpGraphic greenCheck, redX, greenCheckmark;//todo

	//UI Meters etc...
	[SerializeField]
	Color start;
	[SerializeField]
	Color end;
	
	
	void Update () 
	{
		switch (gameState) {
		case GameState.Config :
			ConfigureAssignment();
			//check JSON to see if it is ReqIMG or not, if is set GameType to GameType.Image
			gameState = GameState.SetRound;
			break;
			
		case GameState.SetRound :
			CheckForSequenceMastery(); //eliminate mastered sequences
			InitiateSequence();
			gameState = GameState.Playing;
			break;
			
		case GameState.Playing :
			CheckSequence(); //checks to see how many items have been placed
			if (numberOfDraggablesSnapped == 1){ //when all items have been placed
				gameState = GameState.CheckAnswer;
			}
			break;
			
		case GameState.CheckAnswer :
			if (isButtonPressed) {
				if (Checker()){
					gameState = GameState.CorrectAnswer;
				}
				else {
					gameState = GameState.WrongAnswer;
				}
			}
			break;
			
		case GameState.CorrectAnswer :
			if (AnswerCorrect()){
				gameState = GameState.WinScreen;
			}
			else {
				gameState = GameState.SetRound;
			}
			break;
			
		case GameState.WrongAnswer :
			AnswerWrong();
			gameState = GameState.Playing;
			break;
			
		case GameState.WinScreen :
			if ((Time.time - startTime) > exitTime) {
				LoadMainMenu();
			}
			break;
		}
		ClockShader(); //shows rotating timer
		
	}
	
	void CheckSequence(){
		//checks to see how many items are currently snapped into place, keeps track of the number.
		if (draggables != null) {
			numberOfDraggablesSnapped = 0;
			foreach (GameObject x in draggables) {
				if (x.GetComponent<DraggableGUI> ().isSnapped) {
					numberOfDraggablesSnapped++; //how many items are currently snapped +1
				}
				
			}
			if (numberOfDraggablesSnapped == 1)
				submitButton.GetComponent<Image> ().color = new Color (1, 1, 1, 1); //show button 
		}
	}
	
	void ClockShader(){
		//COUNTDOWN CLOCK SHADER
//		float fractionOfTimer = (timer.startTime - timer.elapsedTime) / timer.startTime; 
//		CircleMaterial.material.SetFloat ("_Angle", Mathf.Lerp (-3.14f, 3.14f, fractionOfTimer));
//		CircleMaterial.material.SetColor ("_Color", Color.Lerp (start, end, fractionOfTimer));
	}
	
	void ConfigureAssignment() {
		submitButton = GameObject.Find ("SubmitButton"); //TODO GET RID OF ALL .FINDS
		scaleFactor = GameObject.Find ("GameCanvas").GetComponent<Canvas> ().scaleFactor;
//		timer = GameObject.Find("TimerText").GetComponent<Timer1>();
		greenCheck = GameObject.Find ("greenCheck").GetComponent<PopUpGraphic> ();
		parentCanvas = GameObject.Find ("GameCanvas");
		draggableGUIHolder = GameObject.Find ("DraggableGUIHolder");
		redX = GameObject.Find ("redX").GetComponent<PopUpGraphic> ();
		Input.multiTouchEnabled = true;
		
		//parse CSV
		thisCSVParser = GetComponent<CSVParser> ();
		matrixOfCSVData = new List<List<string>> ();
		
		//list init
		listOfSequences = new List<Sequence> (); //use this to store per sequence mastery values

		//parsing
		matrixOfCSVData = thisCSVParser.Parse (csvText);
		
		for (int i = 0; i < matrixOfCSVData.Count; i++) { //fill out list of Sequence class instances
			Sequence tempSequence = new Sequence();
			tempSequence.initIndex = i;
			tempSequence.sequenceOfStrings = matrixOfCSVData[i];
			listOfSequences.Add(tempSequence);
		}

	}
	
	public void CheckForSequenceMastery() {
		if (currentRow >= listOfSequences.Count)
			currentRow = 0; //loop around to beginning of list
		while (listOfSequences[currentRow].sequenceMastery==1f && listOfSequences.Count != 0) { //skip over completed 
			listOfSequences.Remove(listOfSequences[currentRow]);
			if (listOfSequences.Count > currentRow+1) {
				currentRow++;
			}
			else 
				currentRow = 0;
		}
	}
	
	public void LoadMainMenu() {
		Application.LoadLevel("AssignmentMenu");
		
	}
	void WinRound() {
		GameObject winCard = Instantiate (winningConditionPopUp) as GameObject;
		winCard.transform.SetParent(parentCanvas.transform);
		winCard.transform.localScale *= scaleFactor;
		winCard.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
		gameState = GameState.WinScreen; //i know that this is the wrong way to change gamestate but I have to do it until a major refactor
		startTime = Time.time;
	}
	
	public void InitiateSequence () { //displaces current sequence
		currentSequence = listOfSequences [currentRow].sequenceOfStrings;
		picture.sprite = pictures [currentRow].sprite;
		//instantiate all of the targets and draggables in the correct positions
		for (int i = 1; i < currentSequence.Count; i++) {
			//calculate position of target based on i and sS.Count
			//generate currentSequence.Count number dragable GUI objects
			GameObject tempDraggable = (GameObject)Instantiate(draggableGUIPrefab);
			tempDraggable.transform.SetParent(draggableGUIHolder.transform);
			tempDraggable.transform.localScale *= scaleFactor;
			tempDraggable.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
			tempDraggable.GetComponent<DraggableGUI>().SetValues(currentSequence[i], gameType);
			draggables.Add (tempDraggable);
		}
		//use mastery to determine how many answers will be filled in
		//GameObject targetHolder = GameObject.Find ("TargetGUIHolder");
		string tempPrompt = currentSequence [0];
		string replaceComma = tempPrompt.Replace ('/', ',');
		prompt.GetComponent<Text> ().text = replaceComma;
		target.GetComponent<TargetGUI> ().correctAnswer = currentSequence [1];
	}
	
	public bool Checker (){
		print ("checK");
		if (target.GetComponent<TargetGUI>().correctAnswer == target.GetComponent<TargetGUI>().occupier.GetComponent<DraggableGUI>().stringValue) {
			print ("true");

			return true;
		}
		else {
			print ("false");

			return false;
		}
	}
	
//	void AdjustMasteryMeter(bool didAnswerCorrect) {
////		if (didAnswerCorrect && timer.timer > 1) {
//			listOfSequences[listOfSequences[currentRow].initIndex].sequenceMastery += .25f;
//		}
//		
//		else {
//			if (listOfSequences[listOfSequences[currentRow].initIndex].sequenceMastery > 0) {
//				listOfSequences[listOfSequences[currentRow].initIndex].sequenceMastery -= .25f;
//			}
//		}
//		
//		float totalMastery = 0f;
//		foreach (Sequence x in listOfSequences) {
//			totalMastery+=x.sequenceMastery;
//		}
//		totalMastery = totalMastery / listOfSequences.Count;
//		mastery.value = totalMastery;
////		timer.TrackWinLoss(didAnswerCorrect);
////		timer.Reset();
//		
//	}
	
	void AnswerWrong(){
		if (SoundManager.s_instance!=null) SoundManager.s_instance.PlaySound (SoundManager.s_instance.m_wrong);
		
		redX.StartFade (); //TODO change to drag this into inspector
//		AdjustMasteryMeter (false);
//		foreach(GameObject go in draggables){
//			if (go.GetComponent<DraggableGUI>().isMismatched == true) { //showing what you got right and wrong with red and green GUIs
//				GameObject gr = Instantiate(REDX) as GameObject;
//				gr.transform.SetParent(parentCanvas.transform);//Set in inspector
//				gr.transform.position = go.transform.position;
//				gr.transform.localScale = new Vector3(1f,1f,1f);
//			}
//			else {
//				GameObject gc = Instantiate(GREENCHECKMARK) as GameObject;
//				gc.transform.SetParent(parentCanvas.transform);
//				gc.transform.position = go.transform.position;		
//				gc.transform.localScale = new Vector3(1f,1f,1f);
//			}
//		}
		ResetDraggables();
//		timer.TrackWinLoss(false);
		DisableSubmitButton ();
		
	}
	
	bool AnswerCorrect() {
		if (SoundManager.s_instance!=null) SoundManager.s_instance.PlaySound (SoundManager.s_instance.m_correct);
		
		greenCheck.StartFade (); //TODO set in inspector
		foreach(GameObject go in draggables) {
			//			GameObject gc = Instantiate(GREENCHECKMARK) as GameObject;
			//			gc.transform.SetParent(GameObject.Find ("GameCanvas").transform);
			//			gc.transform.position = go.transform.position;
			Destroy (go);
		}
		target.GetComponent<TargetGUI> ().Reset ();
		draggables.Clear();
		currentRow++;
		CheckForSequenceMastery ();
//		AdjustMasteryMeter (true);
		DisableSubmitButton ();
		
		if (mastery.value > .97f) {
			return true;
		} else { 
			return false;
		}
	}
	
	void ResetDraggables () {
		target.GetComponent<TargetGUI> ().occupier.GetComponent<DraggableGUI> ().isSnapped = false;
		target.GetComponent<TargetGUI>().occupier.transform.position = new Vector3 (Screen.width/2, Screen.height/4, 0);

	}
	void DisableSubmitButton(){
		submitButton.GetComponent<Image> ().color = new Color (1, 1, 1, .3f); //allow
		isButtonPressed = false;
	}
	public void PressSubmitButton () {
		if (submitButton.GetComponent<Image> ().color.a == 1f) {
			isButtonPressed = true;
		}
	}
}
