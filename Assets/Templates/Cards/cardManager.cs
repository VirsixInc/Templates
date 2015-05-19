using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Card{

  public GameObject objAssoc;
  public Text objText;
  public Image objImg;
  public string answer;
  public string question;
  public indiCard thisIndiCard;
  public Card(GameObject objRef, Text objTxtRef, Image objImageRef){
    objAssoc = objRef;
    objImg = objImageRef;
    objText = objTxtRef;
  }
  public void setCard(Term termToUse, bool useImage){
    answer = termToUse.answer;
    question = termToUse.question;
    objImg.sprite = termToUse.imgAssoc; 
    objText.text = answer;
    objAssoc.SetActive(true);
  }
};

public class Term{
  public string answer;
  public string question;
  public Sprite imgAssoc;
  public int mastery = 0;
  public bool mastered = false;
  public Term(string newQuestion, string newAnswer, string filePathForImg = null){
    if(filePathForImg != null){
      imgAssoc = Resources.Load<Sprite>(filePathForImg);
    }
    question = newQuestion;
    answer = newAnswer;
  }
}

public class cardManager : MonoBehaviour {

  public enum GameState{
    ConfigCards,
    PlayingCards,
    ResetCards,
    ConfigKeyboard,
    ResetKeyboard,
    PlayingKeyboard,
    End};
  public GameState currentState; //public for debug purposes 
  public GameObject instPartFab;
  public GameObject circGraphic;
  public GameObject background;

  public TextAsset csvToUse;
  public Text questDisplay;
  public InputField keyboardText;
  public GameObject cardsView;
  public GameObject keyboardView;

  public List<Card> allCards = new List<Card>();
  public List<Term> allTerms = new List<Term>();
  public List<Term> unmasteredTerms = new List<Term>();

  public bool useImages;
  private bool handleCardPress, firstPress, handleKeyboardSubmit, firstSubmit;

  private int currentDifficulty;

  private float timeBetweenCorrAnswers;

  private int currIndex;
  private int amtOfCards;
  private int correctTermIndex;
  private int totalMastery;
  private int requiredMastery = 4;
  private int currentPhase;

  public string baseImagePath;
	public GameObject winningSlide;
	
  public Slider masteryMeter;

  private Vector3 questDispStart, questDispEnd;

  public AppManager manager;
	
  void Awake(){
//    manager = GameObject.FindGameObjectWithTag("appManager").GetComponent<AppManager>();
  }
	void Update () {

    switch(currentState){
      case GameState.ConfigCards:
        keyboardView.SetActive(false);
        cardsView.SetActive(true);
        currentDifficulty = 1;
        GameObject[] cardObjs = GameObject.FindGameObjectsWithTag("card");
        cardObjs = cardObjs.OrderBy(c=>c.name).ToArray();
        questDispStart = circGraphic.transform.localPosition;
        questDispEnd = circGraphic.transform.localPosition;
        questDispEnd.y = questDispEnd.y*-1;
        foreach(GameObject card in cardObjs){
          Card newCard = new Card(card, card.transform.Find("Text").GetComponent<Text>(), card.transform.Find("Image").GetComponent<Image>());
          newCard.thisIndiCard = card.GetComponent<indiCard>();
          allCards.Add(newCard);
        }
        allTerms = convertCSV(parseCSV(csvToUse));
        unmasteredTerms = allTerms.ToList();

        totalMastery = unmasteredTerms.Count*requiredMastery;
//        baseImagePath = baseImagePath + manager.currentAssignments[manager.currIndex];
        currentState = GameState.ResetCards;
        break;
      case GameState.ResetCards:
        Timer1.s_instance.Reset(15f);
        foreach(Card currCard in allCards){
          currCard.objAssoc.SetActive(false);
        }
        correctTermIndex = Random.Range(0,unmasteredTerms.Count);
        currentDifficulty = Mathf.Clamp(currentDifficulty, unmasteredTerms[correctTermIndex].mastery,  3); 
        amtOfCards = (int)(3*currentDifficulty);
        List<int> uniqueIndexes = generateUniqueRandomNum(amtOfCards, unmasteredTerms.Count, correctTermIndex);
        for(int i = 0; i<uniqueIndexes.Count;i++){
          if(!useImages){
            allCards[i].setCard(unmasteredTerms[uniqueIndexes[i]], false);
          }else{
            allCards[i].setCard(unmasteredTerms[uniqueIndexes[i]], true);
          }
          //Instantiate(instPartFab, allCards[i].objAssoc.transform.position+new Vector3(0f,0f,-10f), Quaternion.identity); 

        }
        questDisplay.text = unmasteredTerms[correctTermIndex].question;
        firstPress = true;
        currentState = GameState.PlayingCards;
        break;
      case GameState.PlayingCards:
        if(circleDrag.c_instance.tapped){
        }else if(!circleDrag.c_instance.tapped && circleDrag.c_instance.lastCardHit != null){
          cardHandler(int.Parse(circleDrag.c_instance.lastCardHit.gameObject.name));
          circleDrag.c_instance.reset();
        }else{
          circGraphic.transform.localPosition = Vector3.Lerp(
              questDispStart,
              questDispEnd,
              Timer1.s_instance.normTime
              );
        }
        if(handleCardPress){
          if(firstPress && allCards[currIndex].answer == unmasteredTerms[correctTermIndex].answer){
            background.SendMessage("correct");
            unmasteredTerms[correctTermIndex].mastery++;
            currentState = GameState.ResetCards;
            if(unmasteredTerms[correctTermIndex].mastery == requiredMastery*.75f){
              unmasteredTerms.RemoveAt(correctTermIndex);
              if(unmasteredTerms.Count == 0){//checkForNewPhase()){
                currentState = GameState.ConfigKeyboard;
              }
            }
          }else if(allCards[currIndex].answer == unmasteredTerms[correctTermIndex].answer){
            background.SendMessage("correct");
            unmasteredTerms[correctTermIndex].mastery--;
            currentState = GameState.ResetCards;
          }else{
            allCards[currIndex].objAssoc.SendMessage("incorrectAnswer");
          }
          background.SendMessage("incorrect");
          Timer1.s_instance.Pause();
          firstPress = false;
          handleCardPress = false;
          masteryMeter.value = getMastery();


        }
        if(Timer1.s_instance.timesUp && !Timer1.s_instance.pause){
          Timer1.s_instance.Pause();
          unmasteredTerms[correctTermIndex].mastery -=2;
        }
        break;
      case GameState.ConfigKeyboard:
        keyboardView.SetActive(true);
        cardsView.SetActive(false);

        unmasteredTerms = allTerms.ToList();
        currentState = GameState.ResetKeyboard;
        break;
      case GameState.ResetKeyboard:
        Timer1.s_instance.Reset(15f);
        firstSubmit = true;
        correctTermIndex = Random.Range(0,unmasteredTerms.Count);
        questDisplay.text = unmasteredTerms[correctTermIndex].question;

        
        currentState = GameState.PlayingKeyboard;
        break;
      case GameState.PlayingKeyboard:
        if(handleKeyboardSubmit){
          if(keyboardText.text.ToLower() == unmasteredTerms[correctTermIndex].answer){
            if(firstSubmit){
              unmasteredTerms[correctTermIndex].mastery++;
            }
            currentState = GameState.ResetKeyboard;
            if(unmasteredTerms[correctTermIndex].mastery == requiredMastery*.25f){
              unmasteredTerms.RemoveAt(correctTermIndex);
            }
          }else if(firstSubmit){
            unmasteredTerms[correctTermIndex].mastery -= 2;
          }
          Timer1.s_instance.Pause();
          firstSubmit = false;
          handleKeyboardSubmit = false;
          keyboardText.text = "";
				masteryMeter.value = getMastery();
        }
        if(Timer1.s_instance.timesUp && !Timer1.s_instance.pause){
          Timer1.s_instance.Pause();
          unmasteredTerms[correctTermIndex].mastery -=2;
        }
        break;
      case GameState.End:
			winningSlide.SetActive(true);
        break;
    }
  }

