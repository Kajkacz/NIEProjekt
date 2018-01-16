using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour {

	public GameObject textCanvas;
	public string nextScene;

	void OnTriggerEnter(Collider col){
		if (col.gameObject.tag == "Player") {
			textCanvas.SetActive (true);
		}
	}

	void OnTriggerExit(Collider col){
			textCanvas.SetActive (false);
	}

	void Update(){
		if (textCanvas.activeSelf)
		if(Input.GetButtonDown("Jump")){
				StorySpark.quitting = true;
				SceneManager.LoadScene(nextScene);
			}
	}
}
