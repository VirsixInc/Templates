using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour {
	
	public bool fadeIn;
	public float startTime, fadeTime = 1f;
	public Image image;
	SpriteRenderer sprite;
	//Outline myOutline;
	Color outlineinit;
	void Start() {
		//sprite = GetComponent<SpriteRenderer> ();
		image = GetComponent<Image> ();
		image.color = new Color(1f,1f,1f,0f);
		StartFade();
		StartCoroutine("DestroySprite");
		//myOutline = transform.parent.GetComponent<Outline>();
		//outlineinit = myOutline.effectColor;
		//print(outlineinit);

//		if(image.name == "XX(Clone)"){
//			myOutline.effectColor = Color.red;
//		}
//		else{
//			myOutline.effectColor = Color.green;
//		}



	}
	
	void Update() {
		if (fadeIn) {

			float timePassed = (Time.time - startTime);
			float fracJourney = timePassed / fadeTime;
			if (sprite!=null){
				sprite.color = Color.Lerp (new Color (1f, 1f, 1f, 0f), new Color (1f, 1f, 1f, 1f), fracJourney);
			}
			if (image!=null){
				image.color = Color.Lerp (new Color (1f, 1f, 1f, 1f), new Color (1f, 1f, 1f, 0f), fracJourney);
			}
//			if (myOutline!=null){
//				myOutline.effectColor = new Color (255f, 60f, 51f, 1f);
//			}
		}
	}

//	public void SetOutline(){
//		myOutline = transform.parent.GetComponent<Outline>();
//		outlineinit = myOutline.effectColor;
//	}

	public void StartFade() {
		startTime = Time.time;
		fadeIn = true;
	}
	IEnumerator DestroySprite() {
		yield return new WaitForSeconds(1.5f);
		//myOutline.effectColor = outlineinit;
		Destroy(gameObject);
	}


}