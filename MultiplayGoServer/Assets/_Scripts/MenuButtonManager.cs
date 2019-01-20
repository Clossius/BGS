using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonManager : MonoBehaviour
{
	[Header("Buttons")]
	public GameObject playButton;
	public GameObject vsAIButton;

    // Start is called before the first frame update
    void Start()
    {
		playButton.SetActive (false);
		vsAIButton.SetActive (false);
    }

	public void LoadMenuSettings ()
	{
		playButton.SetActive (true);
		vsAIButton.SetActive (true);
	}
}
