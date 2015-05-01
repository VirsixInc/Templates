using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CSVParser : MonoBehaviour {

	public List<List<string>> listOfStrings = new List<List<string>>();
	
	public List<List<string>> Parse(TextAsset csvString){
		//get the array of lines
		string[] arrayOfLines = csvString.ToString().Split ("\n" [0]);

		//take each line, split by comma, and then populate list at that index
		for (int i=0; i<arrayOfLines.Length; i++) {
			string[] arrayOfStrings = arrayOfLines[i].Split(","[0]);
			listOfStrings.Add (new List<string>());
			for (int j=0; j<arrayOfStrings.Length; j++){
				string temp = arrayOfStrings[j].Replace('|',',');

				listOfStrings[i].Add (temp);
			}
		}
		return listOfStrings;
	}
}
