using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCore : MonoBehaviour
{
	public static GameCore instance { private set; get; }

    
    private void Awake()
	{
		if (instance != null)
			Destroy(this.gameObject);
		else
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
		}
	}
}
