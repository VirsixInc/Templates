using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class AssignmentStartButton : MonoBehaviour {

	EventTrigger thisEventTrigger;
	EventTrigger.Entry entry = new EventTrigger.Entry();
	PointerEventData eventData;
	
	// Use this for initialization
	void Start () {
		thisEventTrigger = GetComponent<EventTrigger>();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback.AddListener( (eventData) => {CallManager(); } );
		thisEventTrigger.delegates.Add(entry);
	}

	void CallManager() {
//		SoundManager.s_instance.PlaySound (SoundManager.s_instance.m_start);
		AppManager.s_instance.ClickHandler(transform.parent.gameObject.GetComponent<AssignmentGUI>().assignmentIndex);
		//AppManager.s_instance.currentAssignment = transform.parent.GetComponent<Assignment> ();
//		AppManager.s_instance.currentAppState = AppState.Playing;
	}
}
