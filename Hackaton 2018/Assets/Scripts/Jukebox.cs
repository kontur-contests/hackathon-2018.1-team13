using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour
{
	private AudioSource audioController;

	// Use this for initialization
	private void Awake ()
	{
		audioController = GetComponent<AudioSource>();
	}

	public AudioClip music_idle;
	public AudioClip music_combat;
	public void PlayIdle()
	{
        audioController.Stop();

        audioController.PlayOneShot( music_idle );
	}
	public void PlayCombat()
	{
        audioController.Stop();
        audioController.PlayOneShot( music_combat );
	}
}
