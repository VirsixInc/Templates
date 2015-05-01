using UnityEngine;
using System.Collections;
using System;

public class ContentDownloader : MonoBehaviour {

	//each asset bundle has to be specifically exported for android or iOS
	//thus, every update made for brainrush must be  multiplatform multiexport

	/*

	This ContentDownloader needs to check if there is new content to be downloaded
	Accesses a url that has a list of urls
	compare that list to the current stored XML list of URL that have already been downloaded, only download the new ones
	If there is, it needs to choose the content appropiate for the platform, Android or iOS
	
	Once the Content is downloaded using WWW www = WWW.LoadFromCacheOrDownload (urlToAssetBundle, version, crc32Checksum); 
	which can be waited for by yield return www;

	The content must be stored locally into a folder of either text or images using File.WriteAllBytes
	The content have a tag or title that can be receieved 
	
	Cached AssetBundles are uniquely identified solely by the filename and version number; all domain and path information in url is ignored by Caching	

	 */



//		IEnumerator Start ()
//		{
//			var www = WWW.LoadFromCacheOrDownload("http://brainrush.com/Lesson", 5);
//			yield return www;
//			if(!string.IsNullOrEmpty(www.error))
//			{
//				Debug.Log(www.error);
//				return;
//			}
//			var myLoadedAssetBundle = www.assetBundle;
//			
//			var asset = myAssetBundle.mainAsset;
//		}

	// C#
//	IEnumerator GetAssetBundle() {
//		WWW download;
//		string url = "http://somehost/somepath/someassetbundle.assetbundle";
//		download = WWW.LoadFromCacheOrDownload(url, 0);
//		
//		yield return download;
//		
//		AssetBundle assetBundle = download.assetBundle;
//		if (assetBundle != null) {
//			// Alternatively you can also load an asset by name (assetBundle.Load("my asset name"))
//			Object go = assetBundle.mainAsset;
//			
//			if (go != null)
//				Instantiate(go);
//			else
//				Debug.Log("Couldn't load resource");    
//		} else {
//			Debug.Log("Couldn't load resource");    
//		}
//	}

//	public void SaveDownloadedAsset(WWW obj) {
//		try {
//			// create the directory if it doesn't already exist
//			if (!Directory.Exists(Path.Combine(parent, child))) {
//				Directory.CreateDirectory(Path.Combine(parent, child));
//			}
//			
//			// write out the file
//			string filePath = Path.Combine(Path.Combine(parent, child), obj.assetBundle.mainAsset.name + ".prefab");    // grab the filepath
//			var bytes = obj.bytes;  // initialize the byte string
//			
//			File.WriteAllBytes(filePath, bytes);    // write the object out to disk
//		}
//		catch(Exception e) {
//			Debug.Log("Couldn't save file: " + System.Environment.NewLine + e);
//		}
//	}


}
