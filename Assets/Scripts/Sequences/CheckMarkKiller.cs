using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CheckMarkKiller : MonoBehaviour {

	public bool fadeOut;
	float startTime, fadeTime = .5f;

	void Start() {
		StartFade ();
	}

	void Update() {
		if (fadeOut) {
			float timePassed = (Time.time - startTime);
			float fracJourney = timePassed / fadeTime;
			GetComponent<Image>().color = Color.Lerp (new Color (1f, 1f, 1f, 1f), new Color (1f, 1f, 1f, 0f), fracJourney);
			if (fracJourney >= 1) {
				Destroy(transform.gameObject);
			}
		}
	}
	
	public void StartFade() {
		transform.localScale = new Vector3(1f,1f,1f); //fixed scale issue
			startTime = Time.time;
			fadeOut = true;
	}
}
