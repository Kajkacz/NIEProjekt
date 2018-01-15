using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {

	// Variables
	public Transform player;
	public float smooth = 0.25f;
	public Vector3 offset;
	private Vector3 velocity = Vector3.zero;

	void Start () {
		transform.position=player.position+offset;
	}
	


	// Methods
	void LateUpdate()
	{
		Vector3 pos = player.position+offset;
		Vector3 smoothpos = Vector3.SmoothDamp (transform.position, pos, ref velocity, smooth);
		transform.position = smoothpos;
	}
		
}
