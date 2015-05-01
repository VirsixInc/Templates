using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextPopUpChild : MonoBehaviour {

	public Image parentImage;
	Text thisText;
	// Use this for initialization
	void Start () {
		parentImage = transform.parent.gameObject.GetComponent<Image> ();
		thisText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		thisText.color = new Color (thisText.color.r,thisText.color.g,thisText.color.b,parentImage.color.a);
	}
}
