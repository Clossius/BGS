using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButtonScript : MonoBehaviour
{
	[Header("Images")]
	public Sprite onIcon;
	public Sprite offIcon;

	[Header("GameObjects")]
	public GameObject image;
	private GameObject soundManager;

	private string globalSoundKey = "gsk";

    // Start is called before the first frame update
    void Start()
    {
		soundManager = GameObject.Find ("SoundManager");
		LoadSoundSettings ();
		ChangeImage ();

    }

	private void LoadSoundSettings ()
	{
		bool hasKey = PlayerPrefs.HasKey (globalSoundKey);

		if (!hasKey) 
		{
			PlayerPrefs.SetInt (globalSoundKey, 0);
		}
	}

	private void ChangeImage ()
	{
		if (PlayerPrefs.GetInt(globalSoundKey) == 0)
		{
			image.GetComponent<Image> ().sprite = onIcon;
			soundManager.GetComponent<SoundManagerScript> ().SetGlobalSound (true);
		} else if (PlayerPrefs.GetInt(globalSoundKey) == 1)
		{
			image.GetComponent<Image> ().sprite = offIcon;
			soundManager.GetComponent<SoundManagerScript> ().SetGlobalSound (false);
		}
	}

	public void ChangeSettings ()
	{
		if (PlayerPrefs.GetInt (globalSoundKey) == 1) {
			PlayerPrefs.SetInt (globalSoundKey, 0);
		} else {
			PlayerPrefs.SetInt (globalSoundKey, 1);
		}

		ChangeImage ();
	}
}
