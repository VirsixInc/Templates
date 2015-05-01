using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopUpGraphic : MonoBehaviour {
	public bool fadeOut;
	public bool oneToZeroAlpha;
	public float startTime, fadeTime = 3;
	Image image;
	// Use this for initialization
	void Start () {
		image = GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update() {
		if (fadeOut) {
			float timePassed = (Time.time - startTime);
			float fracJourney = timePassed / fadeTime;
				if (image!=null)
					image.color = Color.Lerp (new Color (1f, 1f, 1f, .5f), new Color (1f, 1f, 1f, 0f), fracJourney);
			}
		}
	public void StartFade() {
		startTime = Time.time;
		fadeOut = true;
	}

}
