using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CardAssets : ScriptableObject {

	public List<TextAsset> CSVs;

	[System.Serializable]
	public class Subject {
		public Subject(string p_name, List<string[]> stringList) {
			dataPairs = new List<DataPair>();
			name = p_name;
			foreach(string[] stringArray in stringList) {
				dataPairs.Add(new DataPair(stringArray));
			}
		}

		[System.Serializable]
		public class DataPair {
			public DataPair(string[] stringArray) {
				definitions = new List<string>();
				term = stringArray[0];
				for(int i = 1; i < stringArray.Length; i++) {
					definitions.Add(stringArray[i]);
				}
			}
			[SerializeField]
			public string term;
			[SerializeField]
			public List<string> definitions;
		}

		public List<string[]> Data {
			get {
				List<string[]> data = new List<string[]>();
				for(int i = 0; i < dataPairs.Count; i++) {
					string[] termsAndDefininitions = new string[dataPairs[i].definitions.Count + 1];
					termsAndDefininitions[0] = dataPairs[i].term;
					for(int j = 0; j < dataPairs[i].definitions.Count; j++) {
						termsAndDefininitions[j + 1] = dataPairs[i].definitions[j];
					}
					data.Add(termsAndDefininitions);
				}
				return data;
			}
		}
		public List<DataPair> dataPairs;
		public string name;
	}

	public List<Subject> parsedCSVs;

	public void Parse() {
		parsedCSVs = new List<Subject>();
		foreach(TextAsset text in CSVs) {
			parsedCSVs.Add(new Subject(text.name, ParseCSV(text)));
			Debug.Log (parsedCSVs.Count);
		}
		//AssetDatabase.SaveAssets();
	}

	List<string[]> ParseCSV(TextAsset csvToParse) {
		List<string[]> listToReturn = new List<string[]>();
		string[] lines = csvToParse.text.Split('\n');
		for(int i = 0;i<lines.Length;i++){
			string[] currLine = lines[i].Split(',');
			listToReturn.Add(currLine);
		}
		return listToReturn;
	}

	// Use this for initializationß
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
