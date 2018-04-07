﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Katana : AWeapon
{
	private void Update()
	{
		if (PlayerController.input.b_LMB)
		{
			if (!inAction && drawn)
			{
				StartCoroutine(Primary());
			}
		}

		if (PlayerController.input.b_RMB)
		{
			if (!inAction)
				drawn = true;
		}
		else
			if (!inAction)
				drawn = false;

		PlayerController.instance.stateLook = drawn ? LookState.aim : LookState.free;
		animator.SetBool("draw", drawn);
	}

	float damage = 0.34f;
	float cooldown = 0.5f;
	float distance = 0.85f;

	private IEnumerator Primary()
	{
		inAction = true;

		Vector3 origin = PlayerController.instance.cameraController.transform.position;
		Vector3 direction = PlayerController.instance.cameraController.transform.forward;
		
		Ray ray = new Ray(origin, direction);

		Debug.DrawRay(origin, direction, Color.red, 2f);

		animator.SetTrigger("fire");
		//lineRend.enabled = true;
		//lineRend.SetPosition(0, lineRend.transform.position);
		//lineRend.SetPosition(1, ray.origin + ray.direction);
		Debug.Log("fistfuck");

		RaycastHit[] hitInfo = Physics.RaycastAll(ray);
		hitInfo = hitInfo.OrderBy((x) => x.distance).ToArray();
		foreach (RaycastHit hit in hitInfo)
		{
			if (hit.distance > distance)
				break;

			IHitable hitable = hit.transform.GetComponent<IHitable>();
			if (hitable != null && (Object)hitable != PlayerController.instance)
			{
				AttackInfo aInfo = new AttackInfo(damage, direction, hit.point, hit.normal);
				hitable.OnHit(aInfo);
				if (aInfo.blocked)
					break;
			}
		}

		yield return new WaitForSeconds(cooldown);
		//lineRend.enabled = false;
		inAction = false;
	}
}
