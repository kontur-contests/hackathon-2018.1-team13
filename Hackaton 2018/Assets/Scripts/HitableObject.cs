using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParticleType = ParticleCaster.ParticleType;

public class HitableObject : MonoBehaviour, IHitable
{
	protected Rigidbody ragdoll;
	protected bool dead = false;
	
	public ParticleType particle = ParticleType.metal;
	public bool fragile = false;

	protected virtual void Awake()
	{
		ragdoll = GetComponent<Rigidbody>();
	}

	#region IHitable
	public virtual void OnHit(AttackInfo aInfo)
	{
		if (!dead)
			StartCoroutine(FadeOut(fragile ? 0.5f : 5, fragile ? 0.5f : 2));

		dead = true;
		transform.parent = null;
		ragdoll.isKinematic = false;
		ragdoll.AddForceAtPosition(aInfo.impulse * aInfo.damage * 5, aInfo.point, ForceMode.Impulse);
		ragdoll.maxAngularVelocity = 50;
		ragdoll.angularVelocity = transform.InverseTransformVector(aInfo.impulse) * 50;

		aInfo.damage *= 0.5f;
		ParticleCaster.Cast(aInfo.point, aInfo.normal, particle);
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
		// gameObject.SetActive(false);
		yield return null;
	}
	#endregion
}
