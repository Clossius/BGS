using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Game {
	public int boardSize;
	public int numPlayers;
	public int ruleSet;
	public int timeSet;
	public int numColors;
	public string roomID;
	public bool gameActive;

	public Game () {}
}

public class GameSettings : MonoBehaviour {


	public GameObject tmp;
	//TextMeshProUGUI text;

	public Game game;

	// Use this for initialization
	void Start () {
		game = new Game ();
		//text = tmp.GetComponent<TextMeshProUGUI> ();
		SetupDefaultGame ();

		GameObject.Find ("_GobanManager").GetComponent<DrawLineScript> ()
			.MakeGoban ();
		GameObject.Find ("_StoneManager").GetComponent<StoneManagerScript> ()
			.InitializeStones ();
	}

	public void GameUpdate (Game newGame)
	{
		game = newGame;
	}

	// Update settings text.
	void UpdateSettingsText ()
	{
		//text.text = "Board Size: " + game.boardSize.ToString () +
		//"\nPlayers: " + game.numPlayers.ToString ();

		//if (game.ruleSet == 0){ text.text = text.text + "\nRule Set: Capture Go"; }
		//else if (game.ruleSet == 1){ text.text = text.text + "\nRule Set: Territory"; }
		//else if (game.ruleSet == 2){ text.text = text.text + "\nRule Set: One Color Go"; }
		//else if (game.ruleSet == 3){ text.text = text.text + "\nRule Set: Blind Go"; }

		//if (game.timeSet == 0){ text.text = text.text + "\nTime Settings: 1m 10x3"; }
		//else if (game.timeSet == 1){ text.text = text.text + "\nTime Settings: 5m 30x3"; }
		//else if (game.timeSet == 2){ text.text = text.text + "\nTime Settings: 10m 30x5"; }
		//else if (game.timeSet == 3){ text.text = text.text + "\nTime Settings: 20m 30x5"; }
	}

	// Set default room settigns
	public void SetupDefaultGame ()
	{
		game.boardSize = 9;
		game.numPlayers = 2;
		game.ruleSet = 0;
		game.timeSet = 0;
		game.numColors = 2;
		UpdateSettingsText ();
	}

	public void UpdateBoardSize (int size)
	{
		game.boardSize = size;
		UpdateSettingsText ();
	}

	public void UpdateNumPlayers (int num)
	{
		game.numPlayers = num;
		UpdateSettingsText ();
	}

	public void UpdateRuleSet (int num)
	{
		game.ruleSet = num;
		UpdateSettingsText ();
	}

	public void UpdateTimeSet (int num)
	{
		game.timeSet = num;
		UpdateSettingsText ();
	}

	// Set all of the game settings to send to the
	// room.
	public void InitializeGameSettings ()
	{
		if (game.numPlayers == 2)
		{
			game.numColors = 2;
		}

		game.roomID = GetGameRoomName ();
	}

	private string GetGameRoomName ()
	{
		string name = "NoName";

		bool condition = true;
		while (condition)
		{
			condition = false;
			int num = Random.Range (1000, 10000);

			name = num.ToString ();

			GameObject[] gos = GameObject.FindGameObjectsWithTag ("RoomSettings");

			for (int i=0; i<gos.Length;i++)
			{
				//string id = gos [i].GetComponent<RoomSettingsScript> ().roomID;
				//if ( id == num.ToString()){condition = true;}
			}
		}

		if (name == "NoName"){Debug.Log("ERROR: Generating room name.");}

		return name;
	}
}
