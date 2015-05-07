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

};

public class cardManager : MonoBehaviour {

  private enum GameState{Config,Playing,Reset,End};
  GameState currentState; 
  public List<Card> allCards;

  private bool handleCardPress;
  private float currentDifficulty;
  private int currIndex;
  private int amtOfCards;
	
	void Update () {
    switch(currentState){
      case GameState.Config:
        GameObject[] cardObjs = GameObject.FindGameObjectsWithTag("card");
        cardObjs = cardObjs.OrderBy(c=>c.name).ToArray();
        foreach(GameObject card in cardObjs){
          Card newCard = new Card();
          newCard.objAssoc = card;
          newCard.objText = card.transform.GetChild(0).GetComponent<Text>();
          allCards.Add(newCard);
        }
        break;
      case GameState.Reset:
        for(int i = 0; i<amtOfCards;i++){
          allCards[i].objAssoc.SetActive(true);
        }
        break;
      case GameState.Playing:
        break;
      case GameState.End:
        break;
    }
	
	}

  public void cardHandler (int cardIndex) {
    handleCardPress = true;
    currIndex = cardIndex;

  }
}
