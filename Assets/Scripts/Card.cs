using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Card : MonoBehaviour {
	//False is the definition side, true is the term side
	public enum cardState {front, back};
	public cardState myState;
	public string[] questionAnswer = new string[2];

	public Sprite[] cardSides = new Sprite[2];

	Text myText;

	bool flipping = false;
	Vector3 firstRotation, secondRotation;
	float lerpTimer;
	public float flipSpeed = 1f;
	bool sideFlipped = false;
	// Use this for initialization
	void Start () {
		myText = this.GetComponentInChildren<Text>();
		myState = cardState.front;
		DontDestroyOnLoad(this.gameObject);
		firstRotation = Vector3.zero;
		secondRotation = new Vector3(180f, 0f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		if(flipping) {
			transform.eulerAngles = Vector3.Lerp(firstRotation, secondRotation, lerpTimer);
			lerpTimer += Time.deltaTime / flipSpeed;
			if(lerpTimer > 0.5f && !sideFlipped) {
				sideFlipped = true;
				Vector3 temp = firstRotation;
				firstRotation = secondRotation;
				secondRotation = temp;
				if(myState == cardState.front){
					myState = cardState.back;
					this.GetComponent<Image>().sprite = cardSides[1];
				}
				else if(myState == cardState.back){
					myState = cardState.front;
					this.GetComponent<Image>().sprite = cardSides[0];
				}
			}
			if(lerpTimer > 1f) {
				sideFlipped = false;
				lerpTimer = 0f;
				flipping = false;
//				transform.eulerAngles = Vector3.Lerp(firstRotation, secondRotation, lerpTimer);
				Vector3 temp = firstRotation;
				firstRotation = secondRotation;
				secondRotation = temp;
			}
		}
		switch(myState){
			case cardState.front: 
				myText.text = questionAnswer[0];
				//myText.transform.eulerAngles = new Vector3(0f,0f,0f);
				break;

			case cardState.back:
				myText.text = questionAnswer[1];
				break;
		}
	}

	void OnMouseDown(){
		print ("CLICK");

		if(!flipping)
			flipping = true;
	}

	public void Flip(){
		if(!flipping){
			flipping = true;
		}
	}
}
