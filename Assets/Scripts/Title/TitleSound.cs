using UnityEngine;
using System.Collections;

public class TitleSound : MonoBehaviour {

	public float beepTime;

	public AudioSource beep;

	private bool played = false;


	void Start () {
	
	}

	void Update () {
	
		if( Time.time >= beepTime && !played ) {
			beep.Play ();
			played = true;
		}
	}
}
