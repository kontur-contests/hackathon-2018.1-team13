using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class AWeapon : MonoBehaviour
{
	protected Animator animator;
	public bool equiped = false;
	public bool inAction = false;
	public bool drawn = false;

	public virtual bool CanUnequip() {	return !inAction && !drawn;	}
	public virtual bool CanEquip()	{	return equiped;	}

	protected virtual void Awake()
	{
		animator = GetComponent<Animator>();
	}

	protected Quaternion Spread(float spread)
	{
		float rnd = (Random.value - 0.5f) * spread;
		return Quaternion.Euler(rnd, rnd, rnd);
	}
}