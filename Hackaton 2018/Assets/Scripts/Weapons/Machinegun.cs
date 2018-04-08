using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Machinegun : AWeapon
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

    public float HeatBar
    {
        get
        {
            return Mathf.Clamp01(currentHeat / heatCap);
        }
    }

    private LineRenderer lineRend;

	protected override void Awake()
	{
		lineRend = GetComponentInChildren<LineRenderer>();
		base.Awake();
	}

    private bool m_cooling;
    private bool cooling
    {
        get
        {
            return m_cooling;
        }
        set
        {
            m_cooling = value;
            animator.SetBool("cooling", value);
        }
    }

    private bool Overheated
    {
        get
        { 
            return currentHeat > heatCap;
        }
    }
    

    public override void Reset()
    {
        base.Reset();
        cooling = false;
        currentHeat = 0;
    }

    private void Update()
	{
        if (Overheated)
        {
            if (!inAction && !cooling)
            {
                StartCoroutine(Owerheat());
            }
        }

        if (PlayerController.input.b_LMB)
		{
			if (!inAction && !cooling)
			{				
				StartCoroutine(Primary());				
			}
		}

        if ( currentHeat > 0)
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
	
	float spread = 1.5f;
	float damage = 2f;
	float recoil = 3f;
    float effectTime = 0.01f;
	float cooldown = 0.15f;

	private IEnumerator Primary()
	{
		inAction = true;

		if (audio)
			audio.PlayOneShot( audio.clip );
            
		Vector3 origin = PlayerController.instance.cameraController.transform.position;
		Vector3 direction = PlayerController.instance.cameraController.transform.forward;
        currentHeat += heatGainPerShot;
     
		direction = Spread(spread) * direction;
		Ray ray = new Ray(origin, direction);

		Debug.DrawRay(origin, direction, Color.red, 2f);

		animator.SetTrigger("fire");
		lineRend.enabled = true;
		lineRend.SetPosition(0, lineRend.transform.position);
		lineRend.SetPosition(1, ray.origin + ray.direction * 5);
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

        yield return new WaitForSeconds(effectTime);
		lineRend.enabled = false;
		yield return new WaitForSeconds(cooldown - effectTime);
		inAction = false;
	}

	private IEnumerator Owerheat()
	{
		inAction = true;
        cooling = true;
        yield return new WaitForSeconds(overheatPunishnent);
        animator.SetTrigger("Owerheat");
        Debug.Log("Owerheat");
        inAction = false;
        while (currentHeat > 0)
        {
            currentHeat -= heatLoose * Time.deltaTime;
            yield return null;
        }
        currentHeat = 0;
        cooling = false;   
	}
}
