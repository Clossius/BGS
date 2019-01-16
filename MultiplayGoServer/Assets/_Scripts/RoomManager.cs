using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class RoomPropertyKeys 
{
	public string boardSize = "bs";
	public string numPlayers = "np";
	public string currentTurn = "ct";
	public string ruleSet = "rs";
	public string timeSet = "ts";
	public string activeGame = "ag";
	public string playerOneName = "pon";
	public string playerTwoName = "ptn";
	public string playerOneTime = "pot";
	public string playerTwoTime = "ptt";
	public string isVisible = "iv";
	public string isOpen = "io";
}

public class RoomManager : MonoBehaviourPunCallbacks {

	/// <summary>
	/// Called when the local user/client left a room, so the game's logic can clean up it's internal state.
	/// </summary>
	/// <remarks>When leaving a room, the LoadBalancingClient will disconnect the Game Server and connect to the Master Server.
	/// This wraps up multiple internal actions.
	/// 
	/// Wait for the callback OnConnectedToMaster, before you use lobbies and join or create rooms.</remarks>

	ExitGames.Client.Photon.Hashtable hash;
	RoomPropertyKeys rpk;
	PhotonView pv;
	GameObject stoneManger;
	List<int> playerColors;
	List<string> players;

	// Initialization
	void Start ()
	{
		hash = new ExitGames.Client.Photon.Hashtable ();
		rpk = new RoomPropertyKeys();
		pv = this.GetComponent<PhotonView> ();
		stoneManger = GameObject.Find ("_StoneManager");
		playerColors = new List<int> ();
	}
		
	// Initialize the room settings.
	public void InitializeRoomSettings ()
	{
		Start ();

		if(!PhotonNetwork.IsMasterClient)
		{
			Debug.Log ("Not the master client.");

			if (!PhotonNetwork.IsConnected)
			{
				Debug.Log ("Client not connected.");
				return;
			}

			hash = PhotonNetwork.CurrentRoom.CustomProperties;

			if(hash == null){ Debug.Log ("No Custom Properties found in current room."); }

			return;
		}
			

		hash.Add (rpk.boardSize, 9);
		hash.Add (rpk.currentTurn, 0);
		hash.Add (rpk.numPlayers, 2);
		hash.Add (rpk.ruleSet, 0);
		hash.Add (rpk.timeSet, 0);
		hash.Add (rpk.activeGame, false);
		hash.Add (rpk.playerOneName, "");
		hash.Add (rpk.playerTwoName, "");
		hash.Add (rpk.isOpen, true);
		hash.Add (rpk.isVisible, true);

		string[] lobbyProperties = new string[] {
			rpk.boardSize,
			rpk.numPlayers,
			rpk.ruleSet,
			rpk.activeGame,
			rpk.timeSet,
			rpk.playerOneName,
			rpk.playerTwoName,
			rpk.isOpen,
			rpk.isVisible
		};

		PhotonNetwork.CurrentRoom.SetPropertiesListedInLobby (lobbyProperties);

		string username = PhotonNetwork.NickName;
		hash [rpk.playerOneName] = username;

		PhotonNetwork.CurrentRoom.SetCustomProperties (hash, null, null);

		GameObject.Find ("GameMenu").GetComponent<SubMenu> ()
			.UpdatePlayers ((string)PhotonNetwork.CurrentRoom.CustomProperties[rpk.playerOneName]
				, (string)PhotonNetwork.CurrentRoom.CustomProperties[rpk.playerTwoName], 0, 0);

		GameObject.Find ("GameMenu").GetComponent<SubMenu> ().JoinRoom (PhotonNetwork.CurrentRoom.Name);
	}

	// Player joined
	public void OnPlayerJoined (string username)
	{
		Debug.Log ("Player Joined: " + username);

		if(!PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected){Start (); return;}

		hash = PhotonNetwork.CurrentRoom.CustomProperties;
		bool gameActive = (bool)hash[rpk.activeGame];

		if (!gameActive)
		{
			hash [rpk.playerTwoName] = username;

			if(PhotonNetwork.NickName == username){Debug.Log ("ERROR: Duplicate Usernames");}

			hash [rpk.activeGame] = true;
			hash [rpk.isVisible] = false;

			PhotonNetwork.CurrentRoom.SetCustomProperties (hash, null, null);

			SetPlayerColors (2); // TODO: Set per room settings in the future for more than 2 players.
			ResetCurrentTurn(0); // 0 is black. Change to default first color in the future.

			players = new List<string>();
			players.Add ((string)hash[rpk.playerOneName]);
			players.Add ((string)hash[rpk.playerTwoName]);

			pv.RPC ("GameRoomSetup", RpcTarget.All, (string)hash[rpk.playerOneName], (string)hash[rpk.playerTwoName], playerColors[0], playerColors[1]);
		}
	}

