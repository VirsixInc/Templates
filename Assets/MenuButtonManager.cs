using UnityEngine;
using System.Collections;

public class MenuButtonManager : MonoBehaviour {

	//this class handles the triggering of menu button events
	Fader[] faders;
	AnimationSlide[] animationSlides;
	void Awake(){
		faders = GetComponentsInChildren<Fader> ();
		animationSlides = GetComponentsInChildren<AnimationSlide> ();
	}
	
	void OnEnable() {
		ActivateMenuButtons ();
	}
	
	public void ActivateMenuButtons () {
		if (faders != null) {
			foreach (Fader f in faders) {
				f.StartFadeOut ();
			}
		}
		if (animationSlides != null) {
			//			animationSlide.s ();
		}
	}
	
	public void DeActivateMenuButtons () {
		if (faders != null) {
			foreach (Fader f in faders) {
				f.StartFadeIn ();
			}
		}
	}
}