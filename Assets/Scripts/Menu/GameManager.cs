using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public bool menuActive;

	public float time;

    void Start()
    {
        //BACHELOR STUFF DELETE THIS AFTER
        PlayerPrefs.SetInt("Players", 8);
        PlayerPrefs.SetInt("Laps", 10);
        PlayerPrefs.SetInt("Quali", 1);
        PlayerPrefs.SetFloat("Difficulty", 0f);
    }

	void Update () {
	
		if( !menuActive )
			time += Time.deltaTime;


    }
}
