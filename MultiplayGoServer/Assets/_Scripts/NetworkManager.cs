using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class Room 
{
	public RoomInfo roomInfo;
	public ExitGames.Client.Photon.Hashtable hash;

	public Room (RoomInfo ri, ExitGames.Client.Photon.Hashtable h)
	{
		roomInfo = ri;
		hash = h;
	}
}

public class NetworkManager: MonoBehaviourPunCallbacks {

	/// <summary>
	/// The game version. Clients can only connect
	/// with othe clients using the same game version.
	/// Private so users cannot change the version.
	/// </summary>

	//private Dictionary<string, RoomInfo> m_cachedRoomList;
	public byte maxPlayers = 4;
	List<Room> localRooms;
	RoomPropertyKeys rpk;

	PhotonView pv;


	// ********************************
	// Initizlization
	// ********************************

	private void awake ()
	{
		// Make sure when using PhotonNetwork.LoadLevel() on the 
		// master client and all clients in the same room sync their
		// level automatically.
		PhotonNetwork.AutomaticallySyncScene = true;
		//m_cachedRoomList = new Dictionary<string, RoomInfo> ();
	}

	private void Start ()
	{
		pv = PhotonView.Get(this);

		rpk = new RoomPropertyKeys();
		ConnectToNetwork ();

		PhotonNetwork.Instantiate ("PlayerData", new Vector3 (0.0f, 0.0f, 0.0f),
			Quaternion.Euler(0.0f, 0.0f, 0.0f), 0, null);
	}

	// ********************************
	// Connections
	// ********************************

	// Connect to the network using the settings already applied
	// to the photon objects.
	// Set the game version to use when connecting.
	public void ConnectToNetwork ()
	{
		PhotonNetwork.ConnectUsingSettings ();
	}

	// When you connect to the master server.
	public override void OnConnectedToMaster ()
	{
		Debug.Log ("Connected to Master.");
		Debug.Log ("Region: " + PhotonNetwork.CloudRegion.ToString());
		Debug.Log ("Version: " + PhotonNetwork.AppVersion);

		if(SceneManager.GetActiveScene().name == "LoginScene")
		{
			GameObject.Find ("VersionText").GetComponent<VersionText> ().SetVersion ();
		}

		JoinLobby();

		// If in the Login Scene. Find the scene manager object and call the connected funtion
		// to activate the buttons.
		GameObject loginSceneManager = GameObject.Find ("_SceneObjectManager");
		if (loginSceneManager != null)
		{
			loginSceneManager.GetComponent<LobbySceneManager> ().Connected ();
		}
	}

	// Join the lobby of the server.
	public void JoinLobby ()
	{
		if (PhotonNetwork.IsConnected)
		{
			PhotonNetwork.JoinLobby ();
		}
	}

	// Joined a lobby.
	public override void OnJoinedLobby ()
	{
		Debug.Log ("Joined lobby.");
		GameObject.Find ("SoundManager").GetComponent<SoundManagerScript> ().PlayBackgroundOne ();
		if (PhotonNetwork.NickName != "" && SceneManager.GetActiveScene().name == "MenuScene")
		{
			GameObject.Find ("MenuScreen").GetComponent<MenuButtonManager>().LoadMenuSettings();
			GameObject.Find ("MenuScreen").GetComponent<TopBarSettings> ()
				.SetOnlineUsersText (PhotonNetwork.CountOfPlayers);
		}
			
	}

