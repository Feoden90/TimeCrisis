using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//generic class to handle a spline made of cubic bezier curves;
public class SplineData : MonoBehaviour {

	//this is where control points are stored (local transform);
	public Vector3[] localControlPoints;

	//if you want to fix the start or the end of the spline in space, set these, otherwise leave null;
	public Transform fixedStartPosition;
	public Transform fixedFinalPosition;

	//method invoked by unity when the component is reset on the inspector;
	public void Reset(){
		localControlPoints = new Vector3[]{Vector3.zero,new Vector3(2,0,0), new Vector3(2,0,1), new Vector3(0,0,1)};
	}

	//method used to access a certain point on the spline with value € [0,1];
	public Vector3 GetSplinePointAt(float value){
		float t = Mathf.Clamp01 (value);
		if (t == 1f) {
			return GetBezierPointAt (GetBezierCurvesCount () - 1, 1);
		}
		int curve = (int)(t * GetBezierCurvesCount ());
		float q = Mathf.Clamp01 (t * GetBezierCurvesCount () - curve);
		return GetBezierPointAt (curve, q);
	}

	//method used to access velocity (tangent) at a certain point on the spline with value € [0,1];
	public Vector3 GetSplineVelocityAt(float value){
		float t = Mathf.Clamp01 (value);
		if (t == 1f) {
			return GetBezierVelocityAt (GetBezierCurvesCount () - 1, 1);
		}
		int curve = (int)(t * GetBezierCurvesCount ());
		float q = Mathf.Clamp01 (t * GetBezierCurvesCount () - curve);
		return GetBezierVelocityAt (curve, q);
	}
	
	//private method to access velocity (tangent) at a certain point on a given bezier curve with value € [0,1];
	private Vector3 GetBezierVelocityAt(int curve, float value){
		float t = Mathf.Clamp01 (value);
		float u = 1 - t;
		Vector3 P0 = GetControlPoint(3 * curve + 0);
		Vector3 P1 = GetControlPoint(3 * curve + 1);
		Vector3 P2 = GetControlPoint(3 * curve + 2);
		Vector3 P3 = GetControlPoint(3 * curve + 3);
		return 3 * u * u * (P1 - P0) + 6 * u * t * (P2 - P1) + 3 * t * t * (P3 - P2);
	}
	
	//private method to access a certain point on a given bezier curve with value € [0,1];
	private Vector3 GetBezierPointAt(int curve, float value){
		float t = Mathf.Clamp01 (value);
		float u = 1 - t;
		Vector3 P0 = GetControlPoint(3 * curve + 0);
		Vector3 P1 = GetControlPoint(3 * curve + 1);
		Vector3 P2 = GetControlPoint(3 * curve + 2);
		Vector3 P3 = GetControlPoint(3 * curve + 3);
		return u * u * u * P0 + 3 * u * u * t * P1 + 3 * u * t * t * P2 + t * t * t * P3;
	}

	//method used to know the number of curves in the spline;
	public int GetBezierCurvesCount(){
		return (GetNumberOfControlPoints())/ 3;
	}

	//method used to get the lenght of the control points vector (used but not necessary);
	public int GetNumberOfControlPoints(){
		return localControlPoints.Length;
	}

	//method used to access the WORLD position of a control point;
	public Vector3 GetControlPoint(int index){
		return transform.TransformPoint (localControlPoints [index]);
	}

	//method used to set the WORLD position of a control point;
	public void SetControlPoint(int index, Vector3 value){
		localControlPoints [index] = transform.InverseTransformPoint (value);
	}

	//method used to resize the number of points (adding a cubic bezier = 3 points);
	public void AddBezierCurve(){
		Array.Resize (ref localControlPoints, localControlPoints.Length + 3);
	}
	
	//method used to resize the number of points (removing a cubic bezier = 3 points);
	public void RemoveBezierCurve(){
		if (localControlPoints.Length < 4) {
			return;
		}
		Array.Resize (ref localControlPoints, localControlPoints.Length - 3);
	}
	
}
