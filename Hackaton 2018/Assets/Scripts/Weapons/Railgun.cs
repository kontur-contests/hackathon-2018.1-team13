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
    private float heatGainPerShot = 0.5f;

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
	
    public float HeatBar
    {
        get
        {
            return Mathf.Clamp01(currentHeat / heatCap);
        }
    }

    private bool Overheated
    {
        get
        {
            return currentHeat > heatCap;
        }
    }

    private bool cooling;

    public override void Reset()
    {
        base.Reset();
        cooling = false;
        currentHeat = 0;
    }

	float damage = 0.1f;
	float distance = 20f;

	private void FixedUpdate()
	{
		if (inAction && !cooling)
		{
			currentHeat += heatGainPerShot;
			if (Overheated)
			{
				StartCoroutine(Owerheat());
				return;
			}

			lineRend.enabled = true;
			RaycastHit hit;

			Vector3 origin = PlayerController.instance.cameraController.transform.position;
			Vector3 direction = PlayerController.instance.cameraController.transform.forward;
			currentHeat += heatGainPerShot;

			Ray ray = new Ray(origin, direction);
			
			int steps = Mathf.CeilToInt( distance );
			Vector3 final = origin + direction * distance;
			if (Physics.SphereCast(ray, 0.3f, out hit, distance))
			{
				final = hit.point;
				steps = Mathf.Max( Mathf.CeilToInt(hit.distance), 2 );

				IHitable hitable = hit.transform.GetComponent<IHitable>();
				if (hitable != null && (Object)hitable != PlayerController.instance)
				{
					AttackInfo aInfo = new AttackInfo(damage, direction * 0.001f, hit.point, hit.normal);
					hitable.OnHit(aInfo);
				}
			}
			Debug.DrawLine(origin, final, Color.red, 2f);
			lineRend.positionCount = steps;
			lineRend.SetPosition(0, lineRend.transform.position);
			lineRend.SetPosition(steps - 1, final);
			for ( int i = 1; i < steps - 1; i++ )
			{
				Vector3 dir = Vector3.Lerp(lineRend.transform.position, final, Mathf.Floor(i) / steps);
				Vector3 vec = new Vector3( Random.Range( -1, 1 ), Random.Range( -1, 1 ), Random.Range( -1, 1 ) ) * 0.1f;
				dir += vec;
				lineRend.SetPosition(i, dir);
			}
		}
		else
			lineRend.enabled = false;
	}

	private void Update()
	{
        Color color = PlayerController.instance.HeatIndicator.color;
        color.a = 0;
        PlayerController.instance.HeatIndicator.color = color;
		return;

        if (Overheated)
        {
            if (!inAction)
            {
                StartCoroutine(Owerheat());
            }
        }

		if (!cooling)
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

        PlayerController.instance.HeatIndicator.fillAmount = HeatBar;
        Color col = PlayerController.instance.HeatIndicator.color;
        col.a = cooling ? 1 : HeatBar;
        PlayerController.instance.HeatIndicator.color = col;
	}

	private IEnumerator Owerheat()
	{
		inAction = true;
        cooling = true;
        yield return new WaitForSeconds(overheatPunishnent);
        animator.SetTrigger("Owerheat");
        Debug.Log("Owerheat");
        while (currentHeat >= 0)
        {
            currentHeat -= heatLoose * Time.deltaTime;
            yield return null;
        }
        currentHeat = 0;
        cooling = false;
        inAction = false;
	}
}
