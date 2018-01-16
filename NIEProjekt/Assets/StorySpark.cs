using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorySpark : MonoBehaviour {
	public GameObject storyCanvas;

	public static bool quitting = false;

	void OnLevelWasLoaded(){
		quitting = false;
	}

	void onApplicationQuit(){
		quitting = true;
	}

	void OnDestroy(){
		if (!quitting) {
			Debug.Log ("AA");
			Instantiate (storyCanvas);
		}
	}
}
