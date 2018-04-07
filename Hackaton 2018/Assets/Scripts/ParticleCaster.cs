using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCaster : MonoBehaviour
{
	private static ParticleCaster instance;

	public GameObject particle;
	public enum ParticleType {
		dust, 
		blood,
		wood, 
		sand,
		metal,
		stone,
		glass
	}
	private Color[] color = { 
		Color.grey, 
		Color.red, 
		new Color(0.5f, 0.3f, 0.1f), 
		new Color(0.9f, 0.7f, 0.6f),
		Color.yellow,
		Color.yellow,
		Color.white
	};
	public AudioClip[] sound;

	private void Awake()
	{
		instance = this;
	}

	public static void Cast(Vector3 position, Vector3 normal, ParticleType type = ParticleType.dust)
	{
		GameObject go = Instantiate(instance.particle);
		go.transform.position = position;
		go.transform.LookAt(position + normal);
		go.GetComponent<Renderer>().material.color = instance.color[(int)type];
		go.GetComponent<ParticleScript>().Play(instance.sound[(int)type]);
	}
}
