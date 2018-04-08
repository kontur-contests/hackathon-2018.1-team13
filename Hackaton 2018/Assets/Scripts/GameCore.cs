using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCore : MonoBehaviour
{
	public static GameCore instance { private set; get; }
	public Jukebox jukebox;

    private bool m_aggrsive = false;
    public bool aggrsive
    {
        get
        {
            return m_aggrsive;
        }
        set
        {
            m_aggrsive = value;
            if (value)
                jukebox.PlayCombat();
            else
                jukebox.PlayIdle();
        }
    }

    

    private void Awake()
	{
		if (instance != null)
			Destroy(this.gameObject);
		else
		{
			// DontDestroyOnLoad(gameObject);
			instance = this;
		}
	}

	private void Start()
	{
		jukebox = GetComponentInChildren<Jukebox>();
		jukebox.PlayIdle();
	}
}
