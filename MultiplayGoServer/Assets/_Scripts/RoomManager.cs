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

	// Initialization
	void Start ()
	{
		hash = new ExitGames.Client.Photon.Hashtable ();
		rpk = new RoomPropertyKeys();
		pv = this.GetComponent<PhotonView> ();
		stoneManger = GameObject.Find ("_StoneManager");
	}

	public override void OnLeftRoom ()
	{
		Debug.Log ("Left room.");
		GameObject.Find ("_Scripts").GetComponent<MenuManager> ().ChangeMenu (1);
	}

	public void LeaveRoom ()
	{
		PhotonNetwork.LeaveRoom ();
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


		string username = PhotonNetwork.NickName;
		hash [rpk.playerOneName] = username;

		PhotonNetwork.CurrentRoom.SetCustomProperties (hash, null, null);

		GameObject.Find ("GameMenu").GetComponent<SubMenu> ()
			.UpdatePlayers ((string)PhotonNetwork.CurrentRoom.CustomProperties[rpk.playerOneName]
				, (string)PhotonNetwork.CurrentRoom.CustomProperties[rpk.playerTwoName]);

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

			PhotonNetwork.CurrentRoom.SetCustomProperties (hash, null, null);

			pv.RPC ("SubMenuPlayerSettings", RpcTarget.All, (string)hash[rpk.playerOneName], (string)hash[rpk.playerTwoName]);
		}
	}

	// Call all the clients to set up the players
	// settings in the sub menu.
	[PunRPC]
	private void SubMenuPlayerSettings (string pOne, string pTwo)
	{
		GameObject gameMenu = GameObject.Find ("GameMenu");

		gameMenu.GetComponent<SubMenu> ().UpdatePlayers (pOne, pTwo);
		gameMenu.GetComponent<SubMenu> ().StartGameSetup ();
	}

	// Debug the properties of the room that changed.
	public override void OnRoomPropertiesUpdate (ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		base.OnRoomPropertiesUpdate (propertiesThatChanged);
		Debug.Log (propertiesThatChanged);
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

		pv.RPC ("PlayMovesOnAllClients", RpcTarget.All, move);
	}

	[PunRPC]
	private void PlayMovesOnAllClients (string move)
	{
		//TODO: Set up a way to load the moves from the server
		// in case of disconnection and rejoining or for viewers who join
		// late.
		stoneManger.GetComponent<StoneManagerScript> ().CreateStone (move, 0);
	}
}
