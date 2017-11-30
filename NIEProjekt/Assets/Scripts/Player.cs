using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLayerMovement : MonoBehaviour {

    private float speed = 10f;
    private Vector3 moveDir = Vector3.zero;
	// Use this for initialization
	void Start () {
		
	}

	void Update () {
        CharacterController controller = gameObject.GetComponent<CharacterController>();

        if (controller.isGrounded)
        {
            moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical");

            moveDir = transform.TransformDirection(moveDir);


        }
	}
}
