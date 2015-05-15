using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class BlurLerp : MonoBehaviour {
	float blurVal;
	public bool fadeIn, fadeOut;
	float startTime, fadeTime = 2f;
	public GameObject MenuCanvas;
	void Start() {
		blurVal = GetComponent<Blur> ().iterations;
	}
	void Update() {
		if (fadeIn) {
			float timePassed = (Time.time - startTime);
			float fracJourney = timePassed / fadeTime;
			blurVal = Mathf.Lerp (0f, 5f, fracJourney);
			if (fracJourney >= 1) {
				fadeIn = false;
			}
		}
		if (fadeOut) {
			float timePassed = (Time.time - startTime);
			float fracJourney = timePassed / fadeTime;
			blurVal = Mathf.Lerp (5f, 0f, fracJourney);
			if (fracJourney >= 1) {
				GetComponent<Blur>().enabled = false;
				MenuCanvas.SetActive (false);

			}
		}
	}
	
	public void Blur () {
		MenuCanvas.SetActive (true);
		GetComponent<Blur>().enabled = true;
		startTime = Time.time;
		fadeIn = true;
	}
	public void UnBlur() {
		startTime = Time.time;
		fadeOut = true;
	}
}
