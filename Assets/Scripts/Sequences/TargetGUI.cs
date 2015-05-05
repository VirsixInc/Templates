using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TargetGUI : MonoBehaviour {

	public string correctAnswer;
	public bool isOccupied = false;
	public GameObject occupier;
	void Start () {
		if (transform.childCount > 0) {
			GetComponentInChildren<Text> ().text = "";
		}
	}


	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject == occupier) {
			if (occupier.GetComponent<DraggableGUI>().currentTarget == gameObject)
				occupier.GetComponent<DraggableGUI>().isSnapped = false;
			isOccupied = false;
			occupier = null;
		}
	}

	public void Reset() {
		isOccupied = false;
		occupier = null;
	}


}
