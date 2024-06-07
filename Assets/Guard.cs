using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static event System.Action OnGuardSpottedPlayer;

    [SerializeField] Transform pathHolder;
    [SerializeField] float speed = 5;
    [SerializeField] float waitTime = 0.3f;
    [SerializeField] float turnSpeed = 90;
    [SerializeField] float timeToSpotPlayer = 0.5f;

    [SerializeField] Light spotLight;
    Color originalSpotlightColor;
    [SerializeField] float viewDistance;
    float viewAngle;
    [SerializeField] LayerMask viewMask;

    Transform player;
    float playerVisibleTimer;

    private void OnDrawGizmos()
    {
        Vector3 startPos = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPos;

        foreach(Transform waypoint in pathHolder)
        {
            Gizmos.DrawCube(waypoint.position, Vector3.one * 0.3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }

        Gizmos.DrawLine(previousPosition, startPos);


        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3[] waypoints = new Vector3[pathHolder.childCount];

        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
        }

        StartCoroutine(followPath(waypoints));

        viewAngle = spotLight.spotAngle;
        originalSpotlightColor = spotLight.color;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    IEnumerator followPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];

        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];

        transform.LookAt(targetWaypoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);

            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];

                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }

            yield return null;
        }
    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 lookDirection = (lookTarget - transform.position).normalized;

        float targetAngle = 90 - Mathf.Atan2(lookDirection.z, lookDirection.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.1f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    bool CanSeePlayer()
    {

        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleBetweenGuardAndPlayer < viewAngle / 2)
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask)) return true;
            }
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanSeePlayer())
        {
            // spotLight.color = Color.red;
            playerVisibleTimer += Time.deltaTime;
        } else
        {
            // spotLight.color = originalSpotlightColor;
            playerVisibleTimer -= Time.deltaTime;
        }

        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        spotLight.color = Color.Lerp(originalSpotlightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);

        if (playerVisibleTimer >= timeToSpotPlayer)
        {
            if (OnGuardSpottedPlayer != null)
            {
                OnGuardSpottedPlayer();
            }
        }
    }
}
