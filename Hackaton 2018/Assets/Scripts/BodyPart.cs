using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParticleType = ParticleCaster.ParticleType;

public class BodyPart : MonoBehaviour, IHitable
{
	// private EnemyController enemy;
	private Rigidbody rb;

	[SerializeField]
	private ParticleType particle = ParticleType.blood;
	
	public float damage_mod = 1f;

	private void Awake()
	{
		// enemy = GetComponentInParent<EnemyController>();
		rb = GetComponentInParent<Rigidbody>();
		rb.isKinematic = true;
	}

	public void OnHit(AttackInfo aInfo)
	{
		// print(enemy.gameObject.name + " hit in " + gameObject.name);
		aInfo.damage *= damage_mod;
		// if (enemy.OnTakeDamage(this, aInfo))
			rb.AddForceAtPosition(aInfo.impulse * aInfo.damage * 5, aInfo.point, ForceMode.Impulse);

		aInfo.blocked = true;
		ParticleCaster.Cast(aInfo.point, aInfo.normal, particle);
	}
}
