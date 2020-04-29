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

    // Start is called before the first frame update
    void Start()
    {
        myAgentState = agent.GetComponent<AgentState>();

        this.gameObject.transform.localScale = new Vector3(sightRange * 2, sightRange, this.gameObject.transform.localScale.z);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Food")
        {
            myAgentState.myState = State.GettingFood;
        }
    }
}
