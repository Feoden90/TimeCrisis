using UnityEngine;
using System.Collections;

//class used to move a target Transform on a given spline;
public class TransformMover : MonoBehaviour {

	public Transform targetTransform;

	public SplineData movementPattern;

	public float speed;

	public PathMode pathMode;

	public bool autoStart;
	public bool preventRotation;

	//if autostart (e.g. spawned enemies) then start coroutine ASAP;
	void Start(){
		if (autoStart) {
			StartCoroutine(MoveTransform());
		}
	}

	//routine that moves the transform along the spline with a given behaviour;
	public IEnumerator MoveTransform (){
		//setting initial data;
		float param = 0f;
		speed = Mathf.Abs (speed);
		//if spline is used in reverse, then start at the end and proceed backwards;
		if (pathMode == PathMode.ReverseNormal) {
			param = 1f;
			speed *= -1;
		}
		SetTransformAt (param);
		bool running = true;
		//the loop cycle;
		while (running) {
			//preventing errors in case there is no transform attached;
			if (targetTransform == null) {
				break;
			}
			//setting the position and rotation of the transform;
			SetTransformAt(param);
			//updating the parameter as time flows; 
			param += Time.deltaTime * speed;
			//checking if the transform has reached the end of the spline;
			switch(pathMode){
				//normal mode: starts at 0, ends at 1;
			case PathMode.Normal:
				if (!(param < 1f)){
					running = false;
				}
				break;
				//reverse normal mode: starts at 1, ends at 0;
			case PathMode.ReverseNormal:
				if (!(param > 0f)){
					running = false;
				}
				break;
				//loop mode: goes from 0 to 1 and restarts;
			case PathMode.Loop:
				if (!(param < 1f)){
					param = 0;
				}
				break;
				//reverse loop mode: goes from 0 to 1, then from 1 to 0 and repeats;
			case PathMode.ReverseLoop:
				if((param < 0f) || (param > 1f)){
					param = Mathf.Clamp01(param);
					speed *= -1;
				}
				break;
			}

			yield return null;
		}
		//for pracautions against lag, sets the transform on the last point;
		SetTransformAt (1);
		if (pathMode == PathMode.ReverseNormal) {
			SetTransformAt(0);
		}
	}

	//the actual method that manually sets the transform on the spline;
	public void SetTransformAt(float value){
		float t = Mathf.Clamp01 (value);
		targetTransform.position = movementPattern.GetSplinePointAt (t);
		if (!preventRotation)
			targetTransform.rotation = Quaternion.LookRotation(movementPattern.GetSplineVelocityAt (t));
	}
}
