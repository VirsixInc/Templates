using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class AssignmentManager : MonoBehaviour {
	public List<Assignment> completedAssignments, incompleteAssignments;
	public Assignment assignment;
	public GameObject masteredHeader, assignmentsHeader;
	public GameObject assignmentGUIPrefab;
	float prefabHeight = 200f, initPrefabYPos = -50f, leftPos = -300f, rightPos = 300f, dividerHeight = 225f, spaceBetweenAssignments = 100f;
	public float upperBound; //for clamping scroll
	public static AssignmentManager s_instance;

	/*

	This functionality occurs in the Main Menu scene.
	
	At start, Assignment manager looks through app manager and gets the list of the assignments
	separate them into completed and incomplete assignments.

	the count of type assignment will be divided by two and rounded up
	loop through and instantiate each 

	instantiate each even prefab on the left at -150 

	*/

	void LoadAssignment() {
		//	foldername.csv, foldername.json, foldername.spritesheet
		// 
	}


	// Use this for initialization
	public void LoadAllAssignments(Assignment[] arrayOfAssignments){
		s_instance = this;
		foreach (Assignment assignment in arrayOfAssignments) {
			if (assignment.isCompleted)
				completedAssignments.Add(assignment); //this is currently being called when it should not
			else
				incompleteAssignments.Add(assignment);
		}

		//calculate upper bound based off amount of assignments
		if (incompleteAssignments.Count%2 == 0 && completedAssignments.Count%2 == 0)
			upperBound = AppManager.s_instance.userAssignments.Count * (prefabHeight + 2 * spaceBetweenAssignments + dividerHeight) / 2;
		else if (incompleteAssignments.Count%2 != 0 || completedAssignments.Count%2 != 0)
			upperBound = (AppManager.s_instance.userAssignments.Count + 2) * (prefabHeight + 2 * spaceBetweenAssignments + dividerHeight) / 2;
		InstantiateTemplates ();

	}

	void InstantiateTemplates() {
		//sets layout of GUI assignment objects
		Vector3 assignmentPosition;

		for (int i = 0; i < incompleteAssignments.Count; i++) { //FOR INCOMPLETE ASSIGNMENTS
			
			if (i%2 == 0){
				//adds how many completed assignments there are to the Y-value to that it all appears stacked on top one another
				assignmentPosition = new Vector3(leftPos, initPrefabYPos - i * (spaceBetweenAssignments + prefabHeight), 0);
			}
			else{
				assignmentPosition = new Vector3(rightPos, initPrefabYPos - (i - 1) * (spaceBetweenAssignments + prefabHeight), 0);
			}

			GameObject tempGUIPrefab = Instantiate(assignmentGUIPrefab) as GameObject;
			tempGUIPrefab.transform.SetParent(GameObject.Find("scrollerHolder").transform);
			tempGUIPrefab.transform.localScale = new Vector3(1,1,1); 
			tempGUIPrefab.transform.localPosition = assignmentPosition;
			tempGUIPrefab.GetComponent<Assignment>().SetAssignment(incompleteAssignments[i]);
		}

		float completedAssignmentOffset;
		if (incompleteAssignments.Count % 2 == 0)
			completedAssignmentOffset = -incompleteAssignments.Count * (spaceBetweenAssignments + prefabHeight) - dividerHeight;
		else
			completedAssignmentOffset = -(incompleteAssignments.Count+1) * (spaceBetweenAssignments + prefabHeight) - dividerHeight;

		for (int i = 0; i < completedAssignments.Count; i++) {
			//if i is even, put it in left column
			if (i%2 == 0){
				assignmentPosition = new Vector3(leftPos, initPrefabYPos - i * (spaceBetweenAssignments + prefabHeight) + completedAssignmentOffset,0);
			}
			else{
				assignmentPosition = new Vector3(rightPos, initPrefabYPos - (i - 1) * (spaceBetweenAssignments + prefabHeight) + completedAssignmentOffset); //i -1 because we want it to be next to the prefab on the left not below
			}
			GameObject tempGUIPrefab = Instantiate(assignmentGUIPrefab) as GameObject;
			tempGUIPrefab.transform.SetParent(GameObject.Find("scrollerHolder").transform, false);
			tempGUIPrefab.transform.localScale = new Vector3(1,1,1); 
			tempGUIPrefab.transform.localPosition = assignmentPosition; //local scale selects canvas space instead of w space
			tempGUIPrefab.GetComponent<Assignment>().SetAssignment(completedAssignments[i]);
		}

		//set divider position for the second 
		if (completedAssignments.Count > 0)
			masteredHeader.transform.localPosition = new Vector3 (0, completedAssignmentOffset + 125, 0);
		else
			masteredHeader.gameObject.SetActive (false);
		assignmentsHeader.transform.localPosition = new Vector3 (0, initPrefabYPos + 350f, 0);
	}

}
