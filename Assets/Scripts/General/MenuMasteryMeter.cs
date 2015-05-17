using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuMasteryMeter : MonoBehaviour {

	Slider thisSlider;

	void Start () {
		thisSlider = GetComponent<Slider> ();
		thisSlider.value = transform.parent.GetComponent<Assignment> ().mastery;
	}

}
