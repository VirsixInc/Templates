﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


[RequireComponent(typeof(EventTrigger))]
public class ChildHotSpot : MonoBehaviour {

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
		HotSpotsGame.s_instance.SubmitAnswer(transform.parent.gameObject.name);
	}
}
