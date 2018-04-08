using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour{

	 void OnTriggerEnter(Collider col)
	{
		PlayerController player = col.GetComponent<PlayerController>();
		if (player && !player.IsDead)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene( "InfinityRoad" );
		}
	}
}
