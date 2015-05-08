using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour {
	public bool fadeOut;
	public bool oneToZeroAlpha;
	public float startTime, fadeTime = 3;
	SpriteRenderer sprite;
	Image image;
	Text text;
	public bool isBlack = false;

	void Start() {
		sprite = GetComponent<SpriteRenderer> ();
		image = GetComponent<Image> ();
		text = GetComponent<Text> ();
	}

	void Update() {
		if (fadeOut) {
			float timePassed = (Time.time - startTime);
			float fracJourney = timePassed / fadeTime;
			if (!oneToZeroAlpha){
			if (sprite!=null)
				if (isBlack)
					sprite.color = Color.Lerp (new Color (0f, 0f, 0f, 0f), new Color (0f, 0f, 0f, 1f), fracJourney);
				else
					sprite.color = Color.Lerp (new Color (1f, 1f, 1f, 0f), new Color (1f, 1f, 1f, 1f), fracJourney);

			if (image!=null)
				if (isBlack)
					image.color = Color.Lerp (new Color (0f, 0f, 0f, 0f), new Color (0f, 0f, 0f, 1f), fracJourney);
				else
					image.color = Color.Lerp (new Color (1f, 1f, 1f, 0f), new Color (1f, 1f, 1f, 1f), fracJourney);
			if (text!=null)
				if (isBlack)
					text.color = Color.Lerp (new Color (0f, 0f, 0f, 0f), new Color (0f, 0f, 0f, 1f), fracJourney);
				else	
					text.color = Color.Lerp (new Color (1f, 1f, 1f, 0f), new Color (1f, 1f, 1f, 1f), fracJourney);
			}
			else {
				if (sprite!=null)
					if (isBlack)
						sprite.color = Color.Lerp (new Color (0f, 0f, 0f, 1f), new Color (0f, 0f, 0f, 0f), fracJourney);
				else
					sprite.color = Color.Lerp (new Color (1f, 1f, 1f, 1f), new Color (1f, 1f, 1f, 0f), fracJourney);
				
				if (image!=null)
					if (isBlack)
						image.color = Color.Lerp (new Color (0f, 0f, 0f, 1f), new Color (0f, 0f, 0f, 0f), fracJourney);
				else
					image.color = Color.Lerp (new Color (1f, 1f, 1f, 1f), new Color (1f, 1f, 1f, 0f), fracJourney);
				if (text!=null)
					if (isBlack)
						text.color = Color.Lerp (new Color (0f, 0f, 0f, 1f), new Color (0f, 0f, 0f, 0f), fracJourney);
				else	
					text.color = Color.Lerp (new Color (1f, 1f, 1f, 1f), new Color (1f, 1f, 1f, 0f), fracJourney);
			}
		}
	}
	
	public void StartFade() {
		startTime = Time.time;
		fadeOut = true;
	}

}