using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum HotSpotPhase {One,Two,Three};
public enum HotSpotGameState {Config, Display, Playing, AnswerSelected, CheckMastery, NextQuestion, Win}

public class HotSpotsGame : MonoBehaviour {


	HotSpotPhase curPhase = HotSpotPhase.One;
	HotSpotGameState curState = HotSpotGameState.Config;
	List<GameObject> individualElements;

	// Update is called once per frame
	void Update () {
	
		switch (curPhase) {
		}
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