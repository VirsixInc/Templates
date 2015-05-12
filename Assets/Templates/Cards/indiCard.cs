using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class indiCard : MonoBehaviour {

  public cardManager manager;

  private bool incorrect = false;
  private GameObject txtObj;
  private float speed = 2.5f;
  private Outline thisOut;

  public Color[] colors = new Color[2];
  void Start() {
    Button but = GetComponent<Button>();
    but.onClick.AddListener(() => manager.cardHandler(int.Parse(gameObject.name))); 
    thisOut = GetComponent<Outline>();
    txtObj = transform.GetChild(0).gameObject;
  }
	void Update () {
    if(incorrect){
      thisOut.effectColor = colors[1]; 
      transform.eulerAngles = Vector3.Lerp(transform.eulerAngles,new Vector3(0f,180f,0f),speed*Time.deltaTime);
      if(transform.eulerAngles.y > 60f && txtObj.activeSelf){
        txtObj.SetActive(false);
      }
      if(transform.eulerAngles.y > 170f){
      }
    }
	}

  void OnDisable(){
    transform.eulerAngles = new Vector3(0,0,0);
    thisOut.effectColor = Color.black;
    incorrect = false;
    txtObj.SetActive(true);
  }

  public void incorrectAnswer(){
    incorrect = true;
  }
}
