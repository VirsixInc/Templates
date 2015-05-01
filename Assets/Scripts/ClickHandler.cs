using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class ClickHandler : MonoBehaviour {

	EventTrigger trigger;
	HotspotsManager myManager;
	EventTrigger.Entry entry = new EventTrigger.Entry();
	PointerEventData eventData;

	
	// Use this for initialization
	void Start () {
		trigger = gameObject.GetComponent<EventTrigger>();
		myManager = GameObject.Find("Manager").GetComponent<HotspotsManager>();
		entry.eventID = EventTriggerType.PointerUp;
		entry.callback.AddListener( (eventData) => {SendNameToManager(); } );
		trigger.delegates.Add(entry);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SendNameToManager(){
		myManager.selected = gameObject.name;
		print(myManager.selected);
	}

}