	// Set the colors of the players in the room.
	private void SetPlayerColors (int maxPlayers)
	{
		playerColors = GameObject.Find ("_Scripts").GetComponent<RandomScript>().RandomIntStartList(0, maxPlayers);

	}

	// Set current turn to the given color
	private void ResetCurrentTurn (int color)
	{
		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		for(int i=0; i<playerColors.Count;i++)
		{
			if(playerColors[i] == color){hash [rpk.currentTurn] = i;}
		}

		PhotonNetwork.CurrentRoom.SetCustomProperties (hash, null, null);
	}

	// Call all the clients to set up the room
	// settings in the sub menu.
	[PunRPC]
	private void GameRoomSetup (string pOne, string pTwo, int pOneColor, int pTwoColor)
	{
		GameObject gameMenu = GameObject.Find ("GameMenu");

		gameMenu.GetComponent<SubMenu> ().UpdatePlayers (pOne, pTwo, pOneColor, pTwoColor);
		gameMenu.GetComponent<SubMenu> ().StartGameSetup ();

		if (PhotonNetwork.NickName == pOne || PhotonNetwork.NickName == pTwo) 
		{
			gameMenu.GetComponent<GameButtonManager> ().GameStart ();
		}
	}

	// Get if the current player can play a move
	public bool CanPlayMove ()
	{
		bool canPlay = false;

		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		if((bool)hash[rpk.activeGame])
		{
			canPlay = true;
		}

		return canPlay;
	}

	// Call the RPC play move funtion.
	public void PlayMove (string move)
	{
		pv.RPC ("PlayMoveOverNetwork", RpcTarget.All, move, (string)PhotonNetwork.NickName);
	}

	// Add a move over the network and then call
	// the place stone funtion on all clients.
	[PunRPC]
	private void PlayMoveOverNetwork (string move, string username)
	{
		if (!PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected) 
		{
			Debug.Log ("Not Master client.");
			return;
		}

		// TODO: Add a way to load and save moves on the server.


		if(username == players[(int)hash[rpk.currentTurn]])
		{
			// Check for legal move.
			hash = PhotonNetwork.CurrentRoom.CustomProperties;

			stoneManger.GetComponent<StoneManagerScript> ().CreateStone (move, playerColors[(int)hash[rpk.currentTurn]]);
			List<Stone> stones = stoneManger.GetComponent<StoneManagerScript> ().GetStones ();
			bool legal = CheckForSuicideMove (move, stones, (int)hash[rpk.boardSize]);
			Debug.Log ("Legal move: " + legal.ToString());
			stoneManger.GetComponent<StoneManagerScript> ().RemoveMove (move);

			if (legal)
			{
				pv.RPC ("PlayMovesOnAllClients", RpcTarget.All, move, playerColors[(int)hash[rpk.currentTurn]]);

			}
		}
	}

	// Check if the move played has any liberties. If not,
	// return false unless capturing something.
	private bool CheckForSuicideMove (string move, List<Stone> stones, int boardSize)
	{
		bool legal = true;

		int liberties = stoneManger.GetComponent<LibertyManager> ().GetLiberties (move, stones, boardSize);
		bool capturing = stoneManger.GetComponent<LibertyManager> ().CheckForCapturing (move, stones, boardSize);

		if(liberties == 0 && !capturing){legal = false;}

		return legal;
	}

	[PunRPC]
	private void PlayMovesOnAllClients (string move, int color)
	{
		//TODO: Set up a way to load the moves from the server
		// in case of disconnection and rejoining or for viewers who join
		// late.
		stoneManger.GetComponent<StoneManagerScript> ().CreateStone (move, color);

		if (!PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected){ return;}

		List<Stone> stones = GameObject.Find ("_StoneManager").GetComponent<StoneManagerScript>().GetStones();

		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		bool capture = CheckForCaptures (stones, (int)hash[rpk.boardSize]);

		// Check for win condition.
		if (capture)
		{
			Debug.Log ("Something Captured!");
			// TODO: Check if the rule set is capture Go or not.

			hash = PhotonNetwork.CurrentRoom.CustomProperties;

			for (int i=0; i<playerColors.Count; i++)
			{
				if ((int)hash[rpk.currentTurn] != i)
				{
					pv.RPC ("NetworkRemoveCapture", RpcTarget.All, playerColors[i], (int)hash[rpk.boardSize]);
				}
			}
				
		}

		CheckWincondition ((int)hash[rpk.ruleSet], capture);
		ChangeTurn ();
	}

