using UnityEngine;
using System.Collections;

public class AnimationSlide : MonoBehaviour {

	//this is for menu GUI items to look extra cool when they pop up

	float lerpTime = 0.5f;
	float currentLerpTime;
	
	float moveDistance = 10f;
	
	Vector3 startPos, endPos, originalPos;

	void Awake () {
		originalPos = transform.position;
	}
	
	protected void Start() {
		startPos = originalPos;
		endPos = transform.position + transform.up * moveDistance;
	}
	
	protected void Update() {
		//reset when we press spacebar
		if (Input.GetKeyDown(KeyCode.Space)) {
			currentLerpTime = 0f;
		}
		
		//increment timer once per frame
		currentLerpTime += Time.deltaTime;
		if (currentLerpTime > lerpTime) {
			currentLerpTime = lerpTime;
		}
		
		//lerp!
		float perc = currentLerpTime / lerpTime;
		transform.position = Vector3.Lerp(startPos, endPos, perc);
	}
}
