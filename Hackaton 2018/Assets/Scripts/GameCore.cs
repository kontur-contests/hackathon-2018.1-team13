using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCore : MonoBehaviour
{
	public static GameCore instance { private set; get; }

    public bool aggrsive = false;
    
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
