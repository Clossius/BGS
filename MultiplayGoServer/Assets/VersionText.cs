using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class VersionText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		SetVersion ();
    }

	public void SetVersion ()
	{
		this.GetComponent<TextMeshProUGUI> ().text = PhotonNetwork.AppVersion;
	}
}
