using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonScript : MonoBehaviour {

	public void PlayButtonClicked (int ruleSet)
	{
		GameObject scripts = GameObject.Find ("_Scripts");

		scripts.GetComponent<ButtonManagerScript> ().PlayButton (ruleSet);
	}
}
