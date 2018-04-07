using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitable
{
	void OnHit(AttackInfo aInfo);
}

public class AttackInfo
{
	public float damage;
	public Vector3 impulse;
	public Vector3 point;
	public Vector3 normal;
	
	public bool blocked = false;
	
	public AttackInfo(float damage, Vector3 impulse, Vector3 point, Vector3 normal)
	{
		this.damage = damage;
		this.impulse = impulse.normalized;
		this.point = point;
		this.normal = normal;
	}
}