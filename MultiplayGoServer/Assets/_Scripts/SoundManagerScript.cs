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
	public AudioClip winningSound;

	[Header("Settings")]
	public bool globalSoundActive = true;

	void Start ()
	{
		background = this.GetComponent<AudioSource> ();
	}

	public void SetGlobalSound (bool condition)
	{
		globalSoundActive = condition;
		background = this.GetComponent<AudioSource> ();
		if (condition) {
			background.Play ();
			background.loop = true;
		} else {
			background.Stop ();
		}
	}

	private void PlaySFXSound (AudioClip clip)
	{
		if(globalSoundActive)
		{
			sfx.PlayOneShot (clip);
		}
	}

	private void PlayBackgroundMusic (AudioClip clip)
	{
		if (background.clip == clip){return;}
		else if(globalSoundActive)
		{
			background.Stop ();
			background.clip = clip;
			background.Play ();
			background.loop = true;
		}

	}

	public void PlayClickSound ()
	{
		PlaySFXSound(buttonClickSound);
	}

	public void PlayStoneSound ()
	{
		PlaySFXSound(stonePlacementSound);
	}

	public void PlayBackgroundOne ()
	{
		PlayBackgroundMusic (backgroundMusic1);
	}


	public void PlayBackgroundTwo ()
	{
		PlayBackgroundMusic (backgroundMusic2);
	}

	public void PlayWinningSound ()
	{
		PlaySFXSound (winningSound);
	}
}
