using UnityEngine;
using System.Collections;

public class AnimateWater : MonoBehaviour {

	public Vector2 offsetSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Renderer> ().material.mainTextureOffset = offsetSpeed * Time.time;
	}
}
