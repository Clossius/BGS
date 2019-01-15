using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class SubMenu : MonoBehaviour {

	private TextMeshProUGUI title;
	private TextMeshProUGUI info;
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

		title = GameObject.FindGameObjectWithTag ("SubMenuTitle").GetComponent<TextMeshProUGUI>();
		info = GameObject.FindGameObjectWithTag ("SubMenuInfo").GetComponent<TextMeshProUGUI>();

		title.text = "Capture Go";
		info.text = "Waiting for game to start.";

		GameObject.Find ("GameMenu").GetComponent<GameButtonManager> ().ResetToDefault ();
	}

	// Update the player information in a room.
	public void UpdatePlayers (string playerOne, string playerTwo, int pOneColor, int pTwoColor)
	{
		TextMeshProUGUI pOneUser = pOneUsername.GetComponent<TextMeshProUGUI> ();
		TextMeshProUGUI pTwoUser = pTwoUsername.GetComponent<TextMeshProUGUI> ();

		pOneUser.text = playerOne;
		pTwoUser.text = playerTwo;

		// TODO: Adjust the functions below to use only SetPlayerColors in the future.
		SetPlayerColor (0, pOneColor);
		SetPlayerColor (1, pTwoColor);
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

	// TODO: Deprecate this and use SetPlayerColors only in the future.
	public void SetPlayerColor (int player, int color)
	{
		List<Color> defaultColors = new List<Color> ();
		defaultColors.Add (Color.black);
		defaultColors.Add (Color.white);

		List<Image> players = new List<Image> ();
		players.Add (pOneColor);
		players.Add (pTwoColor);

		players [player].color = defaultColors [color];

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

	// Game Over. Display the winner and update the room settings.
	public void GameOver (string winner, string wonBy)
	{
		subMenu.SetActive (true);

		title = GameObject.FindGameObjectWithTag ("SubMenuTitle").GetComponent<TextMeshProUGUI>();
		info = GameObject.FindGameObjectWithTag ("SubMenuInfo").GetComponent<TextMeshProUGUI>();

		title.text = "Game Over!";
		info.text = winner + " is the winner!\nWon by " + wonBy;
	}
}
