using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Term{
	public string term;
	public string category;
	public int coneccutiveCorrectanswers = 0;
	public bool mastered = false;
	public int initIndex;
}

public class GameManager : MonoBehaviour {
	public enum GameStates {SelectionSetup, SelectACSV, Setup, Reset, Play, IncorrectAnswer};
	public GameStates myState;
	Canvas myCanvas;

	DraggableCircle draggableCircle; //the thing you drage to categorize stuff ;)
	int termIndex; //the index of the current term being categorized

	public GameObject selectionPrefab;
	public GameObject xPrefab;
	public GameObject checkPrefab;

	AudioSource m_audioSource;
	SndManager m_soundManager;
	
	public List<TextAsset> CSVs; //list of CSVs that i drag and drop in the editor ;)
	public List<string[]> termsAndCategories; // this will be what the selected list becomes after its parsed
	public List<string[]> termsToMaster; //will add all the terms to this list, and remove them one by one once they have been mastered. when this id empty the game has been won;

	List<GameObject> menuButtons; //will add all instatiated buttons in the selection scene to this list on instantiation so i can easily remove them later
	public Scrollbar scrollerForTimer; //this.value = elapsedTime/startTime (value between 0 and 1)
	Slider masterySlider;

	Timer m_timer;
	public bool incorrectAnswer = false;

	//replacing all these lists with one class list? maybe. idk this project needs so major refacotring already lol
	public List<Term> Terms;
	public List<Term> unmasteredTerms;//The copy of the list that we widdle away at as the game goes on. Always update values in Terms, using Terms[unmasteredTerms.initIndex].someProperty;

	public float mastery = 0f;

	// Use this for initialization
	void Start () {
		menuButtons = new List<GameObject>(); //TODO what does this do?
		termsAndCategories = new List<string[]>(); //TODO remove
		termsToMaster = new List<string[]>();	//TODO remove
		Terms = new List<Term>();
		unmasteredTerms = new List<Term>();
		masterySlider = GameObject.Find("Mastery").GetComponent<Slider>();
		myCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		draggableCircle = GameObject.Find("DraggableCircle").GetComponent<DraggableCircle>();
		m_audioSource = gameObject.GetComponent<AudioSource>();
		m_soundManager = GameObject.Find("SndManager").GetComponent<SndManager>();
		m_timer = GameObject.Find("Timer").GetComponent<Timer>();
	}
	
