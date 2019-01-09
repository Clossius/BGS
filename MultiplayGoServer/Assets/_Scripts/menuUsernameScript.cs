using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class menuUsernameScript : MonoBehaviour {

	void Start ()
	{
		TextMeshProUGUI tmp = this.GetComponent<TextMeshProUGUI> ();
		Debug.Log (PhotonNetwork.NickName);
		Debug.Log (tmp.text);
		tmp.text = PhotonNetwork.NickName;
	}
}
