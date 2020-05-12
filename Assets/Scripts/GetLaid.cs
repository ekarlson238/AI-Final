using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetLaid : MonoBehaviour
{
    [HideInInspector]
    public GameObject targetMate;
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

    private bool hasBred;

    [HideInInspector]
    public float hornyLevel = 0;
    [HideInInspector]
    public float hornyLevelBreedThreshold = 10;

    [SerializeField]
    private GameObject bunnyPrefab;

    [HideInInspector]
    public float hornyGainMult = 1;

    void Start()
    {
        hasBred = false;

        myWander = this.GetComponent<Wander>();
        myAgentState = this.GetComponent<AgentState>();

        movementSpeed = myWander.movementSpeed;
        minimumDistToAvoid = myWander.minimumDistToAvoid;
        force = myWander.force;

        SetRotateSpeedAndDodgeTimeFromSpeed();
    }

    void Update()
    {
        hornyLevel += Time.deltaTime * hornyGainMult;

        if (targetMate == null && myAgentState.myState == State.Breeding)
        {
            GoBackToWandering();
        }

        if (myAgentState.myState == State.Breeding)
        {
            GettingLaid();

            if (dodgeTime > 0)
            {
                dodgeTime -= Time.deltaTime;
            }
        }
    }

    private void GettingLaid()
    {
        if (Vector3.Distance(targetMate.transform.position, transform.position) <= 3.0f && !hasBred) //keep these 2 magic numbers equal  V
        {
            hasBred = true;
            Debug.Log("Target Achieved");
            targetMate.SetActive(false);
            StartCoroutine(Pause(EatingPauseTime, targetMate));
        }

        if (dodgeTime <= 0)
            tarRot = (targetMate.transform.position - transform.position);

        tarRot.Normalize();

        AvoidObstacles(ref tarRot);

        if (Vector3.Distance(targetMate.transform.position, transform.position) < 3.0f) return; //keep these 2 magic numbers equal  ^

        curSpeed = movementSpeed * Time.deltaTime;

        var rot = Quaternion.LookRotation(tarRot);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotSpeed * Time.deltaTime);

        transform.position += transform.forward * curSpeed;
    }

    void GoBackToWandering()
    {
        hornyLevel = 0;
        myAgentState.myState = State.Wandering;
        hasBred = false;
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

    public void MadeMate()
    {
        myAgentState.myState = State.Breeding;
    }

    IEnumerator Pause(float waitTime, GameObject food)
    {
        yield return new WaitForSeconds(waitTime);
        GoBackToWandering();

        int rand = Random.Range(1, 3);

        for (int i = 0; i <= rand; i++)
        {
            Instantiate(bunnyPrefab, this.transform.localPosition, Quaternion.identity);
        }
    }
}
