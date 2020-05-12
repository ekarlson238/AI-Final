using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyManager : MonoBehaviour
{
    private Wander myWander;
    private GetLaid myGetLaid;

    private void Awake()
    {
        myWander = this.GetComponentInChildren<Wander>();
        myGetLaid = this.GetComponentInChildren<GetLaid>();

        RandomiseStats();
    }

    public void RandomiseStats()
    {
        myWander.movementSpeed += Random.Range(-2f, 2f);
        myGetLaid.hornyGainMult += Random.Range(-0.2f, 0.2f);
    }

    public void Kill()
    {
        Destroy(this.gameObject);
    }
}
