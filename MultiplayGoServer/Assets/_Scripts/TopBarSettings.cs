using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TopBarSettings : MonoBehaviour
{
	public TextMeshProUGUI usersOnlineText;

    // Start is called before the first frame update
    void Start()
    {
		SetOnlineUsersText (1);
    }

	public void SetOnlineUsersText (int num)
	{
		usersOnlineText.text = "Users: " + num.ToString();
	}
}
