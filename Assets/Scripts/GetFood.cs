using System.Collections;
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

    void Start()
    {
        myWander = this.GetComponent<Wander>();
        myAgentState = this.GetComponent<AgentState>();

        movementSpeed = myWander.movementSpeed;
        minimumDistToAvoid = myWander.minimumDistToAvoid;
        force = myWander.force;

        SetRotateSpeedAndDodgeTimeFromSpeed();
    }

    void Update()
    {
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
        if (Vector3.Distance(targetFood.transform.position, transform.position) <= 1.0f) //keep these 2 magic numbers equal
        {
            Debug.Log("Target Achieved");
            targetFood.SetActive(false);
            StartCoroutine(Pause(EatingPauseTime));
        }

        if (dodgeTime <= 0)
            tarRot = (targetFood.transform.position - transform.position);

        tarRot.Normalize();

        AvoidObstacles(ref tarRot);

        if (Vector3.Distance(targetFood.transform.position, transform.position) < 1.0f) return; //keep these 2 magic numbers equal

        curSpeed = movementSpeed * Time.deltaTime;

        var rot = Quaternion.LookRotation(tarRot);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotSpeed * Time.deltaTime);

        transform.position += transform.forward * curSpeed;
    }

    void GoBackToWandering()
    {
        myAgentState.myState = State.Wandering;
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

    IEnumerator Pause(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        GoBackToWandering();
        Destroy(targetFood);
    }
}
