﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//this fader works on text, image, and sprite
//select different boolean values to determine what sort of fade you want to do

public class Fader : MonoBehaviour {
	//triggers fade
	public bool fading;
	//a = 1 to a = 0 or otherwise
	public bool oneToZeroAlpha;
	public bool customColors = false;
	public float startTime, fadeTime = 3;
	SpriteRenderer sprite;
	Image image;
	Text text;
	public bool isBlack = false;
	Color zeroWhite = new Color(1f, 1f, 1f, 0f), oneWhite = new Color(1f, 1f, 1f, 1f), zeroBlack = new Color(0f, 0f, 0f, 0f), oneBlack = new Color(0f, 0f, 0f, 1f);
	Color currColor;
	public Color customStartColor, customEndColor;
	void Start() {
		sprite = GetComponent<SpriteRenderer> ();
		image = GetComponent<Image> ();
		text = GetComponent<Text> ();
	}
	
	void Update() {
		if (fading) {
			float timePassed = (Time.time - startTime);
			float fracJourney = timePassed / fadeTime;
			//fade in
			if (customColors) {
				currColor = Color.Lerp (customStartColor, customEndColor);
			}

			else if (!oneToZeroAlpha) {
				if (isBlack) {
					currColor = Color.Lerp (zeroBlack, oneBlack, fracJourney);
				}
				//is white
				else {
					sprite.color = Color.Lerp (zeroWhite, oneWhite, fracJourney);
				}
			}

			//fade out
			else {
				if (isBlack) {
					currColor = Color.Lerp (oneBlack, zeroBlack, fracJourney);
				}
				//is white
				else {
					sprite.color = Color.Lerp (oneWhite, zeroWhite, fracJourney);
				}
			}

			if (sprite != null) {
				sprite.color = currColor;
			}
			if (text != null) {
				text.color = currColor;
			}
			if (image != null) {
				image.color = currColor;
			}
			if (fracJourney >= 1) {
				fading = false;
			}
		}
	}
	public void StartFade() {
		startTime = Time.time;
		fading = true;
	}

}
