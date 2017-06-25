using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//DEVE ESSERE AGGIUNTA SU UN FIGLIO DI UN GAMEOBJECT CHE HA STAGEDATA TRA LE COMPONENTI
public class EnemySpawner : MonoBehaviour {

	public GameObject spawnPrefab;

	public SplineData targetSpline;

	public float waitTime;

	public PathMode pathMode;

	public float speed;

	public List<float> splineShootingPositions;

	public bool waitingForSpawn { get; private set; }

	public GameObject spawnedObject { get; private set; }

	//if the component is enabled, then it automatically registers on the stageData;
	void OnEnable(){
		GetComponentInParent<StageData> ().spawners.Add (this);
	}

	//dunno if necessary;
	void OnDisable(){
		//GetComponentInParent<StageData> ().spawners.Remove (this);
	}

	//method invoked by stageData to spawn a new enemy;
	public void Spawn(){
		StartCoroutine (DelayedSpawn ());

	}

	//routine that allows to spawn an enemy after a set time;
	IEnumerator DelayedSpawn(){
		waitingForSpawn = true;
		//waiting for the time to spawn;
		yield return new WaitForSeconds(waitTime);
		//spawning the enemy;
		spawnedObject = Instantiate (spawnPrefab);
		spawnedObject.transform.SetParent (transform);
		//setting all the enemy properties (movement and shot behaviour);
		spawnedObject.GetComponent<TransformMover> ().movementPattern = targetSpline;
		spawnedObject.GetComponent<TransformMover> ().pathMode = pathMode;
		spawnedObject.GetComponent<TransformMover> ().speed = speed;
		spawnedObject.GetComponent<AutomatedShooter> ().splineShootingPositions = splineShootingPositions;

		waitingForSpawn = false;

	}
}
