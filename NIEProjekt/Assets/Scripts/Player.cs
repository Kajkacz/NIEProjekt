using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour {

    public float speed = 10f;
    public AudioClip gameplayMusic;

    private CharacterController controller;
    private float jumpForce = 8f;
    private float gravity = 30f;
    private Vector3 moveDir = Vector3.zero;
     

    private AudioSource source;


    void Start()
    {
       controller = gameObject.GetComponent<CharacterController>();
    }

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.PlayOneShot(gameplayMusic, 1F);
        
    }


    void Update () {
        

        if (controller.isGrounded)
        {

          
            moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            moveDir = transform.TransformDirection(moveDir);

            moveDir *= speed;

            if (Input.GetButtonDown("Jump"))
            {
                moveDir.y = jumpForce;
            }
        }
            moveDir.y -= gravity * Time.deltaTime;
            controller.Move(moveDir * Time.deltaTime);


        if (Input.GetKey("escape"))
            Application.Quit();

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Finish")
        {
            Application.Quit();
        }
    }
}
