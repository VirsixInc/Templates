using UnityEngine;
using System.Collections;

public class SignInButton : MonoBehaviour {

	public void OnClick(){
		//in the future will have it check the username and password
		Application.LoadLevel ("AssignmentMenu");
	}

}
