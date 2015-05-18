using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class HotSpotClick : MonoBehaviour {

	EventTrigger thisEventTrigger;
	EventTrigger.Entry entry = new EventTrigger.Entry();
	PointerEventData eventData;

	// Use this for initialization
	void Start () {
		thisEventTrigger = GetComponent<EventTrigger>();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback.AddListener( (eventData) => {CallGameManager(); } );
		thisEventTrigger.delegates.Add(entry);
	}
	
	//HotSpotsGame.s_instance.SubmitAnswer(gameObject.name)
	void CallGameManager () {
		if(SoundManager.s_instance!=null)SoundManager.s_instance.PlaySound(SoundManager.s_instance.m_snap);
		HotSpotsGame.s_instance.SubmitAnswer(gameObject.name);
	}
}
