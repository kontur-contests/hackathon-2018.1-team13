using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBoss : MonoBehaviour
{
	public Boss boss;

	void OnTriggerEnter(Collider col)
	{
		PlayerController player = col.GetComponent<PlayerController>();
		if (player && !player.IsDead)
		{
			print("x");
			if (boss)
			{
				print("y");
				boss.Wake();
				boss = null;
			}
		}
	}
}
