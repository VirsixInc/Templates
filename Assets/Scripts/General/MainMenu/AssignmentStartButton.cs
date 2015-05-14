using UnityEngine;
using System.Collections;

public class AssignmentStartButton : MonoBehaviour {

	public void OnClick() {
		SoundManager.s_instance.PlaySound (SoundManager.s_instance.m_start);
		//AppManager.s_instance.currentAssignment = transform.parent.GetComponent<Assignment> ();
		AppManager.s_instance.currentAppState = AppState.Playing;
		Application.LoadLevel (transform.parent.GetComponent<Assignment> ().assignmentTitle); //in the future this will be level type and then it will auto load the list
	}
}
