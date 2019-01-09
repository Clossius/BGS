using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySceneManager : MonoBehaviour {

	public GameObject LoginButton;
	public GameObject Guestbutton;
	public GameObject Inputfield;
	public GameObject status;

	//TODO: When someoen logs out, check for master client connection
	// before deactivating the buttons.

	// Use this for initialization
	void Start () {
		LoginButton.SetActive (false);
		Guestbutton.SetActive (false);
		Inputfield.SetActive (false);

		status.SetActive (true);
	}

	public void Connected ()
	{
		LoginButton.SetActive (true);
		Guestbutton.SetActive (true);
		Inputfield.SetActive (true);

		status.SetActive (false);
	}
}
