using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(CardAssets))]
public class CardAssetsEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector ();

		serializedObject.Update();

		CardAssets cardAssets = (CardAssets)target;
		
		if(GUILayout.Button("Parse")) {
			cardAssets.Parse();
		}

//		for(int i = 0, n = cardAssets.parsedCSVs.Count; i < n; i++) {
//			GUILayout.Label(cardAssets.parsedCSVs[i].name);
//			for(int j = 0, n2 = cardAssets.parsedCSVs[i].dataPairs.Count; j < n2 - 1; j++) {
//				EditorGUILayout.BeginHorizontal();
//				EditorGUILayout.TextField(cardAssets.parsedCSVs[i].dataPairs[j].term);
//				for(int k = 0; k < cardAssets.parsedCSVs[i].dataPairs[j].definitions.Count; k++) {
//					EditorGUILayout.TextField(cardAssets.parsedCSVs[i].dataPairs[j].definitions[k]);
//				}
//				EditorGUILayout.EndHorizontal();
//			}
//		}

		serializedObject.UpdateIfDirtyOrScript();
		serializedObject.ApplyModifiedProperties();
		AssetDatabase.SaveAssets();
	}
}

public static class CardAssetUtility {
	[MenuItem("CardAssets/Create")]
	static void Create() {
		CardAssets asset = (CardAssets)CardAssets.CreateInstance<CardAssets>();
		
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "Assets";
		} 
		else if (Path.GetExtension (path) != "") 
		{
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}
		
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + typeof(CardAssets).ToString() + ".asset");
		
		AssetDatabase.CreateAsset (asset, assetPathAndName);
		
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}
}