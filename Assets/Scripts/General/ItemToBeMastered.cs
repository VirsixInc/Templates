using UnityEngine;
using System.Collections;

public class ItemToBeMastered {

	public float sequenceMastery = 0; //incremented by .25, used to adjust difficulty
	public GameObject itemGameObject;
	public ItemToBeMastered (float mastery, GameObject newGameObject){
		sequenceMastery = mastery;
		itemGameObject = newGameObject;
	}
}
