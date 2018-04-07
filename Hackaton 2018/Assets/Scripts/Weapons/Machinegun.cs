using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Machinegun : AWeapon
{
	public static int ammo;

	public static int ammo_current = 30;
	private int ammo_max = 30;

	private LineRenderer lineRend;

	protected override void Awake()
	{
		lineRend = GetComponentInChildren<LineRenderer>();
		base.Awake();
	}

	private void Update()
	{
		if (PlayerController.input.b_LMB)
		{
			if (!inAction)
			{
				if (ammo_current > 0)
					StartCoroutine(Primary());
				else
					Debug.Log("out of ammo");
			}
		}

		if (PlayerController.input.b_Reload)
		{
			if (!inAction)
			{
				StartCoroutine(Reload());
			}
		}
	}
	
	float spread = 1.5f;
	float damage = 1f;
	float recoil = 3f;
	float cooldown = 0.15f;

	private IEnumerator Primary()
	{
		inAction = true;

		Vector3 origin = PlayerController.instance.cameraController.transform.position;
		Vector3 direction = PlayerController.instance.cameraController.transform.forward;

		ammo_current--;

		direction = Spread(spread) * direction;
		Ray ray = new Ray(origin, direction);

		Debug.DrawRay(origin, direction, Color.red, 2f);

		animator.SetTrigger("fire");
		lineRend.enabled = true;
		lineRend.SetPosition(0, lineRend.transform.position);
		lineRend.SetPosition(1, ray.origin + ray.direction);
		Debug.Log("shoot");

		RaycastHit[] hitInfo = Physics.RaycastAll(ray);
		hitInfo = hitInfo.OrderBy((x) => x.distance).ToArray();
		foreach (RaycastHit hit in hitInfo)
		{
			IHitable hitable = hit.transform.GetComponent<IHitable>();
			if (hitable != null && (Object)hitable != PlayerController.instance)
			{
				AttackInfo aInfo = new AttackInfo(damage, direction, hit.point, hit.normal);
				hitable.OnHit(aInfo);
				if (aInfo.blocked)
					break;
			}
		}
		PlayerController.instance.m_cameraRot *= Quaternion.Euler(-recoil, 0, 0);

		yield return new WaitForSeconds(cooldown);
		lineRend.enabled = false;
		inAction = false;
	}

	private IEnumerator Reload()
	{
		inAction = true;
		if (ammo_current < ammo_max && ammo > 0)
		{
			animator.SetTrigger("reload");
			Debug.Log("reload");
			yield return new WaitForSeconds(2.5f);
			ammo_current = Mathf.Clamp(ammo, 1, ammo_max);
		}
		else
		{
			Debug.Log("can't reload");
		}
		inAction = false;
	}
}
