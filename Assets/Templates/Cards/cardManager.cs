using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Card{
  public GameObject objAssoc;
  public Text objText;
  public string answer;
  public string question;
  public Card(GameObject objRef, Text objTxtRef){
    objAssoc = objRef;
    objText = objTxtRef;
  }
  public void setCard(Term termToUse){
    answer = termToUse.answer;
    question = termToUse.question;
    objText.text = answer;
    objAssoc.SetActive(true);
  }
};

public class Term{
  public string answer;
  public string question;
  public int mastery = 0;
  public bool mastered = false;
  public Term(string newQuestion, string newAnswer){
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
  public List<Card> allCards = new List<Card>();
  public List<Term> allTerms = new List<Term>();
  public List<Term> unmasteredTerms = new List<Term>();
  public TextAsset csvToUse;
  public Text questDisplay;
  public InputField keyboardText;
  public GameObject cardsView;
  public GameObject keyboardView;

  private bool handleCardPress, firstPress, handleKeyboardSubmit, firstSubmit;

  private int currentDifficulty;

  private float timeBetweenCorrAnswers;

  private int currIndex;
  private int amtOfCards;
  private int correctTermIndex;

  private int totalMastery;
  private int requiredMastery = 8;

  private int currentPhase;
	
	void Update () {
    switch(currentState){
      case GameState.ConfigCards:
        keyboardView.SetActive(false);
        cardsView.SetActive(true);
        currentDifficulty = 1;
        GameObject[] cardObjs = GameObject.FindGameObjectsWithTag("card");
        cardObjs = cardObjs.OrderBy(c=>c.name).ToArray();
        foreach(GameObject card in cardObjs){
          Card newCard = new Card(card, card.transform.GetChild(0).GetComponent<Text>());
          allCards.Add(newCard);
        }
        allTerms = convertCSV(parseCSV(csvToUse));
        unmasteredTerms = allTerms.ToList();
        currentState = GameState.ResetCards;

        totalMastery = unmasteredTerms.Count*requiredMastery;
        break;
      case GameState.ResetCards:
        Timer1.s_instance.Reset(15f);
        foreach(Card currCard in allCards){
          currCard.objAssoc.SetActive(false);
        }
        correctTermIndex = Random.Range(0,unmasteredTerms.Count);
        currentDifficulty = Mathf.Clamp(unmasteredTerms[correctTermIndex].mastery/2, 1, requiredMastery/2); 
        amtOfCards = (int)(3*currentDifficulty);
        List<int> uniqueIndexes = generateUniqueRandomNum(amtOfCards, unmasteredTerms.Count, correctTermIndex);
        for(int i = 0; i<uniqueIndexes.Count;i++){
          allCards[i].setCard(unmasteredTerms[uniqueIndexes[i]]);
        }
        questDisplay.text = unmasteredTerms[correctTermIndex].question;
        firstPress = true;
        currentState = GameState.PlayingCards;
        break;
      case GameState.PlayingCards:
        if(handleCardPress){
          if(firstPress && allCards[currIndex].answer == unmasteredTerms[correctTermIndex].answer){
            unmasteredTerms[correctTermIndex].mastery++;
            currentState = GameState.ResetCards;
            if(unmasteredTerms[correctTermIndex].mastery == requiredMastery*.75f){
              unmasteredTerms.RemoveAt(correctTermIndex);
              if(unmasteredTerms.Count == 0){//checkForNewPhase()){
                currentState = GameState.ConfigKeyboard;
              }
            }
          }else if(allCards[currIndex].answer == unmasteredTerms[correctTermIndex].answer){
            unmasteredTerms[correctTermIndex].mastery--;
            currentState = GameState.ResetCards;
          }else{
            allCards[currIndex].objAssoc.SendMessage("incorrectAnswer");
          }
          Timer1.s_instance.Pause();
          firstPress = false;
          handleCardPress = false;
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
            currentState = GameState.ResetKeyboard;
            if(unmasteredTerms[correctTermIndex].mastery == requiredMastery*.25f){
              unmasteredTerms.RemoveAt(correctTermIndex);
            }
          }
          firstSubmit = false;
          handleKeyboardSubmit = false;
          keyboardText.text = "";
        }
        break;
      case GameState.End:
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
    print(totalMastery/2);
    print(currentMastery);
    if(currentMastery >= totalMastery/2){
      newPhase = true;
      print("NEW PHASE IS TRUE!");
    }
    return newPhase;
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
      Term termToAdd = new Term(thisLine[0], thisLine[1]);
      listToReturn.Add(termToAdd);
    }
    return listToReturn;
  }

}
