﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyController : MonoBehaviour
{
	protected NavMeshAgent agent;
	protected Animator animator;
	protected Collider capsule;

	protected float health = 3;
	protected bool dead = false;

	protected float visionRange = 5f;
	[SerializeField]
	protected bool aware = false;
	[SerializeField]
	protected bool see = false;
	protected float awareness = 0f;
	protected float awareMax = 10f;
	protected float awareAt = 2f;
	protected Vector3 lastDetectedPos;
	protected Vector3 pointCenter	{	get	{	return transform.position + Vector3.up * agent.height * 0.5f;	}	}


	public virtual bool OnTakeDamage(BodyPart part, AttackInfo aInfo)
	{
		if (dead)
			return true;

		health -= aInfo.damage;
		if (health > 0)
		{
			aware = true;
			awareness = 10;
			lastDetectedPos = PlayerController.instance.transform.position;
			Debug.Log(gameObject.name + ": Got Hit");
			return false;
		}
		else
		{
			Debug.Log(gameObject.name + ": Diez");
			dead = true;
			foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
			{
				rb.gameObject.layer = LayerMask.NameToLayer("Ignore");
				rb.isKinematic = false;
				// rb.AddForceAtPosition(aInfo.impulse * 1, aInfo.point, ForceMode.Impulse);
				// rb.AddTorque(aInfo.impulse * 10, ForceMode.Impulse);
			}

            if (capsule != null)
			    capsule.gameObject.layer = LayerMask.NameToLayer("Ignore");

            if (animator != null)
                animator.enabled = false;

			if (agent)
				agent.enabled = false;

			StopAllCoroutines();
			StartCoroutine(FadeOut(10, 2));
			return true;
		}
	}

	private IEnumerator FadeOut(float delay, float time)
	{
		yield return new WaitForSeconds(delay);
		float t = 0;
		while (t < 1)
		{
			t += Time.deltaTime / time;
			yield return null;
		}
        Destroy(this.gameObject);
		//gameObject.SetActive(false);
		yield return null;
	}
}
