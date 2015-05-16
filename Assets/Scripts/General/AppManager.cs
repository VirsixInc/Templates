using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using UnityEngine.UI;

public enum AppState {Initialize, GetURLs, DownloadAssignments, MenuConfig, AssignmentMenu, Playing, LoadContent};

public enum AssignmentType {Cards, Buckets, Sequencing, HotSpots, Undefined};

public class Assignment {
	public AssignmentType assignmentType;
	public float timeToComplete = 0f;
	public string dateCompleted ="";
	public string dueDate = "11111111";
	public float masteryLevel = 0f;
	public string assignmentTitle = "";
	public int version = 0;
	public bool isCompleted = false;
	public int month = 11, year = 1111, day = 11;
	public GameObject associatedGUIObject;
  public Assignment(string assignTitle, string templateType){
    assignmentType = getAssign(templateType);
    assignmentTitle = assignTitle;
  }

  AssignmentType getAssign(string tempType){
    switch(tempType){
      case "cards":
        return AssignmentType.Cards;
        break;
      case "hotspots":
        return AssignmentType.HotSpots;
        break;
      case "sequences":
        return AssignmentType.Sequencing;
        break;
      case "buckets":
        return AssignmentType.Buckets;
        break;
      default:
        return AssignmentType.Undefined;
        break;
    }
  }
}

public class AppManager : MonoBehaviour {

  public bool localDebug;
	public AppState currentAppState = AppState.Initialize;
	public static AppManager s_instance;
  public List<Assignment> currentAssignments = new List<Assignment>();
	public List<GameObject> userAssignments; //the main list of assignments which can be updated and sent to and fro the server
	string serverURL = "http://192.168.1.8:8080/client", folderName;
  string username = "Alphonse";
  string password = "blargh";

  string masteryFilePath;

	string[] assignmentURLs;
	List<string> assignmentURLsToDownload;
	int assignmentsDownloaded = 0;

  bool urlsDownloaded;
  int assignsLoaded = 0, totalAssigns;

	void Awake() {
    masteryFilePath = Application.persistentDataPath + "mastery.info";
		if (s_instance == null) {
			s_instance = this;
		}
	}

	void Update () {
		switch (currentAppState) {
      case AppState.Initialize :
        if(CheckForInternetConnection() && !localDebug){
          StartCoroutine (DownloadListOfURLs());
          currentAppState = AppState.GetURLs;
        }else{
          currentAppState = AppState.LoadContent;
        }
        break;
      case AppState.GetURLs :
        if(urlsDownloaded){
          currentAppState = AppState.DownloadAssignments;
        }
        break;
      case AppState.DownloadAssignments :
        if(assignsLoaded == totalAssigns){
          currentAppState = AppState.MenuConfig;
        }
        break;
      case AppState.LoadContent:
        loadInLocalAssignments();
        currentAppState = AppState.MenuConfig;
        break;
      case AppState.MenuConfig:
        AssignmentManager.s_instance.LoadAllAssignments(currentAssignments);
        currentAppState = AppState.AssignmentMenu;
        break;
      case AppState.AssignmentMenu :
        break;
    }
    print(currentAppState);
	}

  public int countStringOccurrences(string text, string pattern){
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
      string filePath = (Application.persistentDataPath + "/" + thisAssign).Replace("\"", "");
      if(!File.Exists(filePath + ".data")){
        StartCoroutine(saveAssignment(thisAssign));
      }else{
        currentAssignments.Add(generateAssignment(thisAssign));
        assignsLoaded++;
      }
    }
    urlsDownloaded = true;
	}

  Assignment generateAssignment(string assignName){
    Assignment assignToReturn;
    string[] assign = assignName.Split('_');
    assignToReturn = new Assignment(assign[1],assign[0]);
    return assignToReturn;
  }
  IEnumerator saveAssignment(string assignmentName){
    assignmentName = assignmentName.Replace("\"", "");
		WWW www = new WWW(serverURL + "/pullAssignment?assign=" + assignmentName);
    yield return www;
    JSONObject thisAssignmentInfo = ParseToJSON(www.text);
    string filePath = Application.persistentDataPath + "/" + assignmentName + ".data";
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
    File.AppendAllText(masteryFilePath, assignmentName + ",0\n");
    File.WriteAllLines(filePath, assignmentContent.ToArray());
    currentAssignments.Add(generateAssignment(assignmentName));
    assignsLoaded++;
  }

  public static bool CheckForInternetConnection(){
    try{
      using (var client = new WebClient())
      using (var stream = client.OpenRead("http://www.google.com")){
        return true;
      }
    }catch{
      return false;
    }
  }

  void loadInLocalAssignments(){
    DirectoryInfo localFolder = new DirectoryInfo(Application.persistentDataPath + "/");
    string[] masteryFile = File.ReadAllLines(masteryFilePath);
    foreach(FileInfo currFile in localFolder.GetFiles()){
      print(currFile.ToString());
      string[] path = currFile.ToString().Split('/');
      string assignName = path[path.Length];
      generateAssignment(assignName);

    }
  }

  public IEnumerator uploadAssignMastery(string assignmentName, int mastery){
    assignmentName = assignmentName.Replace("\"", "");
		WWW www = new WWW(serverURL + "/setAssignmentMastery?assignmentName=" + assignmentName + "&student=" + username + "&mastery=" + mastery.ToString());
    yield return www;
  }

	JSONObject ParseToJSON (string txt) {
		JSONObject newJSONObject = JSONObject.Create (txt);
		return newJSONObject;
	}

	public void ClickHandler (int index) {

	}

}