	// Update is called once per frame
	void Update () {
		scrollerForTimer.value = 1-m_timer.elapsedTime/m_timer.startTime;
		//TimerGraphic();
		updateMastery(); //TODO move this ;)
		switch(myState){
			case GameStates.SelectionSetup:
				m_timer.pause = true;
				instnatiateSelectionList();
				myState = GameStates.SelectACSV;
				break;
			case GameStates.SelectACSV: //TODO only change states in state machine
				//if button pressed, mystat = setup;
				break;
			case GameStates.Setup:
				//DestroySelectionButtons();
				termsAndCategories = parseCSV(CSVs[0]);
				DisplayCategories();
				PopulateMasteryList();
				ParsedDataToTerm(); //currently an extra step
				SetTerm();
				m_timer.pause = false;
				myState = GameStates.Play;
				break;
			case GameStates.Reset:
				print ("Reset");
				//RemoveMasteredTerms();
				m_timer.pause = false;
				m_timer.Reset();
				SetTerm();
				myState = GameStates.Play;
				break;
			case GameStates.Play:
				CorrectAnswerCheck(); // TODO have this return bool, if bool true state = reset;
				break;
			case GameStates.IncorrectAnswer:
				CorrectYourMistake();
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

	void instnatiateSelectionList(){
		float ySpacing = myCanvas.pixelRect.height/4;
		float xSpacing = myCanvas.pixelRect.width/4;
		float x = xSpacing;
		float y = myCanvas.pixelRect.height*0.75f;


		for(int i = 0; i < CSVs.Count; i++){
			GameObject temp = Instantiate(selectionPrefab);
			temp.transform.SetParent(myCanvas.transform);
			temp.transform.position = new Vector2(x, y);
			temp.transform.localScale *= myCanvas.scaleFactor;
			temp.GetComponentInChildren<Text>().text = CSVs[i].name;
			temp.GetComponent<Button>().onClick.AddListener(() => LoadCSV(temp.GetComponentInChildren<Text>().text));
			menuButtons.Add(temp);

			x+= xSpacing;
			if(i%3 == 0 && i > 0){
				y-= ySpacing;
				x = xSpacing;
			}

		}

	}

	//Called by buttons on click
	void LoadCSV(string buttonName){
		foreach(TextAsset csv in CSVs){
			if(csv.name == buttonName){
				termsAndCategories = parseCSV(csv);
			}
		}
		//ParsedDataToTerm();
		myState = GameStates.Setup;

//		//print to make sure the CSV was properly parsed
//		foreach(string[] array in termsAndCategories){
//			foreach(string myString in array){
//				print (myString);
//			}
//		}
	}

	//Convert from string array to term class. extra step for sure. 
	void ParsedDataToTerm(){
		int count = 0;
		foreach(string[] term in termsToMaster){
			print ("fak");
			Term temp = new Term(); //TODO make a constructor because its cleaner
			temp.term = term[0];
			temp.category = term[1];
			temp.initIndex = count;
			Terms.Add(temp);
			unmasteredTerms.Add(temp);
			count++;
		}
	}
	
	void DisplayCategories(){
		Text[] temp = GameObject.Find("Panels").GetComponentsInChildren<Text>(); //TODO get rid of this gameobject.Find somehow. Probably use tags. 
		for(int i = 0; i < temp.Length; i++){
			temp[i].text = termsAndCategories[0][i];
		}
	}

	//destroy the menu buttons so we can see the game screen!
	//TODO add parameter GameObject[] so that i dont get an error if there isnt one. 
	void DestroySelectionButtons(){
		foreach(GameObject obj in menuButtons){
			Destroy(obj);
		}
	}

	void PopulateMasteryList(){
		for(int i = 1; i < termsAndCategories.Count; i++){
			termsToMaster.Add(termsAndCategories[i]);
		}
	}

	void SetTerm(){
		termIndex = Random.Range(0,Terms.Count);
		draggableCircle.gameObject.GetComponentInChildren<Text>().text = unmasteredTerms[termIndex].term;// + "     " + unmasteredTerms[termIndex].coneccutiveCorrectanswers;
	}

//	void RemoveMasteredTerms(){
//		foreach(Term term in unmasteredTerms){
//			if(term.mastered){
//				unmasteredTerms.Remove(term);
//			}
//		}
//	}

	void CorrectAnswerCheck(){
		if(draggableCircle.slidding && draggableCircle.currentCategory == unmasteredTerms[termIndex].category){
			draggableCircle.currentCategory = "";
			playCorrectAnswer();
			m_timer.TrackWinLoss(true);
			fadedX(checkPrefab, draggableCircle.transform);
			Terms[unmasteredTerms[termIndex].initIndex].coneccutiveCorrectanswers++;
			termMasteryCheck(Terms[unmasteredTerms[termIndex].initIndex]);
			myState = GameStates.Reset; 
		}
		else if(draggableCircle.slidding && draggableCircle.currentCategory != ""){
			m_timer.TrackWinLoss(false);
			m_timer.pause = true;
			fadedX(xPrefab, draggableCircle.transform);
			playIncorrectAnswer();	
			Terms[unmasteredTerms[termIndex].initIndex].coneccutiveCorrectanswers = 0;
			myState = GameStates.IncorrectAnswer;
		}
	}

	void CorrectYourMistake(){
		if(draggableCircle.slidding && draggableCircle.currentCategory == unmasteredTerms[termIndex].category){
			draggableCircle.currentCategory = "";
			playCorrectAnswer();
			fadedX(checkPrefab, draggableCircle.transform);
			myState = GameStates.Reset;
		}
		else if(draggableCircle.slidding && draggableCircle.currentCategory != "" && incorrectAnswer == true){
			fadedX(xPrefab, draggableCircle.transform);
			playIncorrectAnswer();	
			incorrectAnswer = false;
		}
	}

	void playCorrectAnswer(){
		m_audioSource.clip = m_soundManager.correctAnswer;
		m_audioSource.Play();
	}

	//if a card has been matched 4+ times remove it form the unmastered list
	void termMasteryCheck(Term term){
		if(term.coneccutiveCorrectanswers >= 4){
			term.mastered = true;
			unmasteredTerms.Remove(unmasteredTerms[termIndex]); //remove the temp guy in the unmastered list
		}
	}

	void playIncorrectAnswer(){
		m_audioSource.clip = m_soundManager.incorrectAnswer;
		m_audioSource.Play();
	}


	void TimerGraphic(){
		scrollerForTimer.value = 1-m_timer.elapsedTime/m_timer.startTime;
	}

	//TODO find a better place for this, maybe at the top of reset
	void updateMastery(){
		float mastered = 0f;
		foreach(Term term in Terms){
			mastered += term.coneccutiveCorrectanswers;
		}
		mastery = mastered/(Terms.Count * 4);
		masterySlider.value = mastery;

	}

	void fadedX(GameObject prefab, Transform parent){
		GameObject temp = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
		temp.transform.parent = parent;
		temp.transform.localPosition = Vector3.zero;
		temp.transform.localScale *= myCanvas.scaleFactor;
	}

	public void LoadMainMenu() {
		Application.LoadLevel("AssignmentMenu");
		
	}

}
