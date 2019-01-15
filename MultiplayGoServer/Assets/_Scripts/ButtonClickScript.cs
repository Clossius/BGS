using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickScript : MonoBehaviour
{
	public void ButtonClicked (string buttonName)
	{
		GameObject scripts = GameObject.Find ("_Scripts");

		if (buttonName == "GameMenuLeaveButton")
		{
			scripts.GetComponent<ButtonManagerScript> ().LeaveRoomButton ();
		}

		else if (buttonName == "GameMenuResignButton")
		{
			scripts.GetComponent<ButtonManagerScript> ().ResignButton ();
		}
	}
}
