using UnityEngine;
using System.Collections;

public class indiCard : MonoBehaviour {

  private bool incorrect = false;
  private GameObject txtObj;
  private float speed = 2.5f;
  void Start() {
    txtObj = transform.GetChild(0).gameObject;
  }
	void Update () {
    if(incorrect){
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
    incorrect = false;
    txtObj.SetActive(true);
  }

  public void incorrectAnswer(){
    incorrect = true;
  }
}
