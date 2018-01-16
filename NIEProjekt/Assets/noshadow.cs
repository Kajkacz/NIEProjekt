using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noshadow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

		GetComponent<Renderer>().receiveShadows = true;
	}

}
