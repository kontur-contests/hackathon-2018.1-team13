using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCore : MonoBehaviour
{
	public static GameCore instance;
	
	private void Awake()
	{
		if (instance != null)
			Destroy(this);
		else
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
		}
	}
}
