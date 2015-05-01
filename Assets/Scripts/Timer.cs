using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Timer : MonoBehaviour {

	public float startTime;
	public float timer = 0f;
	public float elapsedTime;
	public bool timerIsElapsed = false;
	public Text timerText;

	public float timerMin = 7f;
	public float timerMax;

	public float answerTooSlow;
	public float timeToAdd;

	bool fading = false;
	public bool throwingAway = false;

	List<bool> winsAndLosses;
	int totalWins;
	int totalLosses;

	public bool pause = false;
	
//	GameObject incorrectBackdrop, correctBackdrop, movingBackdrop;
//	float backdropDistance;
//	Vector3 backdropStartPos;


	// Use this for initialization
	void Start () {
		timer = startTime;
		elapsedTime = startTime - timer;
		timerText = gameObject.GetComponent<Text>();
		winsAndLosses = new List<bool>();
//		incorrectBackdrop = GameObject.Find("LoseScreen");
//		correctBackdrop = GameObject.Find("WinScreen");
//		movingBackdrop = GameObject.Find("OpacitySlider");
//
//		backdropStartPos = movingBackdrop.transform.position;
//		backdropDistance = Vector3.Distance(movingBackdrop.transform.position, Vector3.zero);


	}
	
	// Update is called once per frame
	void Update () {
		if(pause){

		}
		else{
			if(timer > 0f){
				timer -= Time.deltaTime;
				elapsedTime = startTime - timer;
			}
			else{
				timer = 0f;
			}
			
			float temp = Mathf.Ceil(timer);
			timerText.text = temp.ToString();
			//		float fracJourney = elapsedTime/startTime;
			//		movingBackdrop.transform.position = Vector3.Lerp(backdropStartPos, new Vector3(backdropStartPos.x, (backdropStartPos.y/10f), backdropStartPos.z), fracJourney);
			
			//check if we went over time, then set a bool for use by rest of game
			if(timer <= 0 && !timerIsElapsed){
				timerIsElapsed = true;
			}
		}

	}

	//likely to be called by a game manager
	public void Reset(){
//		fading = true;
//
//		if(returnMostRecentWinLoss()){
//			StartCoroutine(fadeOut(correctBackdrop, 255f, 0f, 2f));
//		}
//		else if (!returnMostRecentWinLoss()) {
//			StartCoroutine(fadeOut(incorrectBackdrop, 255f, 0f, 2f));
//		}
//
//		while(fading || throwingAway){
//			yield return new WaitForEndOfFrame();
//		}

		timerIsElapsed = false;
		AdjustStartTime();
		elapsedTime = 0f;
		timer = startTime;
//		movingBackdrop.transform.position = backdropStartPos;
//		GameObject.Find("Terms").GetComponent<CanvasGroup>().interactable = true;
//		GameObject.Find("Defs").GetComponent<CanvasGroup>().interactable = true;
//		timer = startTime;
//		GameObject.Find("GameManager").GetComponent<MatchingManager>().myState = MatchingManager.matchingGameStates.reset;
	}

	//decrease or increase start time based on time it took to complete a task
	//TODO: keep track of correct/incorrect answers. add time for wrong answers, subtract time for right answers. 
	//Timing may take precedent of answers. Keep track of words on screen. Words on screen time is added first. 
	//Algorithm: timer +- wordsOnScreenTime(scalar) => if(rightAnswer) timer +- elapsedTimeValue, else timer += WrongAnswerValue

	void AdjustStartTime(){
		bool lastResult = returnMostRecentWinLoss();
		print ("my last result: " + lastResult);

		if(lastResult){
			startTime--;
		}
		else if(!lastResult){
			startTime++;
		}


		if(startTime <= timerMax){
			if(elapsedTime > (startTime/answerTooSlow)){
				print(elapsedTime + ", " + (elapsedTime * 0.2f));
				timeToAdd = (startTime * 0.15f);
				if(lastResult){
					timeToAdd--;
				}
				else if(!lastResult){
					timeToAdd+=3;
				}
				startTime += timeToAdd;
			}
		}
		if(startTime >= 1+timerMin && lastResult){
			if(elapsedTime < 3.5f){
				startTime-=1f;
			}
		}
		else if(startTime >= 1+timerMin && !lastResult){
			startTime += 3;
		}
		if(startTime <= timerMin){
			startTime = timerMin;
		}
	}

	//called by game manager, a win/lose bool should be sent for tracking 
	public void TrackWinLoss(bool winLose){
		//Make List of bools. track bools. 
		winsAndLosses.Add(winLose);
//		string temp = "";
//		foreach(bool winloss in winsAndLosses){
//			temp += (winloss + ", ");
//		}
//		print (temp);
	}

	//returns sum of wins, and sum of losses;
	void calculateTotalWinloss(){
		totalWins = 0;
		totalLosses = 0;
		foreach(bool winloss in winsAndLosses){
			if(winloss){
				totalWins++;
			}
			if(!winloss){
				totalLosses++;
			}
		}

		print ("Wins: " + totalWins + ", Losses: " + totalLosses);
	} 

	public bool returnMostRecentWinLoss(){
		if(winsAndLosses.Count > 1){
			return winsAndLosses[winsAndLosses.Count-1];
		}
		else return false;


	}

	void AddTimeForWords(float wordsInGame){

		timeToAdd += wordsInGame/3;

	}

}
