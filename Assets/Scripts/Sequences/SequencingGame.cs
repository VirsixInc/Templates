using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum GameType {Text, Image};
public enum GameState {Config, SetRound, Playing, CheckAnswer, WrongAnswer, CorrectAnswer, WinScreen};

public class SequencingGame : MonoBehaviour {

	/*

	When sequencing game starts, it receives a string for text_file or image_file name and a version number that directs it to the TextAsset which will be a either text or image based depending on first part of string
	check to see if our directory has that name and version number, if not, calls ContentDownloader

	*/

	public GameObject draggableGUIPrefab, GUITargetPrefab, REDX, GREENCHECKMARK, submitButton, targetHolder;
	public GameObject draggableHolder;
	public GameObject winningConditionPopUp;
	public GameObject prompt;
	GameObject parentCanvas, draggableGUIHolder;
	List<GameObject> draggables = new List<GameObject>();
	List<GameObject> targets = new List<GameObject>();
	bool isSequenceComplete = false, isButtonPressed = false;
	List<List<string>> matrixOfCSVData;
	List<Sequence> listOfSequences, randomizedListSequences; //listOfSequences exists during an instance of Sequencing game. Current row index accesses the current sequence

	public TextAsset csvText, csvPics;
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
	Timer1 timer; //TODO refactor to a generic timer
	public Image CircleMaterial;
	public Slider mastery;
	
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
			if (numberOfDraggablesSnapped == draggables.Count){ //when all items have been placed
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
			if (numberOfDraggablesSnapped == draggables.Count)
				submitButton.GetComponent<Image> ().color = new Color (1, 1, 1, 1); //show button 
		}
	}

	void ClockShader(){
		//COUNTDOWN CLOCK SHADER
		float fractionOfTimer = (timer.startTime - timer.elapsedTime) / timer.startTime; 
		CircleMaterial.material.SetFloat ("_Angle", Mathf.Lerp (-3.14f, 3.14f, fractionOfTimer));
		CircleMaterial.material.SetColor ("_Color", Color.Lerp (start, end, fractionOfTimer));
	}

	void ConfigureAssignment() {
		submitButton = GameObject.Find ("SubmitButton"); //TODO GET RID OF ALL .FINDS
		scaleFactor = GameObject.Find ("GameCanvas").GetComponent<Canvas> ().scaleFactor;
		timer = GameObject.Find("TimerText").GetComponent<Timer1>();
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
		randomizedListSequences = new List<Sequence> (); //can remove from this list once mastered
		
		//parsing
		matrixOfCSVData = thisCSVParser.Parse (csvText);
		
		for (int i = 0; i < matrixOfCSVData.Count; i++) { //fill out list of Sequence class instances
			Sequence tempSequence = new Sequence();
			tempSequence.initIndex = i;
			tempSequence.sequenceOfStrings = matrixOfCSVData[i];
			listOfSequences.Add(tempSequence);
		}
		
		List<Sequence> tempListSequences = new List<Sequence>(listOfSequences); //copy list
		
		while (tempListSequences.Count > 0) //shuffle list
		{
			int randomIndex = Mathf.FloorToInt(Random.Range(0, tempListSequences.Count));//r.Next(0, inputList.Count); //Choose a random object in the list
			randomizedListSequences.Add(tempListSequences[randomIndex]); //add it to the new, random list
			tempListSequences.RemoveAt(randomIndex); //remove to avoid duplicates
		}
	}

	public void CheckForSequenceMastery() {
		if (currentRow >= randomizedListSequences.Count)
			currentRow = 0; //loop around to beginning of list
		while (listOfSequences[currentRow].sequenceMastery==1f && randomizedListSequences.Count != 0) { //skip over completed 
			randomizedListSequences.Remove(randomizedListSequences[currentRow]);
			if (randomizedListSequences.Count > currentRow+1) {
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
		currentSequence = new List<string> (randomizedListSequences[currentRow].sequenceOfStrings);
		float currentSequenceMastery = randomizedListSequences [currentRow].sequenceMastery;
	
		for (int i = 1; i < currentSequence.Count; i++) { //NOTE I HAD TO DO A SECOND LOOP FOR LAYERING ISSUES
			//calculate position of target based on i and sS.Count
			Rect r = Camera.main.pixelRect;
			float spaceBetweenTargets = r.width/7;
			float totalNumberOfTargets = currentSequence.Count+1;
			float xPositionOfTarget =  r.width/2+(-totalNumberOfTargets * spaceBetweenTargets)/2 + i*spaceBetweenTargets + spaceBetweenTargets/2; //makes targets centered
			GameObject tempTarget = (GameObject)Instantiate(GUITargetPrefab);
			tempTarget.transform.SetParent(targetHolder.transform, false);
//			tempTarget.transform.localScale = new Vector3(1f,1f,1f);
			tempTarget.transform.position = new Vector3(xPositionOfTarget,tempTarget.transform.position.y,0);
			tempTarget.GetComponent<TargetGUI>().correctAnswer = randomizedListSequences[currentRow].sequenceOfStrings[i];
			targets.Add (tempTarget);
			
		}
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
		int totalSpotsFilled = 0;
		for (int i = 0; i < (4 - 4*currentSequenceMastery); i++){ //currentSequenceMastery increments in .25f
			//make sure it does not fill out all answers
			if (totalSpotsFilled >= currentSequence.Count - 1){
				break;
			}

			int randValue = Random.Range(1, currentSequence.Count)-1;
			if (!targets[randValue].GetComponent<TargetGUI>().isOccupied){
				draggables[randValue].GetComponent<DraggableGUI>().AutoFillToTarget(targets[randValue].gameObject);
				totalSpotsFilled++;
			}
		}

	}

	public bool Checker (){
		int misMatches = 0;
		for (int i = 0; i < draggables.Count; i++) {
			if (draggables[i].GetComponent<DraggableGUI>().stringValue != draggables[i].GetComponent<DraggableGUI>().currentTarget.GetComponent<TargetGUI>().correctAnswer) {
				draggables[i].GetComponent<DraggableGUI>().isMismatched = true;
				misMatches ++;
			}
			else
				draggables[i].GetComponent<DraggableGUI>().isMismatched = false;
		}

		if (misMatches!=0) {
			return false;
		}
		else {
			return true;
		}
	}

	void AdjustMasteryMeter(bool didAnswerCorrect) {
		if (didAnswerCorrect && timer.timer > 1) {
			listOfSequences[randomizedListSequences[currentRow].initIndex].sequenceMastery += .25f;
		}

		else {
			if (listOfSequences[randomizedListSequences[currentRow].initIndex].sequenceMastery > 0) {
				listOfSequences[randomizedListSequences[currentRow].initIndex].sequenceMastery -= .25f;
			}
		}

		float totalMastery = 0f;
		foreach (Sequence x in listOfSequences) {
			totalMastery+=x.sequenceMastery;
		}
		totalMastery = totalMastery / listOfSequences.Count;
		mastery.value = totalMastery;
		timer.TrackWinLoss(didAnswerCorrect);
		timer.Reset();

	}

	void AnswerWrong(){
		if (SoundManager.s_instance!=null) SoundManager.s_instance.PlaySound (SoundManager.s_instance.m_wrong);

		redX.StartFade (); //TODO change to drag this into inspector
		AdjustMasteryMeter (false);
		foreach(GameObject go in draggables){
			if (go.GetComponent<DraggableGUI>().isMismatched == true) { //showing what you got right and wrong with red and green GUIs
				GameObject gr = Instantiate(REDX) as GameObject;
				gr.transform.SetParent(parentCanvas.transform);//Set in inspector
				gr.transform.position = go.transform.position;
				gr.transform.localScale = new Vector3(1f,1f,1f);
			}
			else {
				GameObject gc = Instantiate(GREENCHECKMARK) as GameObject;
				gc.transform.SetParent(parentCanvas.transform);
				gc.transform.position = go.transform.position;		
				gc.transform.localScale = new Vector3(1f,1f,1f);
			}
		}
		ResetDraggables();
		timer.TrackWinLoss(false);
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
		foreach (GameObject fo in targets) {
			Destroy (fo);
		}
		targets.Clear();
		draggables.Clear();
		currentRow++;
		CheckForSequenceMastery ();
		AdjustMasteryMeter (true);
		DisableSubmitButton ();

		if (mastery.value > .97f) {
			return true;
		} else { 
			return false;
		}
	}

	void ResetDraggables () {
		for (int i = 1; i < currentSequence.Count; i++) {
			if (draggables[i-1].GetComponent<DraggableGUI>().isMismatched){
				draggables[i-1].GetComponent<DraggableGUI>().isSnapped = false;
				draggables[i-1].transform.position = new Vector3 (Screen.width/2, Screen.height/2, 0);
			}
		}
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