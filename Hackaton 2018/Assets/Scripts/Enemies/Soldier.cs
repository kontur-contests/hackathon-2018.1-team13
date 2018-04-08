using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Soldier : EnemyController
{
	private Vector3 startPos;

	enum Behaviour { idle, attack, move, inair };
	[SerializeField]
	private Behaviour behaviuor = Behaviour.idle;

	public EnumWeapon eWeapon = EnumWeapon.revolver;
	private EnemyWeapon weapon;

	public LineRenderer lineRend;

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		capsule = GetComponent<Collider>();
		lineRend = GetComponentInChildren<LineRenderer>();

		startPos = transform.position;

		if (eWeapon == EnumWeapon.fists)
			weapon = new EnemyFists();
		else if (eWeapon == EnumWeapon.melee)
			weapon = new EnemyMelee();
		else if (eWeapon == EnumWeapon.revolver)
			weapon = new EnemyRevolver();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (dead)
			return;

		see = Detect();
		UpdateState();
		Animation();
		LookAt(PlayerController.instance.transform.position);

		isGrounded = false;
		RaycastHit hitInfo;
		if (Physics.SphereCast(transform.position + Vector3.up * 0.5f, capsule.bounds.extents.x, Vector3.down, out hitInfo, LayerMask.NameToLayer("Default")))
		{
			isGrounded = true;
		}
	}
	
	private bool isGrounded = false;
	void OnCollisionStay(Collision col)
	{

		isGrounded = false;
		foreach (ContactPoint cp in col.contacts)
		{
			if (cp.normal == Vector3.up)
			{
				isGrounded = true;
				return;
			}
		}
	}

	private void Animation()
	{
		animator.SetBool("run", behaviuor == Behaviour.move);
		animator.SetBool("falling", behaviuor == Behaviour.inair);
	}

	private void UpdateState()
	{
		if (behaviuor == Behaviour.inair)
		{
			if (isGrounded)
				behaviuor = Behaviour.idle;
		}
		else if (behaviuor != Behaviour.attack)
		{
			if (agent.isOnOffMeshLink)
			{
				behaviuor = Behaviour.inair;
				float dist = Vector3.Distance(agent.currentOffMeshLinkData.startPos, agent.currentOffMeshLinkData.endPos);
				StartCoroutine(JumpDown(0.5f, dist * 0.9f));
			}
			else
			{
				if (aware)
				{
					if (see)
					{
						float dist = Vector3.Distance(pointCenter, PlayerController.instance.pointCenter);
						if (dist < weapon.range)
						{
							agent.ResetPath();
							StartCoroutine(Attack(PlayerController.instance.pointCenter, dist < 1 ? true : false));
						}
						else if (behaviuor != Behaviour.move || agent.remainingDistance < 0.5f)
							GoTo(lastDetectedPos);
					}
					else if (behaviuor != Behaviour.move)
					{
						GoTo(lastDetectedPos);
					}
				}
				else if (awareness > 0 && behaviuor != Behaviour.move)
				{
					GoTo(startPos);
				}
			}
		}
	}

	private void GoTo(Vector3 pos)
	{
		behaviuor = Behaviour.move;
		agent.SetDestination(lastDetectedPos);
	}
	
	private IEnumerator JumpDown (float height, float duration)
	{
		animator.SetTrigger("jump");
		OffMeshLinkData data = agent.currentOffMeshLinkData;
		Vector3 startPos = agent.transform.position;
		Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
		float normalizedTime = 0.0f;
		while (normalizedTime < 1.0f)
		{
			behaviuor = Behaviour.inair;
			float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
			agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
			normalizedTime += Time.deltaTime / duration;
			yield return null;
		}
		behaviuor = Behaviour.idle;
		agent.CompleteOffMeshLink();
	}

	private void LookAt(Vector3 target)
	{
		if (behaviuor != Behaviour.attack)
			return;

		Vector3 direction = target - transform.position;
		direction.y = 0;
		Quaternion rotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10 * Time.deltaTime);
	}

	public void Disarm()
	{
		weapon = new EnemyFists();
	}

	protected virtual IEnumerator Attack(Vector3 target, bool melee = false)
	{
		EnemyWeapon w = melee ? new EnemyFists() : weapon;
		behaviuor = Behaviour.attack;
		
		yield return new WaitForSeconds(w.delay);

		Vector3 direction = Spread(w.spread) * (target - pointCenter);
		Ray ray = new Ray(pointCenter, direction);
		lineRend.enabled = true;
		lineRend.SetPosition(0, lineRend.transform.position);
		lineRend.SetPosition(1, pointCenter + direction * 5);

		// Debug.DrawRay(pointCenter, direction, Color.green, 2f);
		//Debug.Log("shoot");
		animator.SetTrigger(w.trigger);

		RaycastAttack(ray, weapon.damage, weapon.range);

		yield return new WaitForSeconds(1 - w.delay);
		lineRend.enabled = false;
		behaviuor = Behaviour.idle;
	}

	

	private bool Detect()
	{
		float hits = 0;

		if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) < visionRange)
		{
			Vector3[] points = {
				PlayerController.instance.pointCenter,
				PlayerController.instance.pointHead,
				PlayerController.instance.pointFeet
			};

			RaycastHit hitInfo;
			for (int i = 0; i < points.Length; i++)
			{
				//Debug.DrawLine(pointCenter, points[i], Color.black);
				if (Physics.Linecast(pointCenter, points[i], out hitInfo))
				{
					if (hitInfo.transform == PlayerController.instance.transform)
					{
						//Debug.DrawLine(pointCenter, points[i], Color.red);
						if (awareness > awareAt)
						{
							aware = true;
							awareness = awareMax;
							lastDetectedPos = PlayerController.instance.transform.position;
							return true;
						}
						hits++;
					}
				}
			}
		}

		if (hits > 0)
		{
			if (awareness < awareMax)
				awareness += hits * Time.deltaTime * 0.5f;
			return true;
		}
		else
		{
			if (aware && awareness < awareAt)
				aware = false;
			else if (awareness > 0)
				awareness -= Time.deltaTime;
			return false;
		}
	}

	public enum EnumWeapon { fists, melee, revolver }
	public abstract class EnemyWeapon
	{
		public float range;
		public float damage;
		public float spread;
		public float delay;
		public string trigger;
	}

	public class EnemyFists : EnemyWeapon
	{
		public EnemyFists()
		{
			range = 1;
			damage = 0.5f;
			spread = 0;
			delay = 0.5f;
			trigger = "melee";
		}
	}

	public class EnemyMelee : EnemyWeapon
	{
		public EnemyMelee()
		{
			range = 1;
			damage = 1;
			spread = 0;
			delay = 0.5f;
			trigger = "melee";
		}
	}

	public class EnemyRevolver : EnemyWeapon
	{
		public EnemyRevolver()
		{
			range = 5;
			damage = 1;
			spread = 3;
			delay = 0.5f;
			trigger = "fire";
		}
	}
}
