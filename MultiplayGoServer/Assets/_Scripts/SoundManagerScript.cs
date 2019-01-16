using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
	// The sound manager script.
	// Plays and manages the sounds in the game.

	[Header("AudioSources")]
	public AudioSource sfx;
	private AudioSource background;

	[Header("Music")]
	public AudioClip backgroundMusic1;
	public AudioClip backgroundMusic2;

	[Header("SFX")]
	public AudioClip buttonClickSound;
	public AudioClip stonePlacementSound;

	void Start ()
	{
		background = this.GetComponent<AudioSource> ();
	}

	public void PlayClickSound ()
	{
		sfx.PlayOneShot(buttonClickSound);
	}

	public void PlayStoneSound ()
	{
		sfx.PlayOneShot (stonePlacementSound);
	}

	public void PlayBackgroundOne ()
	{
		if(background.clip == backgroundMusic1){return;}

		background.Stop ();
		background.clip = backgroundMusic1;
		background.Play ();
		background.loop = true;
	}


	public void PlayBackgroundTwo ()
	{
		if(background.clip == backgroundMusic2){return;}

		background.Stop ();
		background.clip = backgroundMusic2;
		background.Play ();
		background.loop = true;
	}
}
