using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : EnemyController
{

    Transform aimTarget;

    Transform moveTarget;

    [SerializeField]
    float curForce = 10;

    [SerializeField]
    float forceAdjStep = 1;

    public float hoverForce = 5.0F;
    public float hoverDamp = 0.5F;

    [SerializeField]
    float lookAtSpeed = 50;

    [SerializeField]
    float angularStability = 0.3f;

    [SerializeField]
    float targetDistanceSqr = 9;

    [SerializeField]
    float moveForce = 1;

    [SerializeField]
    float speedClamp = 1;

    [SerializeField]
    float maxAngularVelocity = 50;

    [SerializeField]
    public float hoverHeight = 4;



    protected Rigidbody ragdoll;

    protected virtual void Awake()
    {
        ragdoll = GetComponent<Rigidbody>();
        aimTarget = GameObject.Find("Player").transform;
        moveTarget = aimTarget;
    }

    // Use this for initialization
    void Start () {
        transform.LookAt(aimTarget);
	}

    void FixedUpdate()
    {        
        HeightStabilization();       
        AngularStabilization();
        MoveToTarget();
    }



    void AngularStabilization()
    {
        var targetAngVel = Vector3.Lerp(ragdoll.angularVelocity, Vector3.zero, Time.deltaTime * angularStability);
        ragdoll.angularVelocity = targetAngVel;
    }

    private void HeightStabilization()
    {
        //curForce -= forceAdjStep * ragdoll.velocity.y;
        //ragdoll.AddForce(Vector3.up * Time.deltaTime * curForce, ForceMode.Impulse);

        RaycastHit hit;
        Ray downRay = new Ray(transform.position, -Vector3.up);
        if (Physics.Raycast(downRay, out hit))
        {
            float hoverError = hoverHeight - hit.distance;
            if (hoverError > 0)
            {
                float upwardSpeed = ragdoll.velocity.y;
                float lift = hoverError * hoverForce - upwardSpeed * hoverDamp;
                ragdoll.AddForce(Mathf.Clamp( lift, -curForce, curForce) * Vector3.up);
            }
        }


    }

    void MoveToTarget()
    {
        //Don't move on hit
        Vector3 relativePos = moveTarget.position - gameObject.transform.position;
        var vel = ragdoll.velocity;
        var sqDisToRange = relativePos.sqrMagnitude - targetDistanceSqr;

        var targetLinVel = Vector3.Lerp(ragdoll.velocity, relativePos.normalized * sqDisToRange, Time.deltaTime * moveForce);

        //ragdoll.velocity = new Vector3(Mathf.Clamp(targetLinVel.y, -speedClamp, speedClamp), vel.y, Mathf.Clamp(targetLinVel.z, -speedClamp, speedClamp));
        //ragdoll.velocity = new Vector3(targetLinVel.x, Mathf.Clamp( vel.y, -speedClamp, speedClamp), targetLinVel.z);
        ragdoll.velocity = new Vector3(targetLinVel.x, vel.y, targetLinVel.z);
    }


    void Update ()
    {
        //aim
        var rotStep = Time.deltaTime * lookAtSpeed;
        var targetRot = Quaternion.LookRotation(aimTarget.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotStep);
    }

    public virtual void OnHit(AttackInfo aInfo)
    {     
        ragdoll.AddForceAtPosition(aInfo.impulse * aInfo.damage * 5, aInfo.point, ForceMode.Impulse);
        ragdoll.maxAngularVelocity = maxAngularVelocity;
        ragdoll.angularVelocity = transform.InverseTransformVector(aInfo.impulse) * maxAngularVelocity;

        aInfo.damage *= 0.5f;
    }
}
