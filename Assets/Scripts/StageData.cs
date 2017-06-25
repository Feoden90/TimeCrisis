using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageData : MonoBehaviour {

	//this is the list of the spawners; automatically filled by them;
	public List<EnemySpawner> spawners;

	//routine launched by GameController to know if all enemies are spawned and killed;
	public IEnumerator CheckIfStageClear(){
		bool clear = false;
		while (!clear) {
			//sets clear to true, then if any enemy is still alive puts it to false;
			clear = true;
			for (int i = 0; i < spawners.Count; i++){
				//if the enemy i has yet to spawn, then false;
				if (spawners[i].waitingForSpawn){
					clear = false;
				} else{
					//if enemy i still alive, then false;
					if (spawners[i].spawnedObject != null){
						clear = false;
					}
				}
			}
			yield return null;
		}
		yield return null;
	}

	//command launched by GameController to spawn all registered enemies;
	public void SpawnAllTargets(){
		foreach (var spawner in spawners) {
			spawner.Spawn();
		}
	}

}