using UnityEngine;
using System.Collections;
public class Wander : MonoBehaviour
{
    private Vector3 tarPos;
    [SerializeField]
    private float movementSpeed = 5.0f;
    [SerializeField]
    private float rotSpeed = 2.0f;
    [SerializeField]
    private float maxMinX, maxMinZ;
    private float minX, maxX, minZ, maxZ;
    [HideInInspector]
    public bool wander;

    public float minimumDistToAvoid = 5.0f;
    public float mass = 5.0f;
    public float force = 50.0f;
    private float curSpeed;

    [SerializeField][Tooltip("How long the agent will take when dodging before again moving towards the target")]
    private float dodgingTime = 0.2f;
    private float dodgeTime = 0;

    private Vector3 tarRot;

    [SerializeField]
    private GameObject targetMarker;

    void Start()
    {
        minX = -maxMinX;
        maxX = maxMinX;
        minZ = -maxMinZ;
        maxZ = maxMinZ;

        wander = true;

        GetNextPosition();
    }

    void Update()
    {
        if (wander)
        {
            RandomlyWander();
        }

        if (dodgeTime > 0)
        {
            dodgeTime -= Time.deltaTime;
        }
    }

    private void RandomlyWander()
    {
        if (Vector3.Distance(tarPos, transform.position) <= 3.0f)
        {
            Debug.Log("Target Achieved");
            GetNextPosition();
        }
        
        if (dodgeTime <= 0) 
            tarRot = (tarPos - transform.position);

        tarRot.Normalize();

        AvoidObstacles(ref tarRot);

        if (Vector3.Distance(tarPos, transform.position) < 3.0f) return;

        curSpeed = movementSpeed * Time.deltaTime;

        var rot = Quaternion.LookRotation(tarRot);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotSpeed * Time.deltaTime);
        
        transform.position += transform.forward * curSpeed;
    }

    void GetNextPosition()
    {
        tarPos = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));
        targetMarker.transform.position = tarPos;
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
}
