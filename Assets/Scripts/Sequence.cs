using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Sequence  {
	
	public List<string> sequenceOfStrings;
	public float sequenceMastery = 0; //incremented by .25, used to adjust difficulty
	public int initIndex; //used to keep track of sequences throughout scene switching etc... similar to an ID
}
