using UnityEngine;
using System.Collections;

public class ReviewManager : MonoBehaviour {
	float LastMouseX = 0f;
	float CurrMouseX = 0f;
	float mouseDelta = 0f;
	public float scrollSpeed;
	GameObject cardParent;
	// Use this for initialization
	void Start () {
		cardParent = GameObject.Find("CardHolder");
	}
	
	// Update is called once per frame
	void Update () {
		MouseUpdate();

	}

	void MouseUpdate(){
		if(Input.GetButton("Fire1")){
			if(CurrMouseX == 0f){
				CurrMouseX = Input.mousePosition.x;
				LastMouseX = CurrMouseX;
			}
			else{
				LastMouseX = CurrMouseX;
				CurrMouseX = Input.mousePosition.x;
				mouseDelta = (LastMouseX - CurrMouseX)*Time.deltaTime*scrollSpeed;
				//PanCamera();
				moveCardHolder();
			}

		}
		else{
			CurrMouseX = 0f;
			LastMouseX = 0f;
		}


		//print("LastMouse: " + LastMouseX + ", CurrMouse: " + CurrMouseX + ", Delta: " + mouseDelta);
	}

	void PanCamera(){	
		Camera.main.transform.position = new Vector3((Camera.main.transform.position.x + mouseDelta), Camera.main.transform.position.y, Camera.main.transform.position.z);
	}
	void moveCardHolder(){
		cardParent.transform.position = new Vector3((cardParent.transform.position.x - mouseDelta), cardParent.transform.position.y, cardParent.transform.position.z);
	}


}
