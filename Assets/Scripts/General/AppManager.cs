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
	string serverURL = "http://192.168.1.17:8080/pullData?username=Alphonse&password=blargh", folderName;
	string[] assignmentURLs;
	List<string> assignmentURLsToDownload;
	bool areURLsUpdated;
	bool isAssignmentDataUpdated; //all assignment class instances created and populated
	int assignmentsDownloaded = 0;

	void Start() {
	}

	void Update () {
		print (currentAppState);
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
				DownloadNewAssignments();
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

	IEnumerator DownloadListOfURLs(){
		print ("download list of URLs");
		WWW www = new WWW(serverURL);
		yield return www;
		StartCoroutine(SaveLocallyAndParseToAssignment(serverURL));
//		assignmentURLs = www.text.Split ('\n');
//		areURLsUpdated = true;
	}

	void DownloadNewAssignments() {
		assignmentsDownloaded = 0;
		foreach (string url in assignmentURLsToDownload) {
			StartCoroutine(SaveLocallyAndParseToAssignment(url));
		}
	}

	IEnumerator SaveLocallyAndParseToAssignment(string url){
		WWW www = new WWW(url);
		yield return www;
		assignmentsDownloaded++;
		string[] splitURL = url.Split ('/');
		string saveFolderName = splitURL [splitURL.Length - 1];

		BinaryFormatter bf = new BinaryFormatter ();
//		FileStream file = File.Create (Application.persistentDataPath + "/" + saveFolderName + "unparsedString.txt");
		FileStream file = File.Create (Application.persistentDataPath + "testFile");
//		print (Application.persistentDataPath + "/" + saveFolderName + "unparsedString.txt");
		bf.Serialize (file, www.text);
//		print ("JSON FILE:" + www.text);
		file.Close ();
		if (File.Exists(Application.persistentDataPath + "testFile")) {
			BinaryFormatter b = new BinaryFormatter();
			FileStream f = File.Open(Application.persistentDataPath + "testFile", FileMode.Open);
			string parseToJSON = (string)b.Deserialize(f);
		}


//		if (File.Exists(Application.persistentDataPath + "/" + saveFolderName + "unparsedString.txt")) {
//			BinaryFormatter b = new BinaryFormatter();
//			FileStream f = File.Open(Application.persistentDataPath + "/" + saveFolderName + "unparsedString.txt", FileMode.Open);
//			string parseToJSON = (string)b.Deserialize(f);
//		}


//		if (File.Exists(FILE_NAME)) 
//		{
//			Console.WriteLine("{0} already exists.", FILE_NAME);
//			return;
//		}
//		StreamWriter sr = File.CreateText("Assets/"+saveFolderName);
//		sr.WriteLine ("This is my file.");
//		sr.WriteLine ("I can write ints {0} or floats {1}, and so on.", 
//		              1, 4.2);
//		sr.Close();
		//here we save JSON to folder name
	}

	void LoadParseTextDataToAssignment() {

	}

	JSONObject ParseToJSON (string txt) {
		JSONObject newJSONObject = JSONObject.Create (txt);
		return newJSONObject;
	}


/*		
  		check URL’s available for each user against list of Assignments
		for each URL that is not yet downloaded, download JSON
		from JSON, parse JSON into a Assignment

		next, take parsed JSON and look for CSV, IMG, GenericType... and check true/false on each one
		if true download url/GenericType and save that to Assignment.folderName

		On LoadLevel, currentAssignment is set in AppManager which allows the Template to access the file name and parse the required CSV
		in the case that the level has already been worked on, it parses the csv and then applies a 
*/		





	void Awake() {
		//Screen.orientation = ScreenOrientation.Landscape;

		if (s_instance == null) {
			//If i am the first instance, make me the first Singleton
			s_instance = this;
		}

		else {
			//If a Singleton already exists and you find another reference in scene, destroy it
//			if (s_instance != this)
//					Destroy (gameObject);
		}
//		DontDestroyOnLoad (gameObject);

	}
}
