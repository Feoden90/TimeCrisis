using UnityEngine;
using System.Collections;

public class PlayerDodge : MonoBehaviour {
	
	public bool isPlayerDown;

	public TransformMover dodgingCamera;

	public bool isInputDisabled;

	void Start () {
		StartCoroutine (MoveCamera ());
	}

	//routine to update the dodging of the camera;
	IEnumerator MoveCamera(){
		while (true) {
			//if the player is up and is dodging, become immortal and start moving camera;
			if (isDodgeKeyPressed() && !isPlayerDown){
				isPlayerDown = true;
				dodgingCamera.pathMode = PathMode.Normal;
				//can't come back until it has ended moving;
				yield return StartCoroutine(dodgingCamera.MoveTransform());
			}
			//if the player is down and releases the key (or input is disabled), camera comes back;
			if (!isDodgeKeyPressed() && isPlayerDown){
				dodgingCamera.pathMode = PathMode.ReverseNormal;
				//can't dodge again until it returns up;
				yield return StartCoroutine(dodgingCamera.MoveTransform());
				isPlayerDown = false;
			}
			yield return null;
		}
	}

	private bool isDodgeKeyPressed(){
		//if input is disabled, then no input is sent to the routine;
		if (isInputDisabled) {
			return false;
		}

		if (Input.GetMouseButton (1)) {
			return true;
		}
		if (Input.GetKey (KeyCode.Space)) {
			return true;
		}
		return false;
	}

}
