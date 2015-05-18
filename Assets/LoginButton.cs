using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginButton : MonoBehaviour {

	public InputField username, password;

	public void SendUserData () {
		if (username.text != "" && password.text != "") {
			StartCoroutine(AppManager.s_instance.loginAcct(username.text, password.text));
			//Application.LoadLevel("AssignmentMenu");
		}
	}


}
