using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour, IHitable
{

    Transform target;

    [SerializeField]
    float curForce = 10;

    [SerializeField]
    float forceAdjStep = 1;

    [SerializeField]
    float lookAtSpeed = 50;


    [SerializeField]
    float angularStability = 0.3f;



    protected Rigidbody ragdoll;

    protected virtual void Awake()
    {
        ragdoll = GetComponent<Rigidbody>();
        target = GameObject.Find("Player").transform;
    }

    // Use this for initialization
    void Start () {
		
	}

    void FixedUpdate()
    {
        //height stabilization
        curForce -= forceAdjStep * ragdoll.velocity.y;


        ragdoll.AddForce(Vector3.up * Time.deltaTime * curForce, ForceMode.Impulse);       
        var targetVel = Vector3.Lerp(ragdoll.angularVelocity, Vector3.zero, Time.deltaTime * angularStability);
        ragdoll.angularVelocity = targetVel;



    }
      

	void Update ()
    {
      var rotStep = Time.deltaTime * lookAtSpeed;
        var targetRot = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotStep);
        
    }

    public virtual void OnHit(AttackInfo aInfo)
    {     
        ragdoll.AddForceAtPosition(aInfo.impulse * aInfo.damage * 5, aInfo.point, ForceMode.Impulse);
        ragdoll.maxAngularVelocity = 50;
        ragdoll.angularVelocity = transform.InverseTransformVector(aInfo.impulse) * 50;

        aInfo.damage *= 0.5f;
    }
}
