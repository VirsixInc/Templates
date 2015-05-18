using UnityEngine;
using System.Collections;

public class circleDrag : MonoBehaviour {

  public static circleDrag c_instance;
  public bool tapped, newCard;
  private Vector3 newPos;
  public GameObject lastCardHit;
  private indiCard card;
	void Start () {
    c_instance = this;
	
	}
	
	// Update is called once per frame
	void Update () {
    if(tapped){
      newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      newPos.z = 0f;
      transform.position = newPos; 
      Vector3 localPos = transform.localPosition;
      localPos.z = 0f;
      transform.localPosition = localPos; 
    }
	}
  void OnMouseDown(){
    tapped = true;
		if(SoundManager.s_instance!=null)SoundManager.s_instance.PlaySound(SoundManager.s_instance.m_snap);

  }
  void OnMouseUp(){
    tapped = false;
		if(SoundManager.s_instance!=null)SoundManager.s_instance.PlaySound(SoundManager.s_instance.m_snap);

  }

  void OnTriggerEnter2D(Collider2D coll){
    if(tapped){
      pullCardData(coll);
    }
  }
  void OnTriggerStay2D(Collider2D coll){
    if(tapped){
      if(coll.gameObject == lastCardHit && card != null){
        card.highLighted = true;
      }
      if(lastCardHit == null){
        pullCardData(coll);
      }
    }
  }
  void OnTriggerExit2D(Collider2D coll){
    if(tapped){
      if(card != null){
        card.highLighted = false;
        card = null;
      }
      lastCardHit = null;
    }
  }
  void pullCardData(Collider2D coll){
    if(card != null){
      card.highLighted = false;
    }
    lastCardHit = coll.gameObject;
    card = lastCardHit.GetComponent<indiCard>();
    newCard = true;
  }

  public void reset(){
    card.highLighted = false;
    card = null;
    lastCardHit = null;
  }
}
