using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public enum HotSpotPhase {Elements,Typing,Groups};
public enum HotSpotGameState {Config, Display, Playing, AnswerSelected, CheckMastery, NextQuestion, Win}

public class HotSpotsGame : MonoBehaviour {

	public Text promptText;

	HotSpotPhase curPhase = HotSpotPhase.Elements;
	HotSpotGameState curState = HotSpotGameState.Config;
	GameObject[] individualElements, groups;
	List<ItemToBeMastered> phaseOneObjs, phaseTwoObjs, phaseThreeObjs, currentPhase;
	int currentIndex;
	// Update is called once per frame
	void Update () {
	
		switch (curState) {
		case HotSpotGameState.Config : 
			individualElements = GameObject.FindGameObjectsWithTag("elements");
			groups = GameObject.FindGameObjectsWithTag("groups");
			foreach (GameObject go in individualElements){
				ItemToBeMastered item = new ItemToBeMastered(go, 0);
				phaseOneObjs.Add(item);
				phaseTwoObjs.Add(item);
			}
			foreach (GameObject go in groups){
				ItemToBeMastered item = new ItemToBeMastered(go, 0);
				phaseThreeObjs.Add(item);
			}

			//sort all the lists alphabetically
			List<ItemToBeMastered> tempList = new List<ItemToBeMastered>();
			tempList = phaseOneObjs.OrderBy(item => item.itemGameObject.name).ToList();
			phaseOneObjs = new List<ItemToBeMastered>(tempList);

			List<ItemToBeMastered> tempList2 = new List<ItemToBeMastered>();
			tempList2 = phaseTwoObjs.OrderBy(item => item.itemGameObject.name).ToList();
			phaseTwoObjs = new List<ItemToBeMastered>(tempList2);

			List<ItemToBeMastered> tempList3 = new List<ItemToBeMastered>();
			tempList3 = phaseThreeObjs.OrderBy(item => item.itemGameObject.name).ToList();
			phaseThreeObjs = new List<ItemToBeMastered>(tempList3);

//			tempList.Clear (); //professional memory management, prob unnecessary but good to know
//			tempList2.Clear ();
//			tempList3.Clear ();
//
//			tempList.TrimExcess();
//			tempList2.TrimExcess();
//			tempList3.TrimExcess();

			break;
		}
	}



	void CheckForMastery() {
		if (currentIndex >= currentPhase.Count)
			currentIndex = 0; //loop around to beginning of list
		while (currentPhase[currentIndex].sequenceMastery==1f && currentPhase.Count != 0) { //skip over completed 
			currentPhase.Remove(currentPhase[currentIndex]);
			if (currentPhase.Count > currentIndex+1) {
				currentIndex++;
			}
			else 
				currentIndex = 0;
		}
	}
	void SetPhase() {

		//clear curList
		//copy other list

	}

}














//	public Image H, Li, Na, K, Pb, I, Ca, Mg, Be, He, Ne, F, O, N, C, Al, Si, P, S, Cl, Cr, Fe, Ni, Cu, Zn, Br, Ag, Sn, Au, Hg, U;
//
// NOTE:THIS IS A HORRIBLE MISTAKE NEVER TO BE REPEATED
//
//
//	// Use this for initialization
//	void Start () {
//		H = GameObject.Find ("H").GetComponent<Image> ();
//		Li = GameObject.Find ("Li").GetComponent<Image> ();
//		Na = GameObject.Find ("Na").GetComponent<Image> ();
//		K = GameObject.Find ("K").GetComponent<Image> ();
//		Pb = GameObject.Find ("Pb").GetComponent<Image> ();
//		I = GameObject.Find ("I").GetComponent<Image> ();
//		Ca = GameObject.Find ("Ca").GetComponent<Image> ();
//		Mg = GameObject.Find ("Mg").GetComponent<Image> ();
//		Be = GameObject.Find ("Be").GetComponent<Image> ();
//		He = GameObject.Find ("He").GetComponent<Image> ();
//		Ne = GameObject.Find ("Ne").GetComponent<Image> ();
//		F = GameObject.Find ("F").GetComponent<Image> ();
//		O = GameObject.Find ("O").GetComponent<Image> ();
//		N = GameObject.Find ("N").GetComponent<Image> ();
//		C = GameObject.Find ("C").GetComponent<Image> ();
//		Al = GameObject.Find ("Al").GetComponent<Image> ();
//		Si = GameObject.Find ("Si").GetComponent<Image> ();
//		P = GameObject.Find ("P").GetComponent<Image> ();
//		S = GameObject.Find ("S").GetComponent<Image> ();
//		Cl = GameObject.Find ("Cl").GetComponent<Image> ();
//		Cr = GameObject.Find ("Cr").GetComponent<Image> ();
//		Fe = GameObject.Find ("Fe").GetComponent<Image> ();
//		Ni = GameObject.Find ("Ni").GetComponent<Image> ();
//		Cu = GameObject.Find ("Cu").GetComponent<Image> ();
//		Zn = GameObject.Find ("Zn").GetComponent<Image> ();
//		Br = GameObject.Find ("Br").GetComponent<Image> ();
//		Ag = GameObject.Find ("Ag").GetComponent<Image> ();
//		Sn = GameObject.Find ("Sn").GetComponent<Image> ();
//		Au = GameObject.Find ("Au").GetComponent<Image> ();
//		Hg = GameObject.Find ("Hg").GetComponent<Image> ();
//		U = GameObject.Find ("U").GetComponent<Image> ();
//	}