	// Check for win condition.
	private void CheckWincondition (int ruleSet, bool captured)
	{
		if (!PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected){ return;}

		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		if (ruleSet == 0 && captured) // Capture Go
		{
			string winner = players[(int)hash[rpk.currentTurn]];
			GameOver (winner, "Capture");
		}
	}

	// Adjust the sub menu for Game Over settings.
	private void GameOver (string winner, string wonBy)
	{
		if (!PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected){ return;}

		hash = PhotonNetwork.CurrentRoom.CustomProperties;
		hash [rpk.activeGame] = false;
		PhotonNetwork.CurrentRoom.SetCustomProperties (hash, null, null);

		pv.RPC ("NetworkGameOver", RpcTarget.All, winner, wonBy);

	}

	[PunRPC]
	public void NetworkGameOver (string winner, string wonBy)
	{
		GameObject gameMenu = GameObject.Find ("GameMenu");
		gameMenu.GetComponent<SubMenu> ().GameOver (winner, wonBy);
		gameMenu.GetComponent<GameButtonManager> ().ResetToDefault ();
	}

	// Remove the stones of the given color with 0 liberties.
	[PunRPC]
	public void NetworkRemoveCapture (int color, int boardSize)
	{
		stoneManger.GetComponent<StoneManagerScript> ().RemoveCaptureColor (color, boardSize);
	}

	// Change the current turn in the room if the game is active.
	private void ChangeTurn ()
	{
		if (!PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected) 
		{
			Debug.Log ("Not Master client.");
			return;
		}
			
		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		if ((bool)hash[rpk.activeGame])
		{
			int currentTurn = (int)hash [rpk.currentTurn];
			currentTurn++;

			if(currentTurn >= players.Count){ currentTurn = 0;}

			hash [rpk.currentTurn] = currentTurn;
			PhotonNetwork.CurrentRoom.SetCustomProperties (hash, null, null);
		}
	}

	// Check if anything has been captured.
	// Returns true if any stone has 0 liberties.
	private bool CheckForCaptures (List<Stone> stones, int boardSize)
	{
		bool capture = false;

		for (int i=0; i<stones.Count; i++)
		{
			int liberties = GameObject.Find ("_StoneManager").GetComponent<LibertyManager> ().GetLiberties (stones[i].coordinate, stones, boardSize);

			Debug.Log (stones[i].coordinate + " : " + liberties.ToString());

			if (liberties == 0){ capture = true; }
		}

		return capture;
	}

	// Makes the current turn player Give Up
	public void GiveUp ()
	{
		pv.RPC ("PlayerGaveUp", RpcTarget.All, PhotonNetwork.NickName);
	}

	[PunRPC]
	private void PlayerGaveUp (string username)
	{
		if(!PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected){return;}

		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		List<string> currentPlayers = players; //TODO: Get players from the Network.

		for (int i=0; i<currentPlayers.Count; i++)
		{
			if (username == currentPlayers[i]){currentPlayers.RemoveAt (i); i = 0;}
		}

		if(currentPlayers.Count == 1)
		{
			GameOver (currentPlayers[0], "Resignation");
		}
	}

	public void LeavingRoom ()
	{
		if(!PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected){return;}

		hash = PhotonNetwork.CurrentRoom.CustomProperties;
		hash[rpk.isOpen] = false;
		hash[rpk.isVisible] = false;
		PhotonNetwork.CurrentRoom.SetCustomProperties(hash, null, null);
	}

	public void CallMasterClientSwitched ()
	{
		pv.RPC ("MasterClientChanged", RpcTarget.All);
	}

	// Gets called when the master client switches.
	[PunRPC]
	public void MasterClientChanged ()
	{
		if (!PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected){return;}
		// Check to see if the players are still connected.
		hash = PhotonNetwork.CurrentRoom.CustomProperties;
		if((bool)hash[rpk.activeGame])
		{
			//TODO: Fix this when the players are kept track of over the network.
			GameOver(PhotonNetwork.NickName, "Resignation");
		}
	}
}
