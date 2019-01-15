using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButtonManager : MonoBehaviour
{
	public GameObject leaveButton;
	public GameObject resignButton;

    // Start is called before the first frame update
    void Start()
    {
		leaveButton.SetActive (false);
		resignButton.SetActive (false);
    }

	public void GameStart ()
	{
		leaveButton.SetActive (false);
		resignButton.SetActive (true);
	}

	public void ResetToDefault ()
	{
		leaveButton.SetActive (true);
		resignButton.SetActive (false);
	}
}