	// When you disconnect.
	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.LogWarningFormat ("Disconnected.", cause);
	}

	// ********************************
	// Room Creating and Joining
	// ********************************

	// Create a room.
	public void CreateRoom (RoomOptions ro)
	{
		PhotonNetwork.CreateRoom ("Room_" + PhotonNetwork.NickName, ro);
	}

	public override void OnCreateRoomFailed (short returnCode, string message)
	{
		base.OnCreateRoomFailed (returnCode, message);
		Debug.Log ("Failed to create room. Creating unique name room.");

		string roomName = "Room_" + PhotonNetwork.NickName;
		int num = Random.Range (1000, 10000);
		roomName = roomName + num.ToString ();

		RoomOptions ro = new RoomOptions ();

		PhotonNetwork.CreateRoom (roomName, ro);
	}

	// Join a room with a given name.
	public void JoinRoom (string roomName)
	{
		PhotonNetwork.JoinRoom (roomName, null);
	}


	// Join room if found. Otherwise create room.
	public void JoinOrCreateRoom (RoomOptions ro)
	{
		PhotonNetwork.JoinOrCreateRoom ("Room_" + System.Guid.NewGuid().ToString(), ro, TypedLobby.Default);

	}

	// Join random room or create one.
	public void JoinCustomRandomRoom ()
	{
		//Debug.Log ("Rooms found: " + localRooms.Count + "/" + localRooms.Count);
		RoomOptions ro = new RoomOptions ();

		if (localRooms.Count == 0) {
			Debug.Log ("No Rooms found, creating room.");
			CreateRoom (ro);
		} else {
			Debug.Log ("Rooms Found!");
			List<Room> roomsOpen = new List<Room> ();

			for (int i = 0; i < localRooms.Count; i++) {
				ExitGames.Client.Photon.Hashtable hash = localRooms [i].roomInfo.CustomProperties;
				if(!(bool)hash[rpk.activeGame])
				{
					if ((string)hash [rpk.playerOneName] == "" || (string)hash [rpk.playerTwoName] == "") {
						roomsOpen.Add (localRooms[i]);
					}
				}
			}

			if (roomsOpen.Count == 0) {
				Debug.Log ("Creating Room after rooms found.");
				CreateRoom (ro);
			} else {
				Debug.Log ("Joining Room " + roomsOpen[0].roomInfo.Name);
				JoinRoom (roomsOpen[0].roomInfo.Name);
			}
		}
	}

	public override void OnRoomListUpdate (List<RoomInfo> roomList)
	{
		base.OnRoomListUpdate (roomList);

		localRooms = new List<Room> ();

		for (int i=0; i<roomList.Count; i++)
		{
			bool removed = roomList [i].RemovedFromList;
			if (roomList[i].IsVisible && !removed)
			{
				ExitGames.Client.Photon.Hashtable hash = roomList [i].CustomProperties;
				if(hash == null){Debug.Log ("NULL ERROR");}
				if ((bool)hash[rpk.isVisible])
				{
					Room newRoom = new Room (roomList[i], roomList[i].CustomProperties);
					localRooms.Add (newRoom);
				}
			}
		}

		Debug.Log ("Room added. New room count: " + localRooms.Count);
		if (localRooms.Count > 0)
		{
			Debug.Log ("Is Visible: " + localRooms[0].roomInfo.IsVisible.ToString());
			Debug.Log ("Num Players: " + localRooms[0].hash[rpk.numPlayers].ToString());
			Debug.Log ("Room Name: " + (string)localRooms[0].roomInfo.Name);
			Debug.Log ("Active Game: " + localRooms[0].hash[rpk.activeGame].ToString());

		}
	}

	// When a client fails to join a room this function is called.
	public override void OnJoinRoomFailed (short returnCode, string message)
	{
		base.OnJoinRoomFailed (returnCode, message);
		Debug.Log ("Failed to join a room. " + message);
		JoinCustomRandomRoom ();
	}

	// ********************************
	// Room Call Backs
	// ********************************

	// Client joined a room.
	// If client is the master client, load the room settings.
	// If clinet is not master client,
	// call all clients in the room to execute the ClientJoined function.
	public override void OnJoinedRoom ()
	{
		Debug.Log ("Client joined room.");

		if (PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected) {
			Debug.Log ("Client is master client.");

			PhotonNetwork.Instantiate ("RoomManager", new Vector3 (0.0f, 0.0f, 0.0f),
				Quaternion.Euler (0.0f, 0.0f, 0.0f), 0, null);

			GameObject.FindGameObjectWithTag ("RoomManager").GetComponent<RoomManager> ().InitializeRoomSettings ();
		} else {
			Debug.Log ("Client is not Master Client.");
			pv.RPC ("ClientJoined", RpcTarget.All, (string)PhotonNetwork.NickName);
		}
			

	}

	// Execute on all clients in the room.
	// If the client is the master client and is connected,
	// call the room manager OnPlayerJoined function.
	[PunRPC]
	void ClientJoined (string username)
	{
		Debug.Log ("RPC Call Client: " + username + " joined room.");

		if (PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected) {

			GameObject roomManager = GameObject.FindGameObjectWithTag ("RoomManager");
			roomManager.GetComponent<RoomManager> ().OnPlayerJoined (username);

		}
	}

	// Leave the current room the client is in and call OnLeftRoom.
	public void LeaveCurrentRoom ()
	{
		if(PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
		{
			Debug.Log ("Making room false.");
			Debug.Log (PhotonNetwork.CurrentRoom.IsVisible);
			GameObject.FindGameObjectWithTag ("RoomManager").GetComponent<RoomManager>().LeavingRoom();

		}

		PhotonNetwork.LeaveRoom ();

	}

	// Gets called when the client leaves a room.
	// Join the lobby.
	public override void OnLeftRoom ()
	{
		base.OnLeftRoom ();
		Debug.Log("Left Room");
		SceneManager.LoadScene ("MenuScene");
		JoinLobby ();
	}

	[PunRPC]
	private void CreateRoomManager ()
	{
		if(!PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected){return;}

		PhotonNetwork.Instantiate ("RoomManager", new Vector3 (0.0f, 0.0f, 0.0f),
			Quaternion.Euler (0.0f, 0.0f, 0.0f), 0, null);

		GameObject.FindGameObjectWithTag ("RoomManager").GetComponent<RoomManager> ().CallMasterClientSwitched ();
	}

	public override void OnMasterClientSwitched (Player newMasterClient)
	{
		base.OnMasterClientSwitched (newMasterClient);

		pv.RPC ("CreateRoomManager", RpcTarget.All);
	}

	public void UpdatePlayersOnline ()
	{
		int numClients = PhotonNetwork.CountOfPlayers;

		GameObject.Find ("MenuScreen").GetComponent<TopBarSettings> ().SetOnlineUsersText (numClients);
	}

	public override void OnLobbyStatisticsUpdate (List<TypedLobbyInfo> lobbyStatistics)
	{
		base.OnLobbyStatisticsUpdate (lobbyStatistics);

		GameObject.Find ("MenuScreen").GetComponent<TopBarSettings> ().
		SetOnlineUsersText (PhotonNetwork.CountOfPlayers);
	}
}
