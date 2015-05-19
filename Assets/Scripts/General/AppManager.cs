using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using UnityEngine.UI;

public enum AppState {Login, Initialize, GetURLs, DownloadAssignments, MenuConfig, AssignmentMenu, Playing, LoadContent};

public class Assignment {
	public float timeToComplete = 0f;
	public string dateCompleted ="";
	public string dueDate = "11111111";
	public int version = 0;
	public bool isCompleted = false;
	public int month = 11, year = 1111, day = 11;
	public GameObject associatedGUIObject;

	public int mastery = 0;

	public string assignmentTitle = "";
  public string fullAssignTitle = "";
  public string type = "";
  public float secondsOnAssignment;
  public float timeAtLoad;


  public Assignment(string assignTitle, string templateType, string fullAssignTit=null){
    if(fullAssignTit != null){
      fullAssignTitle = fullAssignTit;
    }
    type = templateType;
    assignmentTitle = assignTitle;
  }
}

public class AppManager : MonoBehaviour {

  public bool localDebug;
	private AppState currentAppState;
	public static AppManager s_instance;
  public List<Assignment> currentAssignments = new List<Assignment>();
	public List<GameObject> userAssignments;
  public int currIndex;

	string[] assignmentURLs;
	string serverURL = "http://96.126.100.208:8080/client", folderName,
         username,
         password,
         masteryFilePath,
         filePathToUse;

  int assignsLoaded = 0, assignmentsDownloaded = 0, totalAssigns;

	List<string> assignmentURLsToDownload;

  bool urlsDownloaded, clicked, userExists, hardcoded = true;

	void Awake() {
    s_instance = this;
    masteryFilePath = Application.persistentDataPath + "mastery.info";
    DontDestroyOnLoad(transform.gameObject);
	}

	void Update () {
    print(currentAppState);
		switch (currentAppState) {
      case AppState.Login :
        if(userExists){
          currentAppState = AppState.Initialize;
          if(currentAppState == AppState.Initialize){
            if(hardcoded){
              currentAssignments.Add(new Assignment("hotspots_periodic","hotspots"));
              currentAssignments.Add(new Assignment("cards_Chemistry","cards"));
              currentAppState = AppState.MenuConfig;
            }
            Application.LoadLevel("AssignmentMenu");
          }
        }
        break;
      case AppState.Initialize :
        if(hardcoded){
          currentAppState = AppState.MenuConfig;
        }else{
          if(CheckForInternetConnection() && !localDebug){
            StartCoroutine (DownloadListOfURLs());
            currentAppState = AppState.GetURLs;
          }else{
            currentAppState = AppState.MenuConfig;
          }
        }
        break;
      case AppState.GetURLs :
        if(hardcoded){
          currentAppState = AppState.MenuConfig;
        }else{
          if(urlsDownloaded){
            currentAppState = AppState.DownloadAssignments;
          }
        }
        break;
      case AppState.DownloadAssignments :
        if(hardcoded){
          currentAppState = AppState.MenuConfig;
        }else{
          if(assignsLoaded == totalAssigns){
            currentAppState = AppState.LoadContent;
          }
        }
        break;
      case AppState.LoadContent:
        if(hardcoded){
          currentAppState = AppState.MenuConfig;
        }else{
          loadInLocalAssignments();
          currentAppState = AppState.MenuConfig;
        }
        break;
      case AppState.MenuConfig:
        AssignmentManager.s_instance.LoadAllAssignments(currentAssignments);
        currentAppState = AppState.AssignmentMenu;
        break;
      case AppState.AssignmentMenu :
        if(clicked){
//          filePathToUse = Application.persistentDataPath + "/" + currentAssignments[currIndex].fullAssignTitle;
          Application.LoadLevel(currentAssignments[currIndex].type);
          currentAssignments[currIndex].timeAtLoad = Time.time;
          clicked = false;
        }
        break;
      case AppState.Playing:
        break;
    }
	}

  void OnLevelWasLoaded(int level){
    if(level == 2){
      currentAppState = AppState.MenuConfig;
    }
  }

  public IEnumerator loginAcct(string name, string wrd){
    WWW www = new WWW(serverURL + "/logStudentIn?username=" + name + "&password=" + wrd);
    yield return www;
    if(www.text == "true"){
      userExists = true;
      username = name;
      password = wrd;
    }else{
      userExists = false;
    }
  }

  public void quitTemp(){
    currentAssignments[currIndex].secondsOnAssignment = currentAssignments[currIndex].timeAtLoad-Time.time;
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
        assignsLoaded++;
      }
    }
    urlsDownloaded = true;
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
    string[] masteryFile;
    if(File.Exists(masteryFilePath)){
      masteryFile = File.ReadAllLines(masteryFilePath);
    }else{
      File.WriteAllText(masteryFilePath, "");
    }
    foreach(FileInfo currFile in localFolder.GetFiles()){
      string[] path = currFile.ToString().Split('/');
      string assignName = path[path.Length-1];
      Assignment currAssign = generateAssignment(assignName);
      currAssign.mastery = pullAssignMastery(currAssign);
      currentAssignments.Add(currAssign);
    }
  }

  Assignment generateAssignment(string assignName){
    Assignment assignToReturn;
    string[] assign = assignName.Split('_');
    string assignWithoutExt = assignName.Split('.')[0];
    assignToReturn = new Assignment(assign[1],assign[0],assignWithoutExt);
    return assignToReturn;
  }

  public int pullAssignMastery(Assignment currAssign){
    int mastery = 0;
    string[] masteryFile = File.ReadAllLines(masteryFilePath);
    bool foundFile = false;
    if(masteryFile.Length > 0){
      foreach(string currLine in masteryFile){
        if(currLine.Contains(currAssign.fullAssignTitle)){
          foundFile = true;
          string[] operateLine = currLine.Split(',');
          mastery = int.Parse(operateLine[1]);
        }
      }
    }
    if(!foundFile){
      File.AppendAllText(masteryFilePath, currAssign.fullAssignTitle + ",0\n");
    }
    return mastery;
  }

  public void saveAssignmentMastery(Assignment assignToSave, int mastery){
    string[] masteryFile = File.ReadAllLines(masteryFilePath);
    bool foundFile = false;

    for(int i = 0; i<masteryFile.Length; i++){
      if(masteryFile[i].Contains(assignToSave.fullAssignTitle)){
        foundFile = true;
        masteryFile[i] = assignToSave.fullAssignTitle + "," + mastery.ToString();
        break;
      }
    }
    File.WriteAllText(masteryFilePath, String.Empty);
    File.WriteAllLines(masteryFilePath, masteryFile);
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
    clicked = true;
    currIndex = index;
	}

}
