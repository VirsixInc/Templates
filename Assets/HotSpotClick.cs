using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class HotSpotClick : MonoBehaviour {

	EventTrigger thisEventTrigger;
	// Use this for initialization
	void Start () {
		thisEventTrigger = GetComponentInParent<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback.AddListener( (eventData) => { HotSpotsGame.s_instance.SubmitAnswer(gameObject.name); } );
		thisEventTrigger.delegates.Add(entry);
	}
	
	//HotSpotsGame.s_instance.SubmitAnswer(gameObject.name)
	void Update () {
	
	}
}
