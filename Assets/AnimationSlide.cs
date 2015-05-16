using UnityEngine;
using System.Collections;

public enum SlideDirection {Up, Down, Left, Right}
public class AnimationSlide : MonoBehaviour {

	//this is for menu GUI items to look extra cool when they pop up
	public float startTime, fadeTime = 1;

	float moveDistance = 10f;
	public SlideDirection thisSlideDirection = SlideDirection.Up;
	Vector3 startPos, endPos, originalPos, moveDirection;
	bool isSliding = false;

	void Awake () {
		originalPos = transform.position;
		switch (thisSlideDirection) {
		case SlideDirection.Down :
			moveDirection = -transform.up;
			break;

		case SlideDirection.Up :
			moveDirection = transform.up;
			break;

		case SlideDirection.Left :
			moveDirection = -transform.right;
			break;

		case SlideDirection.Right :
			moveDirection = transform.right;
			break;
		}
		startPos = originalPos;
		endPos = transform.position + moveDirection * moveDistance;
	}
	
	public void Slide() {
		transform.position = originalPos;
		isSliding = true;
		startTime = Time.time;
	}
	
	void Update() {
		float timePassed = (Time.time - startTime);
		float fracJourney = timePassed / fadeTime;
		//increment timer once per frame

		transform.position = Vector3.Lerp(startPos, endPos, fracJourney);
		if (fracJourney >= 1) {
			isSliding = false;
		}
	}
}
