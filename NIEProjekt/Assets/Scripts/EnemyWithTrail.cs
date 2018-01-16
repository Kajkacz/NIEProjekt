using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyWithTrail : MonoBehaviour
{
	
    public Transform pathHolder;
    public bool isClosedPath;
	public float waitTime;
	public float volumeUp;

    public Light coneOfSight;
    public float ViewDistance;
    public AudioClip found;
    float viewAngle;


    private GameObject player;
	private Vector3[] waypoints;
	private int destinationPoint;
	private NavMeshAgent agent;
	private bool reverse = false;
    private ParticleSystem particle;
    private AudioSource source;
    static float audioTimeStamp;
	private enum state {PATROL, ATTACK};
	private state currentState;
    void Start()
    {
		currentState = state.PATROL;
        coneOfSight.color = Color.green;
		viewAngle = coneOfSight.spotAngle;

        waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
            waypoints[i] = pathHolder.GetChild(i).position;

        player = GameObject.FindGameObjectWithTag("Player");
        particle = GetComponent<ParticleSystem>();

		agent = GetComponent<NavMeshAgent>();
		agent.autoBraking = false;

		transform.position = waypoints[0];
		transform.forward = waypoints[1];
		destinationPoint = 1;

    }

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }


    void Update()
	{
		switch(currentState)
		{
		case state.PATROL:
			{
				if (Spotted ()) {
					currentState = state.ATTACK;
					agent.destination = player.transform.position;
					agent.speed *= 5;
					source.volume *= volumeUp;
					source.maxDistance *= volumeUp;
					source.PlayOneShot (found, 1F);
					coneOfSight.color = Color.red;
					if (audioTimeStamp <= Time.time) {
						//source.PlayOneShot (found, 1F);
						audioTimeStamp = Time.time + found.length;
					}
				} else if (!agent.pathPending && agent.remainingDistance < 0.5f) {
					patrol ();
				}
				break;
			}
		case state.ATTACK:
			{
				agent.destination = player.transform.position;
				if (!agent.pathPending)
				{
					if (agent.remainingDistance <= agent.stoppingDistance)
					{
						StorySpark.quitting = true;
							SceneManager.LoadScene (SceneManager.GetActiveScene().name);

					}
				}
				break;
			}
		/*if (Spotted ()) {
			coneOfSight.color = Color.red;
			if (audioTimeStamp <= Time.time) {
				source.PlayOneShot (found, 1F);
				audioTimeStamp = Time.time + found.length + followTime;
			}
			agent.destination = player.transform.position;
			agent.speed = oldSpeed * 10;
		} else if (!agent.pathPending && agent.remainingDistance < 0.5f) {
			patrol ();
			agent.speed = oldSpeed;
			coneOfSight.color = Color.green;
		} /*else {
			agent.speed = oldSpeed;
			coneOfSight.color = Color.green;*/

		}
    }

	void patrol(){
		agent.destination = waypoints [destinationPoint];
		if (isClosedPath)
			destinationPoint = (destinationPoint + 1) % waypoints.Length;
		else if (destinationPoint == waypoints.Length - 1 || destinationPoint == 0)
			reverse = !reverse;
		if (!reverse)
			destinationPoint++;
		else
			destinationPoint--;
	}		

    void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(waypoint.position, .2f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        if (isClosedPath)
            Gizmos.DrawLine(previousPosition, startPosition);
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(new Vector3(transform.position.x, 0, transform.position.z), transform.forward * ViewDistance);
    }

    bool Spotted()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > ViewDistance)
            return false;
        if (Vector3.Angle(transform.forward, (player.transform.position - transform.position)) > (viewAngle) / 2)
            return false;
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + transform.up, (player.transform.position - transform.position), out hitInfo, ViewDistance))
            if (hitInfo.collider.tag == "Player")
                return true;
        return false;
    }

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Sunlight")
		{
			if (currentState == state.ATTACK) {
				agent.destination = waypoints [destinationPoint];
			}
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.gameObject.tag == "Sunlight")
		{
			if (currentState == state.ATTACK) {
				agent.destination = waypoints [destinationPoint];
				agent.speed /= 5;
				source.volume /= volumeUp;
				source.maxDistance /= volumeUp;
				currentState = state.PATROL;
			}
		}
	}

	IEnumerator followPath(Vector3[] waypoints, int targetWaypointIndex)
	{
		transform.position = waypoints [0];
		transform.forward = waypoints [1];
		bool reverse = false;
		Vector3 targetWaypoint = waypoints[targetWaypointIndex];
		transform.LookAt (targetWaypoint);

		while (true) 
		{
			transform.position = Vector3.MoveTowards (transform.position, targetWaypoint, agent.speed * Time.deltaTime);
			if (transform.position == targetWaypoint) {
				if (isClosedPath)
					targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
				else if (targetWaypointIndex == waypoints.Length - 1 || targetWaypointIndex == 0)
					reverse =! reverse;
				if (!reverse)
					targetWaypointIndex++;
				else
					targetWaypointIndex--;
				targetWaypoint = waypoints [targetWaypointIndex];

				yield return new WaitForSeconds (waitTime);
				yield return StartCoroutine (TurnToWaypoint (targetWaypoint));
			} yield return null;
		} 
	}

	IEnumerator TurnToWaypoint(Vector3 target)
	{
		Vector3 dirToTarget = (target - transform.position).normalized;
		float targetAngle = 90 - Mathf.Atan2 (dirToTarget.z, dirToTarget.x) * Mathf.Rad2Deg;

		while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle))>0.05f) 
		{
			float angle = Mathf.MoveTowardsAngle (transform.eulerAngles.y, targetAngle, agent.angularSpeed*Time.deltaTime);
			transform.eulerAngles = Vector3.up * angle;
			yield return null;
		}
	}


}
