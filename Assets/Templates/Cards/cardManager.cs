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

  private enum GameState{Config,Playing,Reset,End};
  GameState currentState; 
  public List<Card> allCards = new List<Card>();
  public List<Term> allTerms = new List<Term>();
  public List<Term> unmasteredTerms = new List<Term>();
  public TextAsset csvToUse;
  public Text questDisplay;

  private bool handleCardPress, firstPress;

  private float currentDifficulty;

  private int currIndex;
  private int amtOfCards;
  private int correctTermIndex;

  private int totalMastery;
  private int requiredMastery = 6;
	
	void Update () {
    switch(currentState){
      case GameState.Config:
        currentDifficulty = 1f;
        GameObject[] cardObjs = GameObject.FindGameObjectsWithTag("card");
        cardObjs = cardObjs.OrderBy(c=>c.name).ToArray();
        foreach(GameObject card in cardObjs){
          Card newCard = new Card(card, card.transform.GetChild(0).GetComponent<Text>());
          allCards.Add(newCard);
        }
        allTerms = convertCSV(parseCSV(csvToUse));
        unmasteredTerms = allTerms;
        currentState = GameState.Reset;

        totalMastery = unmasteredTerms.Count*requiredMastery;
        break;
      case GameState.Reset:
        Timer1.s_instance.Reset(15f);
        amtOfCards = (int)(3*currentDifficulty);
        foreach(Card currCard in allCards){
          currCard.objAssoc.SetActive(false);
        }
        List<int> uniqueIndexes = generateUniqueRandomNum(amtOfCards, unmasteredTerms.Count);
        for(int i = 0; i<uniqueIndexes.Count;i++){
          allCards[i].setCard(unmasteredTerms[uniqueIndexes[i]]);
        }
        correctTermIndex = uniqueIndexes[Random.Range(0,uniqueIndexes.Count)];
        questDisplay.text = unmasteredTerms[correctTermIndex].question;
        firstPress = true;
        currentState = GameState.Playing;
        break;
      case GameState.Playing:
        if(handleCardPress){
          if(firstPress && allCards[currIndex].answer == unmasteredTerms[correctTermIndex].answer){
            unmasteredTerms[correctTermIndex].mastery++;
            if(unmasteredTerms[correctTermIndex].mastery == requiredMastery){
              unmasteredTerms.RemoveAt(correctTermIndex);
            }
            currentState = GameState.Reset;
          }else if(allCards[currIndex].answer == unmasteredTerms[correctTermIndex].answer){
            currentState = GameState.Reset;
          }else{
            allCards[currIndex].objAssoc.SendMessage("incorrectAnswer");
          }
          firstPress = false;
          handleCardPress = false;
        }
        break;
      case GameState.End:
        break;
    }
	
	}

  List<int> generateUniqueRandomNum(int amt, int randRange){
    List<int> listToReturn = new List<int>();
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

  public void cardHandler (int cardIndex) {
    handleCardPress = true;
    currIndex = cardIndex;

  }
}
