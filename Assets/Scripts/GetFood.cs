﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFood : MonoBehaviour
{
    [HideInInspector]
    public GameObject targetFood;
    private float movementSpeed;
    private float rotSpeed;

    private float minimumDistToAvoid;
    private float force;
    private float curSpeed;

    //[Tooltip("How long the agent will take when dodging before again moving towards the target")]
    private float dodgingTime;
    private float dodgeTime = 0;

    private Vector3 tarRot;

    private AgentState myAgentState;
    private Wander myWander;

    [SerializeField]
    private float EatingPauseTime = 2.0f;

    private FoodManager foodManager;

    private bool hasEaten;

    //[HideInInspector]
    public float hunger = 0;
    private float starve = 20;
    [HideInInspector]
    public float eatThreshold = 5f;

    private BunnyManager bunnyManager;

    void Start()
    {
        bunnyManager = this.GetComponentInParent<BunnyManager>();

        hasEaten = false;

        foodManager = GameObject.FindObjectOfType<FoodManager>();

        myWander = this.GetComponent<Wander>();
        myAgentState = this.GetComponent<AgentState>();

        movementSpeed = myWander.movementSpeed;
        minimumDistToAvoid = myWander.minimumDistToAvoid;
        force = myWander.force;

        SetRotateSpeedAndDodgeTimeFromSpeed();
    }

    void Update()
    {
        hunger += Time.deltaTime * (movementSpeed / 10);

        if (hunger >= starve)
        {
            bunnyManager.Kill();
        }

        if (myAgentState.myState == State.GettingFood)
        {
            GettingFood();

            if (dodgeTime > 0)
            {
                dodgeTime -= Time.deltaTime;
            }
        }
    }

    private void GettingFood()
    {
        if (Vector3.Distance(targetFood.transform.position, transform.position) <= 2.0f && !hasEaten) //keep these 2 magic numbers equal  V
        {
            hasEaten = true;
            Debug.Log("Target Achieved");
            targetFood.SetActive(false);
            StartCoroutine(Pause(EatingPauseTime, targetFood));
        }

        if (dodgeTime <= 0)
            tarRot = (targetFood.transform.position - transform.position);

        tarRot.Normalize();

        AvoidObstacles(ref tarRot);

        if (Vector3.Distance(targetFood.transform.position, transform.position) < 2.0f) return; //keep these 2 magic numbers equal  ^

        curSpeed = movementSpeed * Time.deltaTime;

        var rot = Quaternion.LookRotation(tarRot);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotSpeed * Time.deltaTime);

        transform.position += transform.forward * curSpeed;
    }

    void GoBackToWandering()
    {
        myAgentState.myState = State.Wandering;
        hasEaten = false;
    }

    public void AvoidObstacles(ref Vector3 dir)
    {
        RaycastHit hit;

        int layerMask = 1 << 8;

        Debug.DrawRay(transform.position, transform.forward * minimumDistToAvoid, Color.red);

        if (Physics.Raycast(transform.position, transform.forward, out hit, minimumDistToAvoid, layerMask))
        {
            Debug.Log("Hit");
            Vector3 hitNormal = hit.normal;
            hitNormal.y = 0.0f;
            dir = transform.forward + hitNormal * force;

            dodgeTime = dodgingTime;
        }
    }

    public void SetRotateSpeedAndDodgeTimeFromSpeed()
    {
        rotSpeed = movementSpeed * 2 / 5;
        dodgingTime = 1 / movementSpeed;
    }

    IEnumerator Pause(float waitTime, GameObject food)
    {
        yield return new WaitForSeconds(waitTime);
        //hunger -= eatThreshold;
        hunger -= 5f;
        Debug.Log(eatThreshold);
        GoBackToWandering();
        foodManager.SpawnNewCarrot(food);
    }
}
