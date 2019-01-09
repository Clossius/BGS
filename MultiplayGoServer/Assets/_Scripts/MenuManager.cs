using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour {

	//************************************************
	// This script is for managing active menus.
	// ***********************************************

	// Assign menus to Game Objects
	public GameObject loginMenu;
	public GameObject lobbyMenu;
	public GameObject gameMenu;

	// Use this for initialization
	// Add menus to list and load first menu
	List <GameObject> menus;
	void Start () {
		menus = new List<GameObject> ();
		menus.Add (loginMenu); // 0
		menus.Add (lobbyMenu); // 1
		menus.Add (gameMenu); // 2

		ChangeMenu (0);
	}
	
	// Change menu
	// Activates the given menu and deactivates the others.
	public void ChangeMenu (int menu)
	{
		// Make sure at least one menu is active.
		bool menuActiveCondition = false;

		for (int i=0; i<menus.Count; i++)
		{
			if (menu == i) {
				menus [i].SetActive (true);
				menuActiveCondition = true;

			} else {
				menus [i].SetActive (false);
			}

		}

		// If no menu was loaded Log Error.
		if (!menuActiveCondition)
		{
			menus [0].SetActive (true);
			Debug.Log ("ERROR: Menu value out of range.");
		}

		if (menus[1].activeSelf)
		{
			GameObject[] gos = GameObject.FindGameObjectsWithTag ("PlayerData");
			GameObject topText2 = GameObject.FindGameObjectWithTag ("TopBarUserCount");
			TextMeshProUGUI text = topText2.GetComponent<TextMeshProUGUI> ();
			text.text = "Users: " + gos.Length.ToString ();
		}
	}

	public int GetActiveMenu ()
	{
		int num = -1;

		for (int i=0; i <menus.Count;i++)
		{
			if (menus[i].activeSelf){num = i;}
		}

		return num;
	}
}
