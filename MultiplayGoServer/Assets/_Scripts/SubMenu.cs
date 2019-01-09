using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class SubMenu : MonoBehaviour {

	public GameObject title;
	public GameObject pOneUsername;
	public GameObject pTwoUsername;
	public GameObject pOneTimeText;
	public GameObject pTwoTimeText;
	public GameObject subMenu;
	public Image pOneColor;
	public Image pTwoColor;

	// When a player joins a room
	// this function gets called.
	// Open the submenu and initialize text.
	public void JoinRoom (string roomID)
	{
		subMenu.SetActive (true);

		title = GameObject.FindGameObjectWithTag ("SubMenuTitle");
		TextMeshProUGUI text = title.GetComponent<TextMeshProUGUI> ();

		text.text = "Room: " + roomID;
	}

	// Update the player information in a room.
	public void UpdatePlayers (string playerOne, string playerTwo)
	{
		TextMeshProUGUI pOneUser = pOneUsername.GetComponent<TextMeshProUGUI> ();
		TextMeshProUGUI pTwoUser = pTwoUsername.GetComponent<TextMeshProUGUI> ();

		pOneUser.text = playerOne;
		pTwoUser.text = playerTwo;

	}

	// Set the colors of the play information image
	// Takes in a list of int for the player colors
	// and sets the colors by the int provided.
	// int provided cannot be greater than the default Color size.
	public void SetPlayerColors (List<int> colors)
	{
		List<Color> defaultColors = new List<Color> ();
		defaultColors.Add (Color.black);
		defaultColors.Add (Color.white);

		// Check for errors
		if (colors.Count < 2){Debug.Log("ERROR: colors given are not enough for the amount of players.");}

		for(int i=0;i<colors.Count;i++)
		{
			if(colors[i] < 0){Debug.Log ("ERROR: int less than 0 at i = " + i.ToString());}
			if(colors[i] >= defaultColors.Count){Debug.Log ("ERROR: int greater than max default color value at i = " + i.ToString());}
		}

		// Set Colors
		pOneColor.color = defaultColors[colors[0]];
		pTwoColor.color = defaultColors[colors[1]];
	}

	// When the game starts, setup room and start
	// the room settings.
	public void StartGameSetup ()
	{
		GameObject info = GameObject.FindGameObjectWithTag ("SubMenuInfo");
		if (info){
			TextMeshProUGUI text = info.GetComponent<TextMeshProUGUI> ();
			text.text = "Game active.";
		}
		subMenu.SetActive (false);
	}
}
