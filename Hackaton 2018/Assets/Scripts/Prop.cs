using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParticleType = ParticleCaster.ParticleType;

public class Prop : MonoBehaviour, IHitable
{
	protected Rigidbody ragdoll;
	protected bool dead = false;

	[SerializeField]
	private ParticleType particle = ParticleType.wood;

	#region IHitable
	public virtual void OnHit(AttackInfo aInfo)
	{
		aInfo.blocked = true;
		ParticleCaster.Cast(aInfo.point, aInfo.normal, particle);
	}
	#endregion
}
