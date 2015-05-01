using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MasteryText : MonoBehaviour {

	public Text percentage;
	float masteryPercentage;
	public Slider masterySlider;
	public bool isWinningSlide = false;

	// Use this for initialization
	void Start () {
		if (masterySlider == null)
			masterySlider = GameObject.Find ("MasteryMeter").GetComponent<Slider>();
		percentage = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		masteryPercentage = masterySlider.value;
		int percentageValue = Mathf.RoundToInt(masteryPercentage*100);
		if (isWinningSlide)
			percentage.text = "Mastery Achieved! " + percentageValue.ToString () + "%";
		else
			percentage.text = "Mastery: " + percentageValue.ToString () + "%";
	}
}
