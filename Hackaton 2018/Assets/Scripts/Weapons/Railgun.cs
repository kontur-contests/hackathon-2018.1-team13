using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Railgun : AWeapon
{
	[SerializeField]
    private float heatCap = 10;

    [SerializeField]
    private float heatLoose = 0.2f;

    [SerializeField]
    private float heatGainPerShot = 1;

    [SerializeField]
    private float overheatPunishnent = 0.01f;

    [SerializeField]
    private float currentHeat = 0;



    private LineRenderer lineRend;

	protected override void Awake()
	{
		lineRend = GetComponentInChildren<LineRenderer>();
		base.Awake();
	}

    private bool Overheated
    {
        get
        {
            return currentHeat > heatCap;
        }
    }

	private void FixedUpdate()
	{
		if (inAction)
		{
			currentHeat += heatGainPerShot;

			lineRend.enabled = true;
			RaycastHit hit;

			float distance = 100;

			Vector3 origin = PlayerController.instance.cameraController.transform.position;
			Vector3 direction = PlayerController.instance.cameraController.transform.forward;
			currentHeat += heatGainPerShot;

			Ray ray = new Ray(origin, direction);
			
			lineRend.SetPosition(0, lineRend.transform.position);
			if (Physics.SphereCast(ray, 1, out hit, distance))
			{
				int dist = Mathf.FloorToInt(hit.distance);
				lineRend.positionCount = dist;
				for ( int i = 1; i < dist - 1; i++ )
				{
					
				}
				lineRend.SetPosition(dist - 1, hit.point);
			}
			else
			{
				lineRend.positionCount = 2;
				lineRend.SetPosition(1, direction * distance);
			}
		}
		else
			lineRend.enabled = false;
	}

	private void Update()
	{
        if (Overheated)
        {
            if (!inAction)
            {
                StartCoroutine(Owerheat());
            }
        }

        if (PlayerController.input.b_LMB)
		{
			inAction = true;
		}
		else
		{
			inAction = false;
		}

        if (!inAction && currentHeat > 0)
        {
            currentHeat -= heatLoose * Time.deltaTime;
            if (currentHeat < 0)
                currentHeat = 0;
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
        currentHeat += heatGainPerShot;
     
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

	private IEnumerator Owerheat()
	{
		inAction = true;
        yield return new WaitForSeconds(overheatPunishnent);
        animator.SetTrigger("Owerheat");
        Debug.Log("Owerheat");
        while (currentHeat >= 0)
        {
            currentHeat -= heatLoose * Time.deltaTime;
            yield return null;
        }
        currentHeat = 0;
        inAction = false;
	}
}
