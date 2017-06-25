using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SplineData))]
public class SplineDataEditor : Editor {

	//editing the appearance of splines in scene view;
	void OnSceneGUI(){

		//first draw the polygonal line between the control points;
		for (int i = 0; i < GetSpline().GetNumberOfControlPoints() - 1; i++){
			Handles.DrawLine(GetSpline().GetControlPoint(i), GetSpline().GetControlPoint(i+1));
		}

		//then draw the spline, made of different beziers curves;
		for (int i = 0; i < GetSpline().GetBezierCurvesCount(); i++) {
			Vector3 startPos = GetSpline().GetControlPoint(3*i + 0);
			Vector3 startTan = GetSpline().GetControlPoint(3*i + 1);
			Vector3 endTan = GetSpline().GetControlPoint(3*i + 2);
			Vector3 endPos = GetSpline().GetControlPoint(3*i + 3);
			//luckily, unity already does this for us!
			Handles.DrawBezier (startPos, endPos, startTan, endTan, Color.red, null, 3f);
		}

		//then draw the transform handles for each of the control points (unless they are fixed of course);
		for (int i = 0; i < GetSpline().GetNumberOfControlPoints(); i++) {
			if (GetSpline().fixedStartPosition != null)
				if (i == 0) continue;
			if (GetSpline().fixedFinalPosition != null)
				if (i == GetSpline().GetNumberOfControlPoints() -1) continue;
			//method that draws the transform handle;
			HandleSplinePointTransform(i);
		}
	}

	//editing the appearance of the script in the inspector;
	public override void OnInspectorGUI(){

		//first field: the fixed transform fields;
		GetSpline ().fixedStartPosition = EditorGUILayout.ObjectField ("fixedStartPosition", GetSpline ().fixedStartPosition, typeof(Transform), true) as Transform;
		GetSpline ().fixedFinalPosition = EditorGUILayout.ObjectField ("fixedFinalPosition", GetSpline ().fixedFinalPosition, typeof(Transform), true) as Transform;

		//check needed only in case the splinedata vector is renamed;
		if (GetSpline ().GetNumberOfControlPoints() == 0) {
			GetSpline().Reset();
		}
		//if the fixed transform has been set, then update the value in the vector;
		if (GetSpline ().fixedStartPosition != null) {
			GetSpline ().SetControlPoint(0,GetSpline ().fixedStartPosition.position);
		}
		//if the fixed transform has been set, then update the value in the vector;
		if (GetSpline ().fixedFinalPosition != null) {
			GetSpline ().SetControlPoint(GetSpline().GetNumberOfControlPoints() -1,GetSpline ().fixedFinalPosition.position);
		}
		//second field: the control points fields (disabled for fixed points);
		for (int i = 0; i < GetSpline().GetNumberOfControlPoints(); i++) {
			bool editable = true;
			if (((i == 0) && (GetSpline ().fixedStartPosition != null)) || ((i == GetSpline().GetNumberOfControlPoints() -1) && (GetSpline ().fixedFinalPosition != null))){
				editable = false;
			}
			//the method used to draw my custom vector3 field;
			GetSpline().localControlPoints[i] = DrawVector3Element ("P" + i.ToString(), GetSpline().localControlPoints[i], editable);
		}
		//third field: buttons to resize the spline points array (adding or removing a curve);
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("AddCurve")) {
			GetSpline ().AddBezierCurve ();
		}
		if (GUILayout.Button ("RemoveCurve")) {
			GetSpline ().RemoveBezierCurve ();
		}
		EditorGUILayout.EndHorizontal ();
		
	}

	//method used to draw transform handles in scene view (handling undo, refresh, world vs local transform);
	private void HandleSplinePointTransform(int index){
		EditorGUI.BeginChangeCheck ();
		Vector3 p0 = Handles.DoPositionHandle(GetSpline().GetControlPoint(index),Quaternion.identity);
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject(target,"Move Spline Point");
			EditorUtility.SetDirty(target);
			GetSpline().SetControlPoint(index,p0);
		}
	}

	//method used to draw Vector3 fields in the inspector (I wanted to compact the field size);
	private Vector3 DrawVector3Element(string name, Vector3 value, bool active = true){
		Vector3 newvalue;
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label (name + ":");
		if (!active)
			GUI.enabled = false;
		GUILayout.Label ("X");
		EditorGUI.BeginChangeCheck ();
		float xval = EditorGUILayout.FloatField (value.x);
		GUILayout.Label ("Y");
		float yval = EditorGUILayout.FloatField (value.y);
		GUILayout.Label ("Z");
		float zval = EditorGUILayout.FloatField (value.z);
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (target, "Set Spline Point");
			EditorUtility.SetDirty (target);
		}
		newvalue = new Vector3 (xval, yval, zval);
		if (!active)
			GUI.enabled = true;
		EditorGUILayout.EndHorizontal ();
		return newvalue;
	}

	//private method used for renaming ((SplineData)target) to GetSpline() and improving readability;
	private SplineData GetSpline(){
		return target as SplineData;
	}


}
