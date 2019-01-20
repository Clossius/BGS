using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class RaycastScript : MonoBehaviour {

    public Camera cam;
    public GameObject stoneManager;
	public GameObject gameMenu;

	bool playingMove = false;
	
	// Update is called once per frame
	void Update () {

        // If left mouse button is clicked.
		if (Input.GetMouseButtonDown(0))
        {
			// Check if the game is active
			bool activeGame = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>().CanPlayMove();

			// Check if it is our turn.

			if(activeGame && !playingMove)
			{
	            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
	            RaycastHit hit;

	            if (Physics.Raycast(ray, out hit))
	            {
	                Vector3 pos = hit.point;
					string coordinate = GameObject.Find ("_GobanManager")
						.GetComponent<CoordinateManager> ().ClickToCoordinate (pos);
					bool moveExist = GameObject.Find ("_StoneManager")
						.GetComponent<StoneManagerScript> ().CheckMoveExist (coordinate);

					if(coordinate != "" && !moveExist)
					{
						ChangePlayingMove (true);
						GameObject.FindGameObjectWithTag ("RoomManager").GetComponent<RoomManager>().PlayMove(coordinate);
					}
	            }
			}
        }
	}

	public void ChangePlayingMove (bool condition)
	{
		playingMove = condition;
	}
}
