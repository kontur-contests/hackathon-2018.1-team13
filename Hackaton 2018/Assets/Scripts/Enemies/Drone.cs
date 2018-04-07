using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour, IHitable
{

    [SerializeField]
    Transform target;

    [SerializeField]
    float curForce = 10;

    [SerializeField]
    float forceAdjStep = 1;

    [SerializeField]
    float rotSpeed = 1;


    //
    public float angularStability = 0.3f;



    protected Rigidbody ragdoll;

    protected virtual void Awake()
    {
        ragdoll = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start () {
		
	}

    void FixedUpdate()
    {
        curForce -= forceAdjStep * ragdoll.velocity.y;

        ragdoll.AddForce(Vector3.up * Time.deltaTime * curForce, ForceMode.Impulse);
       
        var targetVel = Vector3.Lerp(ragdoll.angularVelocity, Vector3.zero, Time.deltaTime * angularStability);
        ragdoll.angularVelocity = targetVel;
    }
      

	// Update is called once per frame
	void Update () {

     

      var rotStep = Time.deltaTime * rotSpeed;
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
