using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour {
	
    public float speed = 10f;
    public AudioClip gameStartSound;

    private CharacterController controller;
    private float jumpForce = 8f;
    private float gravity = 30f;
    private Vector3 moveDir = Vector3.zero;
	public Light lt;
    private AudioSource source;

	public static bool sparks = true;
	public GameObject sparkPrefab;
	public float lightFade;

    void Start()
    {
		sparks = true;
       controller = gameObject.GetComponent<CharacterController>();
    }

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.PlayOneShot(gameStartSound, 1F);
        
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

		if (Input.GetButton ("Fire1") && Player.sparks) {
			sparks = false;
			lt.intensity *= lightFade;
			Instantiate (sparkPrefab,new Vector3(transform.position.x, transform.position.y-0.7f, transform.position.z), Quaternion.identity);
		}
        if (Input.GetKey("escape"))
            Application.Quit();

    }

    void OnTriggerEnter(Collider col)
    {
		if (col.gameObject.tag == "Finish") {
			Application.Quit ();
		} else if (col.gameObject.tag == "Story") {
			if (!sparks) {
				sparks = true;
				lt.intensity /= lightFade;
			}
			Destroy (col.gameObject);
			Debug.Log ("A");
		}
    }
}
