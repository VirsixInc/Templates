using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public enum AppState {Initialize, GetURLs, DownloadAssignments, WaitForDownloads, ParseJSON, AssignmentMenu, Playing};

public class AppManager : MonoBehaviour {
	public AppState currentAppState = AppState.Initialize;
	public static AppManager s_instance;
	public Assignment currentAssignment;
	public List<GameObject> userAssignments; //the main list of assignments which can be updated and sent to and fro the server
	string serverURL = "http://192.168.1.8:8080/client", folderName;
  string username = "Alphonse";
  string password = "blargh";
	string[] assignmentURLs;
	List<string> assignmentURLsToDownload;
	bool areURLsUpdated;
	bool isAssignmentDataUpdated; //all assignment class instances created and populated
	int assignmentsDownloaded = 0;

	void Awake() {

		if (s_instance == null) {
			s_instance = this;
		}
	}

	void Update () {
		switch (currentAppState) {
      case AppState.Initialize :
        areURLsUpdated = false;
        currentAppState = AppState.GetURLs;
        break;
      case AppState.GetURLs :
        StartCoroutine (DownloadListOfURLs());
        currentAppState = AppState.DownloadAssignments;
        break;
      case AppState.DownloadAssignments :
        if (areURLsUpdated){
//          DownloadNewAssignments();
          currentAppState = AppState.WaitForDownloads;
        }
        break;
      case AppState.WaitForDownloads :
        if (assignmentsDownloaded == assignmentURLsToDownload.Count){
          currentAppState = AppState.AssignmentMenu;
        }
        break;
      
      case AppState.AssignmentMenu :

        break;
    }
	}



//	public void SortAssignments(){ //by due date and completion
//
//		for (int i = 0; i < userAssignments.Count; i++) {
//  
//			List<Assignment> tempList = new List<Assignment>();
//
//			tempList = userAssignments.OrderBy(ass => ass.year)
//				.ThenBy(ass => ass.month)
//					.ThenBy(ass => ass.day)
//						.ThenBy(ass => ass.isCompleted)
//							.Reverse ().ToList(); //reverse puts things in proper order 
//
//			userAssignments = new List<Assignment>(tempList);
//		}
//
//	}

	void CheckForNewAssignments() {
		for (int i = 0; i < assignmentURLs.Length; i++) {
			bool downloadRequired = true;
			//TODO rewrite this to check local database

//			foreach (GameObject x in userAssignments) { //check to see if URL has already been downloaded, could refactor to remove items from usrAss list
//				if (assignmentURLs[i] == x.GetComponent<Assignment>().URL){
//					downloadRequired = false;
//					break;
//				}
//			}

			if (downloadRequired) {
				assignmentURLsToDownload.Add(assignmentURLs[i]); //add URL to list of URLs that need to be downloaded
			}
		}
	}

  public int countStringOccurrences(string text, string pattern){
    // Loop through all instances of the string 'text'.
    int count = 0;
    int i = 0;
    while ((i = text.IndexOf(pattern, i)) != -1){
        i += pattern.Length;
        count++;
    }
    return count;
  }
	IEnumerator DownloadListOfURLs(){
		WWW www = new WWW(serverURL + "/pullData?username=" + username + "&password=" + password);
		yield return www;
    JSONObject allAssignments = ParseToJSON(www.text);
    int assignmentAmt = countStringOccurrences(www.text, "assignmentName");
    print(assignmentAmt);
    for(int i = 0; i<assignmentAmt;i++){
      string thisAssign = (string)(allAssignments[i].GetField("assignmentName").ToString());
      StartCoroutine(saveAssignmentInfo(thisAssign));
    }

		//StartCoroutine(saveAssignmentInfo(serverURL));
//		assignmentURLs = www.text.Split ('\n');
//		areURLsUpdated = true;
	}

  IEnumerator saveAssignment(string assignmentName){
    assignmentName = assignmentName.Replace("\"", "");
		WWW www = new WWW(serverURL + "/pullAssignment?assign=" + assignmentName);
    yield return www;
    JSONObject thisAssignmentInfo = ParseToJSON(www.text);
    string filePath = Application.persistentDataPath + assignmentName + ".data";
		if(File.Exists(filePath)) {
      print("FILE EXISTS");
		}else{
      FileStream file = File.Create (filePath);
      BinaryFormatter bf = new BinaryFormatter();
      bf.Serialize (file, www.text);
      file.Close ();
    }
  }
	IEnumerator saveAssignmentInfo(string assignmentName){
    assignmentName = assignmentName.Replace("\"", "");
		WWW www = new WWW(serverURL + "/pullAssignmentInfo?assign=" + assignmentName);
		yield return www;
    JSONObject thisAssignmentInfo = ParseToJSON(www.text);
    string filePath = Application.persistentDataPath + assignmentName + ".json";
		if(File.Exists(filePath)) {
      print("FILE EXISTS");
		}else{
      FileStream file = File.Create (filePath);
      BinaryFormatter bf = new BinaryFormatter();
      bf.Serialize (file, www.text);
      file.Close ();
    }
    StartCoroutine(saveAssignment(assignmentName));
	}

	JSONObject ParseToJSON (string txt) {
		JSONObject newJSONObject = JSONObject.Create (txt);
		return newJSONObject;
	}

}
