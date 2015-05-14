using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public enum AppState {Initialize, GetURLs, DownloadAssignments, MenuConfig, AssignmentMenu, Playing};

public enum AssignmentType {Cards, Buckets, Sequencing, HotSpots};

public class Assignment {
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

	public void init() {
		day = int.Parse (dueDate.Substring (0, 2));
		month = int.Parse (dueDate.Substring (2,2));
		year = int.Parse (dueDate.Substring (4, 4));
	}
	
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
	}
}

public class AppManager : MonoBehaviour {
	public AppState currentAppState = AppState.Initialize;
	public static AppManager s_instance;
	public Assignment currentAssignment;
	public List<GameObject> userAssignments; //the main list of assignments which can be updated and sent to and fro the server
	string serverURL = "http://localhost:8080/client", folderName;
  string username = "Alphonse";
  string password = "blargh";
	string[] assignmentURLs;
	List<string> assignmentURLsToDownload;
	int assignmentsDownloaded = 0;

  bool urlsDownloaded;

  int assignsLoaded = 0, totalAssigns;

	void Awake() {

		if (s_instance == null) {
			s_instance = this;
		}
	}

	void Update () {
		switch (currentAppState) {
      case AppState.Initialize :
        StartCoroutine (DownloadListOfURLs());
        currentAppState = AppState.GetURLs;
        break;
      case AppState.GetURLs :
        if(urlsDownloaded){
          currentAppState = AppState.DownloadAssignments;
        }
        break;
      case AppState.DownloadAssignments :
        if(assignsLoaded == totalAssigns){
          currentAppState = AppState.AssignmentMenu;
        }
        break;
      case AppState.MenuConfig:
        break;
      case AppState.AssignmentMenu :
        break;
    }
    print(currentAppState);
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
    urlsDownloaded = false;
		yield return www;
    JSONObject allAssignments = ParseToJSON(www.text);
    totalAssigns = allAssignments.Count;
    for(int i = 0; i<totalAssigns;i++){
      string thisAssign = (string)(allAssignments[i].GetField("assignmentName").ToString());
      StartCoroutine(saveAssignmentInfo(thisAssign));
    }
    urlsDownloaded = true;
	}

  IEnumerator saveAssignment(string assignmentName){
    assignmentName = assignmentName.Replace("\"", "");
		WWW www = new WWW(serverURL + "/pullAssignment?assign=" + assignmentName);
    yield return www;
    JSONObject thisAssignmentInfo = ParseToJSON(www.text);
    string filePath = Application.persistentDataPath + assignmentName + ".data";
    List<string> assignmentContent = new List<string>();
    foreach(JSONObject allIndArgs in thisAssignmentInfo.list){
      foreach(JSONObject indArg in allIndArgs.list){
        if(indArg.Count>0){
          string[] argToAdd = new string[indArg.Count];
          int iterator = 0;
          foreach(JSONObject arg in indArg.list){
            argToAdd[iterator] = arg.ToString();
            iterator++;
          }
          string concatString = String.Join(",",argToAdd);
          concatString = concatString.Replace("\"", "");
          assignmentContent.Add(concatString);
        }
      }
    }
    assignsLoaded++;
    File.WriteAllLines(filePath, assignmentContent.ToArray());
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
