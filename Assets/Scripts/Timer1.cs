using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Timer1 : MonoBehaviour {
	public static Timer1 s_instance;

	void Awake () {
		s_instance = this;
	}

	void Update () {
	}
	
	public void Reset(){

	}
	
}

