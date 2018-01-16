using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spark : MonoBehaviour {
	public int waitTime;
	Light light;
	// Use this for initialization
	void Start () {
		light = GetComponent<Light> ();
		StartCoroutine (waitFadeAndDestroy(waitTime, light));
	}

	IEnumerator waitFadeAndDestroy(int n, Light lt)
	{
		yield return new WaitForSeconds (n-1);
		yield return StartCoroutine(fadeLight (lt));
		Destroy (gameObject);
	}
	IEnumerator fadeLight(Light lt){
		for (float t = 0.0f; t < 1f; t += Time.deltaTime) {
			lt.intensity*=0.9f;
			yield return null;
		}
		yield return null;
	}
}
