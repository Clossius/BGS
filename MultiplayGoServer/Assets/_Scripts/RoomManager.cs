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
	public string botLevel = "bl";
	public string campaignLevel = "cl";
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
	public List<int> playerColors;
	List<string> players;
	bool botThinking = false;

	//*********************
	// Initialization
	//*********************

	void LoadRoomSettings ()
	{
		hash = new ExitGames.Client.Photon.Hashtable ();
		rpk = new RoomPropertyKeys();
		pv = this.GetComponent<PhotonView> ();
		stoneManger = GameObject.Find ("_StoneManager");
	}


	public void InitializeRoomSettings ()
	{
		LoadRoomSettings ();
		if(!PhotonNetwork.IsMasterClient)
		{
			Debug.Log ("Not the master client.");

			if (!PhotonNetwork.IsConnected)
			{
				Debug.Log ("Client not connected.");
				return;
			}

			hash = PhotonNetwork.CurrentRoom.CustomProperties;

			if(hash == null){ Debug.Log ("ERROR: No Custom Properties found in current room."); }

			return;
		}

		InitializeRoom ();
		InitializeSubMenu ();

		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		if((int)hash[rpk.ruleSet] == 0){InitializeCaptureGo ();}
		else if((int)hash[rpk.ruleSet] == 1){InitializeCaptureGoBotSettings ((int)hash[rpk.botLevel]);}
		else if((int)hash[rpk.ruleSet] == 2){InitializeCampaignSettings ();}
	}

	// Load the settings for the current room.
	private void InitializeRoom ()
	{
		LocalSettings localSettings = GameObject.Find ("LocalRoomSettings").GetComponent<LocalRoomSettings> ().localSettings;

		hash = new ExitGames.Client.Photon.Hashtable ();

		hash.Add (rpk.boardSize, localSettings.boardSize);
		hash.Add (rpk.currentTurn, 0);
		hash.Add (rpk.numPlayers, localSettings.maxPlayers);
		hash.Add (rpk.ruleSet, localSettings.ruleSet);
		hash.Add (rpk.timeSet, 0);
		hash.Add (rpk.activeGame, false);
		hash.Add (rpk.playerOneName, "");
		hash.Add (rpk.playerTwoName, "");
		hash.Add (rpk.isOpen, localSettings.roomJoinable);
		hash.Add (rpk.isVisible, localSettings.roomVisible);
		hash.Add (rpk.botLevel, localSettings.botLevel);
		hash.Add (rpk.campaignLevel, localSettings.campaignLevel);

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

		hash [rpk.playerOneName] = PhotonNetwork.NickName;

		playerColors = new List<int> ();
		players = new List<string> ();
		players.Add ((string)hash[rpk.playerOneName]);
		SetPlayerColors ((int)hash[rpk.numPlayers]);

		PhotonNetwork.CurrentRoom.SetCustomProperties (hash, null, null);

		stoneManger.GetComponent<StoneManagerScript> ().InitializeStones ();
	}

	private void InitializeSubMenu ()
	{
		GameObject gameMenu = GameObject.Find ("GameMenu");
		gameMenu.GetComponent<SubMenu> ()
			.UpdatePlayers ((string)PhotonNetwork.CurrentRoom.CustomProperties[rpk.playerOneName]
				, (string)PhotonNetwork.CurrentRoom.CustomProperties[rpk.playerTwoName], 0, 0);

		hash = PhotonNetwork.CurrentRoom.CustomProperties;
		gameMenu.GetComponent<SubMenu> ().JoinRoom (PhotonNetwork.CurrentRoom.Name, (int)hash[rpk.ruleSet]);
	}



	// Set the colors of the players in the room.
	private void SetPlayerColors (int maxPlayers)
	{
		playerColors = GameObject.Find ("_Scripts").GetComponent<RandomScript>().RandomIntStartList(0, maxPlayers);
	}

	// Set a specific color for a specific player.
	public void SetPlayerColor (int player, int color)
	{
		playerColors [player] = color;

		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		GameRoomSetup ((string)hash[rpk.playerOneName], (string)hash[rpk.playerTwoName], playerColors[0], playerColors[1]);
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



	//*************************
	// Active Game Functions
	//*************************

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

		hash = PhotonNetwork.CurrentRoom.CustomProperties;
		if ((bool)hash[rpk.activeGame])
		{
			GameObject.Find ("SoundManager").GetComponent<SoundManagerScript> ().PlayBackgroundTwo ();
			GameObject.Find ("GameMenu").GetComponent<SubMenu> ().ChangeSubMenuActive (false);
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

		Debug.Log("Playing move.");
		Debug.Log (username + " "  + players[(int)hash[rpk.currentTurn]]);

		if(username == players[(int)hash[rpk.currentTurn]])
		{
			// Check for legal move.
			hash = PhotonNetwork.CurrentRoom.CustomProperties;

			stoneManger.GetComponent<StoneManagerScript> ().CreateStone (move, playerColors[(int)hash[rpk.currentTurn]]);
			List<Stone> stones = stoneManger.GetComponent<StoneManagerScript> ().GetStones ();
			bool legal = CheckForSuicideMove (move, stones, (int)hash[rpk.boardSize]);
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
		stoneManger = GameObject.Find ("_StoneManager");
		stoneManger.GetComponent<StoneManagerScript> ().CreateStone (move, color);
		stoneManger.GetComponent<RaycastScript> ().ChangePlayingMove (false);

		if (!PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected){ return;}

		List<Stone> stones = GameObject.Find ("_StoneManager").GetComponent<StoneManagerScript>().GetStones();

		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		bool capture = CheckForCaptures (stones, (int)hash[rpk.boardSize]);

		/* This is for Debuging Liberties.
		 * 
		for (int i=0; i<stones.Count; i++)
		{
			List<string> liberties = GameObject.Find ("_StoneManager").GetComponent<LibertyManager> ()
				.GetLibertyCoordinates (stones[i].coordinate, stones, (int)hash[rpk.boardSize]);

			string log = stones [i].coordinate + ": " + liberties.Count.ToString();
			for (int g=0; g<liberties.Count; g++)
			{
				log = log + " " + liberties [g];
			}
			Debug.Log (log);
		}*/

		// Check for win condition.
		if (capture)
		{
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

		if (ruleSet == 1 || ruleSet == 2){ ruleSet = 0;} // Playing Bot in Capture Go

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
		GameObject.Find ("SoundManager").GetComponent<SoundManagerScript>().PlayWinningSound();
		GameObject gameMenu = GameObject.Find ("GameMenu");
		gameMenu.GetComponent<SubMenu> ().GameOver (winner, wonBy);
		gameMenu.GetComponent<GameButtonManager> ().ResetToDefault ();

		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		if ((int)hash[rpk.ruleSet] == 2)
		{
			GameOverForCampaign (winner);
		}
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

			if((int)hash[rpk.ruleSet] == 1) // Playing bot.
			{
				GetBotMove ((int)hash[rpk.botLevel]);
			} else if ((int)hash[rpk.ruleSet] == 2) // Playing tutorial
			{
				GetTutorialMove ();
			}
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


	//************************************
	// Start a One on One Capture Go
	//************************************

	private void InitializeCaptureGo ()
	{
		if (!PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected){return;}

		GameObject.Find ("LocalRoomSettings").GetComponent<LocalRoomSettings> ().LoadCaptureGoSettings();
		InitializeRoom ();
	}

	private void StartCaptureGo ()
	{
		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		hash [rpk.activeGame] = true;
		hash [rpk.isVisible] = false;

		SetPlayerColors (2); // TODO: Set per room settings in the future for more than 2 players.
		ResetCurrentTurn(0); // 0 is black. Change to default first color in the future.

		players.Add ((string)hash[rpk.playerOneName]);
		players.Add ((string)hash[rpk.playerTwoName]);

		pv.RPC ("GameRoomSetup", RpcTarget.All, (string)hash[rpk.playerOneName], (string)hash[rpk.playerTwoName], playerColors[0], playerColors[1]);


	}

	// Player joined
	public void OnPlayerJoined (string username)
	{
		if(!PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected){LoadRoomSettings (); return;}

		hash = PhotonNetwork.CurrentRoom.CustomProperties;
		bool gameActive = (bool)hash[rpk.activeGame];

		if (!gameActive)
		{
			hash [rpk.playerTwoName] = username;

			if(PhotonNetwork.NickName == username){Debug.Log ("ERROR: Duplicate Usernames"); return;}

			PhotonNetwork.CurrentRoom.SetCustomProperties (hash, null, null);

			StartCaptureGo ();
		}
	}

	//**************************
	// Bot Functions
	//**************************

	private void InitializeCaptureGoBotSettings (int botLevel)
	{
		GameObject.Find ("LocalRoomSettings").GetComponent<LocalRoomSettings> ().LoadBotSettings (botLevel);
		InitializeRoom ();

		string botName = GameObject.Find ("BotManager").GetComponent<AIScript> ().GetBotName (botLevel);
		players = new List<string> ();
		players.Add (PhotonNetwork.NickName);
		players.Add (botName);

		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		hash [rpk.playerTwoName] = botName;
		hash [rpk.activeGame] = true;
		hash [rpk.botLevel] = botLevel;

		PhotonNetwork.CurrentRoom.SetCustomProperties (hash, null, null);

		GameRoomSetup (players [0], players [1], playerColors [0], playerColors [1]);

		stoneManger.GetComponent<StoneManagerScript> ().InitializeStones ();

		InitializeSubMenu ();

		StartGameForBot(botLevel);
	}

	public void StartGameForBot (int botLevel)
	{
		ResetCurrentTurn(0);

		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		GameObject.Find ("SoundManager").GetComponent<SoundManagerScript> ().PlayBackgroundTwo ();
		GameObject.Find ("GameMenu").GetComponent<SubMenu> ().ChangeSubMenuActive (false);
		GameObject.Find ("GameMenu").GetComponent<GameButtonManager> ().GameStart ();

		if (players[(int)hash[rpk.currentTurn]] != PhotonNetwork.NickName)
		{
			GetBotMove ((int)hash[rpk.botLevel]);
		}
	}

	// Wait for move to load before playing bot move.
	private void GetBotMove (int botLevel)
	{
		if (!botThinking)
		{
			StartCoroutine ("WaitForSeconds");
		}
	}

	private void BotMove ()
	{
		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		if (players[(int)hash[rpk.currentTurn]] == PhotonNetwork.NickName){ Debug.Log ("Not Bots turn."); return;}

		GameObject botManager = GameObject.Find ("BotManager");

		string move = botManager.GetComponent<AIScript> ()
			.PlayAMove ((int)hash[rpk.boardSize], playerColors[(int)hash[rpk.currentTurn]], (int)hash[rpk.botLevel]);

		Debug.Log ("Bot Move: " + move);

		string botName = botManager.GetComponent<AIScript> ().GetBotName ((int)hash[rpk.botLevel]);
		PlayMoveOverNetwork (move, botName);

	}

	IEnumerator WaitForSeconds ()
	{
		botThinking = true;
		yield return new WaitForSeconds (0.1f);
		BotMove ();
		botThinking = false;
	}

	//********************
	// Campaign
	//********************

	private void InitializeCampaignSettings ()
	{
		Debug.Log ("Loading Campaign Settings.");
		GameObject gameMenu = GameObject.Find ("GameMenu");

		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		gameMenu.GetComponent<TutorialScript> ().LoadCampaign ((int)hash[rpk.campaignLevel]);


	}

	public void StartGameForCampaign (string botName, int stoneSettings, int botLevel)
	{
		InitializeRoom ();

		players = new List<string> ();
		players.Add (PhotonNetwork.NickName);
		players.Add (botName);

		playerColors = new List<int> ();
		playerColors.Add (0);
		playerColors.Add (1);

		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		hash [rpk.currentTurn] = 0;
		hash [rpk.playerTwoName] = botName;
		hash [rpk.activeGame] = true;
		hash [rpk.botLevel] = botLevel;

		PhotonNetwork.CurrentRoom.SetCustomProperties (hash, null, null);

		GameRoomSetup (players [0], players [1], playerColors [0], playerColors [1]);
		stoneManger.GetComponent<StoneManagerScript> ().LoadSpecificStoneSet (stoneSettings);

		GameObject.Find ("SoundManager").GetComponent<SoundManagerScript> ().PlayBackgroundTwo ();
		GameObject.Find ("GameMenu").GetComponent<GameButtonManager> ().GameStart ();
	}

	private void GameOverForCampaign (string winner)
	{
		Debug.Log ("Campaign Game Finished.");
		GameObject.Find ("GameMenu").GetComponent<SubMenu> ().ChangeSubMenuActive (false);
		if(winner == PhotonNetwork.NickName){
			GameObject.Find ("GameMenu").GetComponent<TutorialScript>().PlayedGame(true);
		} else {
			GameObject.Find ("GameMenu").GetComponent<TutorialScript>().PlayedGame(false);
		}

		GameObject.Find ("SoundManager").GetComponent<SoundManagerScript> ().PlayBackgroundOne ();
		GameObject.Find ("GameMenu").GetComponent<GameButtonManager> ().ResetToDefault ();
	}

	private void TutorialMove ()
	{
		hash = PhotonNetwork.CurrentRoom.CustomProperties;

		if (players[(int)hash[rpk.currentTurn]] == PhotonNetwork.NickName){ Debug.Log ("Not Bots turn."); return;}

		GameObject botManager = GameObject.Find ("BotManager");

		string move = botManager.GetComponent<AIScript> ()
			.PlayAMove ((int)hash[rpk.boardSize], playerColors[(int)hash[rpk.currentTurn]], (int)hash[rpk.botLevel]);

		Debug.Log ("Bot Move: " + move);

		string name = GameObject.Find ("GameMenu").GetComponent<TutorialScript> ().GetNPCName ();

		PlayMoveOverNetwork (move, name);
	}

	// Wait for move to load before playing bot move.
	private void GetTutorialMove ()
	{
		if (!botThinking)
		{
			StartCoroutine ("TutorialWaitForSeconds");
		}
	}

	IEnumerator TutorialWaitForSeconds ()
	{
		botThinking = true;
		yield return new WaitForSeconds (0.1f);
		TutorialMove ();
		botThinking = false;
	}
}
