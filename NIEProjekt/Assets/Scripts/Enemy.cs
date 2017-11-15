using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public Transform pathHolder;
	public bool isClosedPath;
	public float waitTime = 0.3f;
	public float speed = 1;
	public float turnSpeed = 90;

	void Start() 
	{
		Vector3[] waypoints = new Vector3[pathHolder.childCount];
		for (int i = 0; i < waypoints.Length; i++)
			waypoints[i] = pathHolder.GetChild(i).position;

		StartCoroutine (followPath (waypoints));
	}

	IEnumerator followPath(Vector3[] waypoints)
	{
		transform.position = waypoints [0];
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
	}

}
