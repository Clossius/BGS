using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager: MonoBehaviourPunCallbacks {

	/// <summary>
	/// The game version. Clients can only connect
	/// with othe clients using the same game version.
	/// Private so users cannot change the version.
	/// </summary>

	private Dictionary<string, RoomInfo> m_cachedRoomList;

	private string gameVersion = "0.0.1"; 
	public byte maxPlayers = 4;
	PhotonView pv;

	private void awake ()
	{
		// Make sure when using PhotonNetwork.LoadLevel() on the 
		// master client and all clients in the same room sync their
		// level automatically.
		PhotonNetwork.AutomaticallySyncScene = true;
		m_cachedRoomList = new Dictionary<string, RoomInfo> ();
	}

	private void Start ()
	{
		//PhotonNetwork.AddCallbackTarget (this);
		pv = PhotonView.Get(this);
		ConnectToNetwork ();
		PhotonNetwork.Instantiate ("PlayerData", new Vector3 (0.0f, 0.0f, 0.0f),
			Quaternion.Euler(0.0f, 0.0f, 0.0f), 0, null);
	}



	/*private void UpdateCachedRoomList (List<RoomInfo> roomList)
	{
		foreach (RoomInfo info in roomList)
		{
			if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
			{
				if (m_cachedRoomList.ContainsKey(info.Name))
				{
					m_cachedRoomList.Remove (info.Name);
				}
			}

			if (m_cachedRoomList.ContainsKey (info.Name)) {
				m_cachedRoomList [info.Name] = info;
			} else {
				m_cachedRoomList.Add (info.Name, info);
			}
		}
	}

	public override void OnRoomListUpdate (List<RoomInfo> roomList)
	{
		base.OnRoomListUpdate (roomList);
		UpdateCachedRoomList (roomList);
	}*/

	// When you connect to the master server.
	public override void OnConnectedToMaster ()
	{
		Debug.Log ("Connected to Master.");
		JoinLobby();

		// If in the Login Scene. Find the scene manager object and call the connected funtion
		// to activate the buttons.
		GameObject loginSceneManager = GameObject.Find ("_SceneObjectManager");
		if (loginSceneManager != null)
		{
			loginSceneManager.GetComponent<LobbySceneManager> ().Connected ();
		}
	}

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
	}

	// When you disconnect.
	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.LogWarningFormat ("Disconnected.", cause);
		//TODO: On disconnected, return to log in menu.
	}

	// Connect to the network using the settings already applied
	// to the photon objects.
	// Set the game version to use when connecting.
	public void ConnectToNetwork ()
	{
		PhotonNetwork.GameVersion = gameVersion;
		PhotonNetwork.ConnectUsingSettings ();
	}

	// Create a room.
	public void CreateRoom (RoomOptions ro)
	{
		PhotonNetwork.CreateRoom ("Room_" + System.Guid.NewGuid().ToString(), ro);
	}


	// Join room if found. Otherwise create room.
	public void JoinOrCreateRoom (RoomOptions ro)
	{
		PhotonNetwork.JoinOrCreateRoom ("Room_" + System.Guid.NewGuid().ToString(), ro, TypedLobby.Default);

	}

	// Join random room or create one.
	public void JoinRandomRoom ()
	{
		int roomCount = PhotonNetwork.CountOfRooms;
		Debug.Log ("Rooms found: " + roomCount);

		if (roomCount == 0) {
			CreateRoom (new RoomOptions ());
		} else {
			Debug.Log ("Room Found!");

			PhotonNetwork.JoinRandomRoom ();

			//RoomInfo[] rooms = PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "");

			//Dictionary<string, RoomInfo>.KeyCollection keys = m_cachedRoomList.Keys;

			//foreach (string key in keys)
			//{
			//	Debug.Log (m_cachedRoomList[key]);
			//}

		}
	}

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


}
