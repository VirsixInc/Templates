using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class BlurLerp : MonoBehaviour {
	public GameObject MenuCanvas;


	public void Blur () {
		MenuCanvas.SetActive (true);
		GetComponent<Blur>().enabled = true;
		StartCoroutine ("BlurIn");
	}
	public void UnBlur() {
		StartCoroutine ("BlurOut");

	}

	IEnumerator BlurIn () {
		while (GetComponent<Blur> ().iterations < 12) {
			GetComponent<Blur> ().iterations += 1;
			yield return new WaitForSeconds(0.03f);
		}


	}

	IEnumerator BlurOut () {
		while (GetComponent<Blur> ().iterations > 0) {
			GetComponent<Blur> ().iterations -= 1;
			yield return new WaitForSeconds(0.03f);
		}
		GetComponent<Blur>().enabled = false;
		MenuCanvas.SetActive (false);


	}
}
