﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : EnemyController
{
    Transform aimTarget;

    [SerializeField]
    Transform shootingPoint;

    Transform moveTarget;

    public LineRenderer lineRend;

    [SerializeField]
    float curForce = 10;

    [SerializeField]
    protected Animator droneAnimator;

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
    float detectDistanceSqr = 72;

    [SerializeField]
    float moveForce = 1;

    [SerializeField]
    float speedClamp = 1;



    [SerializeField]
    float maxAngularVelocity = 50;

    [SerializeField]
    public float hoverHeight = 4;

    [SerializeField]
    float fireRange = 34;

    protected Rigidbody ragdoll;

    protected enum State
    {
        Idle,
        Attack,
        Wait
    }

    protected State droneState = State.Idle;

    protected virtual bool IsAgressive()
    {
        return true;
    }

    private AudioSource m_audio;
    protected virtual void Awake()
    {
        m_audio = GetComponent<AudioSource>();
        ragdoll = GetComponent<Rigidbody>();
        aimTarget = GameObject.Find("Player").transform;
        droneAnimator.SetInteger("State", 1);
        moveTarget = aimTarget;
    }

    // Use this for initialization
    void Start() {
        transform.LookAt(aimTarget);
    }

    void FixedUpdate()
    {
        if (dead)
            return;
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
                ragdoll.AddForce(Mathf.Clamp(lift, -curForce, curForce) * Vector3.up);
            }
        }

    }


     protected bool inFireRange = false;

    void MoveToTarget()
    {
        //Don't move on hit
        Vector3 relativePos = moveTarget.position - gameObject.transform.position;
        var vel = ragdoll.velocity;
       
        var distSqMag = relativePos.sqrMagnitude;
        if (distSqMag > detectDistanceSqr)
            return;

        var sqDisToRange = distSqMag - targetDistanceSqr;

        inFireRange = distSqMag < fireRange;

        var targetLinVel = Vector3.Lerp(ragdoll.velocity, relativePos.normalized * sqDisToRange , Time.deltaTime * moveForce);

        ragdoll.velocity = new Vector3(Mathf.Clamp(targetLinVel.x, -speedClamp, speedClamp), vel.y, Mathf.Clamp(targetLinVel.z, -speedClamp, speedClamp));
        //ragdoll.velocity = new Vector3(targetLinVel.x, Mathf.Clamp( vel.y, -speedClamp, speedClamp), targetLinVel.z);
        //ragdoll.velocity = new Vector3(targetLinVel.x, vel.y, targetLinVel.z);
    }


    protected virtual void Update()
    {
        if (dead)
        {
           return;
        }
        //aim

        var rotStep = Time.deltaTime * lookAtSpeed;
        var deltaVector = aimTarget.position - transform.position;
        var targetRot = Quaternion.LookRotation(deltaVector);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotStep);

        if (droneState == State.Attack || droneState == State.Wait || !inFireRange 
            || !IsAgressive())
            return;

        var angle = Vector3.Angle(deltaVector, transform.forward);

        if (angle > 15)
            return;

        StartCoroutine(Attack());
    }
    [SerializeField]
    private float weaponSpread = 0.5f;

    [SerializeField]
    private float weaponDmg = 0.5f;

    [SerializeField]
    private float weaponRange = 10f;

    [SerializeField]
    private float weaponReloadSec = 3f;

    [SerializeField]
    private float weaponFireRate = 0.23f;

    protected IEnumerator Attack()
    {
        droneState = State.Attack;
        yield return new WaitForSeconds(1);

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(weaponFireRate);

            Vector3 direction = Spread(weaponSpread) * transform.forward;
            Ray ray = new Ray(shootingPoint.position, direction);
            RaycastAttack(ray, weaponDmg, weaponRange);
            var rend = Instantiate(lineRend);
            m_audio.Play();
            rend.transform.position = shootingPoint.transform.position;
            rend.enabled = true;
            rend.SetPosition(0, rend.transform.position);
            rend.SetPosition(1, shootingPoint.position + direction * 5);
            
            Destroy(rend, 1);
        }

        droneState = State.Wait;

        yield return new WaitForSeconds(weaponReloadSec - 1);

        Debug.Log("Pew");
        droneState = State.Idle;
    }

    public override bool OnTakeDamage(BodyPart part, AttackInfo aInfo)
    {
        ragdoll.AddForceAtPosition(aInfo.impulse * aInfo.damage * 5, aInfo.point, ForceMode.Impulse);
        ragdoll.maxAngularVelocity = maxAngularVelocity;
        ragdoll.angularVelocity = transform.InverseTransformVector(aInfo.impulse) * maxAngularVelocity;

        return base.OnTakeDamage(part, aInfo);
      
    }
}
