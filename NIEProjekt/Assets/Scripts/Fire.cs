using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour {

	private AudioSource source;
	public AudioClip found;
	public GameObject firePrefab;
	static float audioTimeStamp;

	void Awake()
	{
		source = GetComponent<AudioSource>();
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Player") {
			if (audioTimeStamp <= Time.time) {
				source.PlayOneShot (found, 1F);
				audioTimeStamp = Time.time + found.length;
			}
			if (Input.GetButton ("Fire1") && Player.sparks > 0) {
				Player.sparks--;
				Instantiate (firePrefab, this.transform);
				Destroy (this);
			}
		}	
	}
}
