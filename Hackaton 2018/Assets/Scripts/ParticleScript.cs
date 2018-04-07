using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
	private ParticleSystem shuriken;
	private AudioSource audioSource;

	private void Awake()
	{
		shuriken = GetComponent<ParticleSystem>();
		audioSource = GetComponent<AudioSource>();
		shuriken.Emit(5);
		StartCoroutine(WaitDestroy());
	}

	public void Play(AudioClip sound)
	{
		if (sound == null)
			return;

		audioSource.clip = sound;
		audioSource.Play();
	}

	private IEnumerator WaitDestroy()
	{
		while (true)
		{	
			if (shuriken.particleCount == 0 && !audioSource.isPlaying)
				Destroy(gameObject);
			yield return null;
		}
	}
}
