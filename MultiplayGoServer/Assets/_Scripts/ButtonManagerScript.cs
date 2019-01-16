using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


public class ButtonManagerScript : MonoBehaviour {
	
	//**********************************************************************
	// This script is for managing the actions of buttons when pressed.
	//**********************************************************************

	public GameObject errorMessage;

	// Guest Button.
	// Logs the user in as a guest.
	// Changes the menu to Lobby Menu
	public void GuestButton ()
	{
		errorMessage.SetActive (false);
		//Create a guest username
		int num = this.GetComponent<RandomScript>().RandInt(1000, 10000);
		PhotonNetwork.NickName = "Guest#" + num.ToString();
		Debug.Log ("Username: " + PhotonNetwork.NickName);
		//TODO: check for duplicate username
		// Change the scene to the menu.
		SceneManager.LoadScene ("MenuScene");

		//Connect to the network.
		GameObject.Find ("_Network").GetComponent<NetworkManager>().JoinLobby();


	}

	// Login Button
	public void LoginButton (GameObject go)
	{
		TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI> ();

		if (text.text.Length > 4 && text.text.Length < 10) {
			//Set the username
			PhotonNetwork.NickName = text.text;
			Debug.Log ("Username: " + PhotonNetwork.NickName);
			//Make sure the error message is false.
			errorMessage.SetActive (false);

			// Change the scene to the menu.
			SceneManager.LoadScene ("MenuScene");

			//Connect to the network.
			GameObject.Find ("_Network").GetComponent<NetworkManager>().JoinLobby();



		} else {
			errorMessage.SetActive (true);
		}
	}

	// Opens the Game menu and board.
	// Sets up room with given conditions.
	public void PlayButton ()
	{
		SceneManager.LoadScene ("Game");

		GameObject.Find ("_Network").GetComponent<NetworkManager> ().JoinCustomRandomRoom();

	}

	// Leaves the game room and returns to the game menu.
	public void LeaveRoomButton ()
	{
		// Leave the room the client is currently in and
		// join the lobby.
		// Change the scene to the menu.
		GameObject.Find("_Network").GetComponent<NetworkManager>().LeaveCurrentRoom();

	}

	// Resign the game you are currently playing.
	public void ResignButton ()
	{
		Debug.Log ("Resign");
		GameObject.FindGameObjectWithTag ("RoomManager").GetComponent<RoomManager> ().GiveUp ();
	}
}
