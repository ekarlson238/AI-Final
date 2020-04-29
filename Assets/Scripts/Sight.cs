using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    //this script is attached to the cone itself not the player
    [SerializeField]
    private GameObject agent;

    public float sightRange = 1;

    private AgentState myAgentState;
    private GetFood myGetFood;

    // Start is called before the first frame update
    void Start()
    {
        myAgentState = agent.GetComponent<AgentState>();
        myGetFood = agent.GetComponent<GetFood>();

        this.gameObject.transform.localScale = new Vector3(sightRange * 2, sightRange, this.gameObject.transform.localScale.z);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Food")
        {
            myGetFood.targetFood = other.gameObject;
            myAgentState.myState = State.GettingFood;
        }
    }
}
