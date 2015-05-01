using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum AssignmentType {Cards, Buckets, Sequencing, HotSpots};

public class Assignment : MonoBehaviour {
	public AssignmentType assignmentType;
	public float timeToComplete = 0f;
	public string dateCompleted ="";
	public string dueDate = "11111111";
	public float masteryLevel = 0f;
	public string URL = "";
	public string assignmentTitle = "";
	public string templateType = "";
	public int version = 0;
	public bool isCompleted = false;
	public int month = 11, year = 1111, day = 11;
//	public Text descriptionText, typeText;

	// Use this for initialization
	void Start () {
		day = int.Parse (dueDate.Substring (0, 2));
		month = int.Parse (dueDate.Substring (2,2));
		year = int.Parse (dueDate.Substring (4, 4));
//		typeText = transform.GetChild (2).GetComponent<Text> ();
//		descriptionText = transform.GetChild (3).GetComponent<Text> ();
	}
	
	// Update is called once per frame
	public void SetAssignment(Assignment newAssignment){
		timeToComplete = newAssignment.timeToComplete;
		dateCompleted = newAssignment.dateCompleted;
		dueDate = newAssignment.dueDate;
		masteryLevel = newAssignment.masteryLevel;
		URL = newAssignment.URL;
		assignmentTitle = newAssignment.assignmentTitle;
		templateType = newAssignment.templateType;
		version = newAssignment.version;
		isCompleted = newAssignment.isCompleted;
//		descriptionText.text = 
	}
}
