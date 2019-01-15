using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonManager : MonoBehaviour
{
	public GameObject playButton;

    // Start is called before the first frame update
    void Start()
    {
		playButton.SetActive (false);   
    }

	public void LoadMenuSettings ()
	{
		playButton.SetActive (true);
	}
}
