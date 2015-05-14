using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class copyColor : MonoBehaviour {


  public Image imageToCopy;

  private Image localImg;
	// Use this for initialization
	void Start () {
    localImg = GetComponent<Image>();
	
	}
	
	// Update is called once per frame
	void Update () {
    localImg.color = imageToCopy.color;
	
	}
}
