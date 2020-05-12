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
    private GetLaid myGetLaid;

    // Start is called before the first frame update
    void Start()
    {
        myAgentState = agent.GetComponent<AgentState>();
        myGetFood = agent.GetComponent<GetFood>();
        myGetLaid = agent.GetComponent<GetLaid>();

        this.gameObject.transform.localScale = new Vector3(sightRange * 2, sightRange, this.gameObject.transform.localScale.z);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(myGetLaid.hornyLevelBreedThreshold);

        if (other.tag == "Food" && myGetFood.hunger >= 5f && myAgentState.myState == State.Wandering)
        {
            Debug.Log("carrot");

            myGetFood.targetFood = other.gameObject;
            myAgentState.myState = State.GettingFood;
        }

        if (other.tag == "Bunny" && myGetLaid.hornyLevel >= 10f && myAgentState.myState == State.Wandering)
        {
            GameObject otherBunny = other.gameObject;
            GetLaid otherGetLaid = otherBunny.GetComponent<GetLaid>();

            if (otherGetLaid.hornyLevel >= 10f)
            {
                myGetLaid.targetMate = otherBunny;
                otherGetLaid.targetMate = this.gameObject;
                otherGetLaid.MadeMate();
                myAgentState.myState = State.Breeding;
            }
        }
    }
}
