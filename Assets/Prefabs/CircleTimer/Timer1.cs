﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Timer1 : MonoBehaviour {
	public static Timer1 s_instance;

  public float timeLeft;
  public bool pause;
  public bool timesUp;
  public Color[] colorsToLerp = new Color[2];


  private float totalTime;
  private float amtOfSeconds;

  private Material timerMat;

  private Text thisText;

	void Awake () {
		s_instance = this;
    timerMat = transform.parent.GetComponent<Image>().material;
    thisText = GetComponent<Text>();
	}

	void Update () {
    if(pause || timesUp){    
    }else{
      timeLeft = totalTime-Time.time;
      timerMat.SetFloat("_Angle", Mathf.Lerp(-3.14f, 3.14f, (float)(timeLeft/amtOfSeconds)));
      timerMat.SetColor("_Color", Color.Lerp(colorsToLerp[1], colorsToLerp[0], (float)(timeLeft/amtOfSeconds)));
      thisText.text = ((int)(timeLeft)).ToString();
      if(timeLeft < 0f){
        timesUp = true;
      } 
    }
	}

	public void Reset(float amtOfTime){
    totalTime = Time.time + amtOfTime;
    amtOfSeconds = amtOfTime;
    pause = false;
    timesUp = false;
	}
}
