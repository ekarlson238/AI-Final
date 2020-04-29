using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State { Wandering, GettingFood }

public class AgentState : MonoBehaviour
{
    [HideInInspector]
    public State myState;

    // Start is called before the first frame update
    void Start()
    {
        myState = State.Wandering; //the initial state
    }
}