  public void cardHandler (int cardIndex) {
    handleCardPress = true;
    currIndex = cardIndex;

  }

  public void keyboardHandler(){
    print("HERE");
    handleKeyboardSubmit = true;
  }

  public void switchState(int newState){
    currentState = (GameState)newState;
  }

  bool checkForNewPhase(){
    bool newPhase = false;
    int amtOfMasteredTerms = allTerms.Count-unmasteredTerms.Count;
    int currentMastery = amtOfMasteredTerms*requiredMastery; 
    foreach(Term currTerm in unmasteredTerms){
      currentMastery += currTerm.mastery;
    }
	
    if(currentMastery >= totalMastery/2){
      newPhase = true;
      print("NEW PHASE IS TRUE!");
    }
    return newPhase;
  }

	float getMastery(){
		float floatToReturn;
		float amtOfMasteredTerms = allTerms.Count-unmasteredTerms.Count;
		float currentMastery = amtOfMasteredTerms*requiredMastery; 
		foreach(Term currTerm in unmasteredTerms){
			currentMastery += currTerm.mastery;
		}
		floatToReturn = (float)(currentMastery / (allTerms.Count*requiredMastery));
		return floatToReturn;
	}
  List<int> generateUniqueRandomNum(int amt, int randRange, int noThisNum = -1){
    List<int> listToReturn = new List<int>();
    if(noThisNum != -1){
      listToReturn.Add(noThisNum);
    }
    if(amt > randRange){
      amt = randRange;
      print("RANGE CANNOT BE MORE THAN AMT");
    }
    while(listToReturn.Count < amt){
      int newVal;
      do{
        newVal = Random.Range(0,randRange);
      }while(listToReturn.Contains(newVal));
      listToReturn.Add(newVal);
    }
    listToReturn = listToReturn.OrderBy(x=>Random.Range(0,listToReturn.Count)).ToList();
    return listToReturn;
  }

	List<string[]> parseCSV(TextAsset csvToParse){
		List<string[]> listToReturn = new List<string[]>();
		string[] lines = csvToParse.text.Split('\n');
		for(int i = 0;i<lines.Length;i++){
			string[] currLine = lines[i].Split(',');
      if(currLine.Length > 0){
        for(int j = 0;j<currLine.Length;j++){
          currLine[j] = currLine[j].Replace('\\',',');
          currLine[j] = currLine[j].ToLower();
        }
        listToReturn.Add(currLine);
      }
		}

		for(int i = 0; i < listToReturn.Count; i++){
			for(int j = 0; j < listToReturn[i].Length; j++){
				string temp = listToReturn[i][j].Replace('|',',');
				listToReturn[i][j] = temp;
			}
		}

		return listToReturn;
	}

  List<Term> convertCSV(List<string[]> inputString){
    List<Term> listToReturn = new List<Term>();
    foreach(string[] thisLine in inputString){
      if(thisLine.Length > 1){
        Term termToAdd;
        if(useImages){
          termToAdd = new Term(thisLine[0], thisLine[1], baseImagePath + "/" + thisLine[1]);
        }else{
          termToAdd = new Term(thisLine[0], thisLine[1]);
        }
        listToReturn.Add(termToAdd);
      }
    }
    return listToReturn;
  }

}
