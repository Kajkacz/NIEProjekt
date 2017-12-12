using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public Transform pathHolder;
	public bool isClosedPath;
	public float waitTime = 0.3f;
	public float speed = 1;
	public float turnSpeed = 90;

	public Light coneOfSight;
	public float ViewDistance;
    public AudioClip found;
    float viewAngle;

	private GameObject player;
    private AudioSource source;
    static float timeStamp;

    void Start() 
	{
		coneOfSight.color = Color.green;
		Vector3[] waypoints = new Vector3[pathHolder.childCount];
		for (int i = 0; i < waypoints.Length; i++)
			waypoints[i] = pathHolder.GetChild(i).position;

		viewAngle = coneOfSight.spotAngle;

		player = GameObject.FindGameObjectWithTag ("Player");
        
		StartCoroutine (followPath (waypoints));
	}

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }


    void Update()
    {
        if (Spotted())
        {
            coneOfSight.color = Color.red;
            if (timeStamp <= Time.time)
            {
                source.PlayOneShot(found, 1F);
                timeStamp = Time.time + found.length;
            }
        }
        else
            coneOfSight.color = Color.green;
	}

	IEnumerator followPath(Vector3[] waypoints)
	{
		transform.position = waypoints [0];
		transform.forward = waypoints [1];
		int targetWaypointIndex = 1;
		bool reverse = false;
		Vector3 targetWaypoint = waypoints[targetWaypointIndex];
		transform.LookAt (targetWaypoint);

		while (true) 
		{
            transform.position = Vector3.MoveTowards (transform.position, targetWaypoint, speed * Time.deltaTime);
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
			float angle = Mathf.MoveTowardsAngle (transform.eulerAngles.y, targetAngle, turnSpeed*Time.deltaTime);
			transform.eulerAngles = Vector3.up * angle;
			yield return null;
		}
	}

	void OnDrawGizmos()
	{
		Vector3 startPosition = pathHolder.GetChild (0).position;
		Vector3 previousPosition = startPosition;
		foreach (Transform waypoint in pathHolder) {
			Gizmos.color = Color.cyan;
			Gizmos.DrawSphere (waypoint.position, .2f);
			Gizmos.DrawLine (previousPosition, waypoint.position);
			previousPosition = waypoint.position;
		}
		if (isClosedPath)
			Gizmos.DrawLine (previousPosition, startPosition);
		Gizmos.color = Color.magenta;
		Gizmos.DrawRay (new Vector3(transform.position.x, 0, transform.position.z), transform.forward * ViewDistance);
	}

	bool Spotted()
	{
		if (Vector3.Distance (transform.position, player.transform.position) > ViewDistance)
			return false;
		if (Vector3.Angle (transform.forward, (player.transform.position-transform.position)) > (viewAngle)/2)
			return false;
		RaycastHit hitInfo;
		if(Physics.Raycast (transform.position + transform.up, (player.transform.position - transform.position), out hitInfo, ViewDistance))
			if(hitInfo.collider.tag == "Player")
				return true;
		return false;
	}

}
