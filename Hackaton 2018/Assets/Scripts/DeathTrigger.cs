using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour{

	 void OnTriggerEnter(Collider col)
	{
		PlayerController player = col.GetComponent<PlayerController>();
		if (player && !player.IsDead)
		{
			player.OnHit(new AttackInfo(1000, Vector3.up, Vector3.zero, Vector3.down));
		}
	}
}
