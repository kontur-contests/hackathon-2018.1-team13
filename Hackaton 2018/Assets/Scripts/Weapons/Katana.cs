using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Katana : AWeapon
{
	public AudioClip sound_ready;
	
	private void OnEnable()
	{
		audio.PlayOneShot( sound_ready );
	}

	private void Update()
	{
		if (PlayerController.input.b_LMB)
		{
			if (!inAction)
			{
				StartCoroutine(Primary());
			}
		}
	}

	float damage = 5f;
	float cooldown = 0.5f;
	float distance = 1.5f;
    int attackCounter = 0;

    const int ATTACK_ANIMATIONS_COUNT = 2;

	private IEnumerator Primary()
	{
		inAction = true;

		if (audio)
			audio.PlayOneShot( audio.clip );

		Vector3 origin = PlayerController.instance.cameraController.transform.position;
		Vector3 direction = PlayerController.instance.cameraController.transform.forward;
		
		Ray ray = new Ray(origin, direction);

		Debug.DrawRay(origin, direction, Color.red, 2f);

		animator.SetTrigger("fire");

        animator.SetInteger ("Rand", attackCounter++ % ATTACK_ANIMATIONS_COUNT);
		//lineRend.enabled = true;
		//lineRend.SetPosition(0, lineRend.transform.position);
		//lineRend.SetPosition(1, ray.origin + ray.direction);

		RaycastHit[] hitInfo = Physics.SphereCastAll(origin, 1, direction, 1);
		foreach (RaycastHit hit in hitInfo)
		{
			IHitable hitable = hit.transform.GetComponent<IHitable>();
			if (hitable != null && (Object)hitable != PlayerController.instance)
			{
				AttackInfo aInfo = new AttackInfo(damage, direction, hit.point, hit.normal);
				hitable.OnHit(aInfo);
			}
		}

		yield return new WaitForSeconds(cooldown);
		//lineRend.enabled = false;
		inAction = false;
	}
}
