using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioSource m_wrong, m_correct, m_win, m_start, m_snap;

	public static SoundManager s_instance;

	void Awake() {
		s_instance = this;
		DontDestroyOnLoad (gameObject);
	}

	void Start () {
		m_win.Play ();
	}

	public void PlaySound (AudioSource source) {
		source.Play ();
	}
}
