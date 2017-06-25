using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public static GameController gameController;

	public StageData firstStage;

	public TransformMover player;

	public GameObject ActionText;

	public GameObject GameOverText;

	public GameObject WinStageText;

	public GameObject RetryButtons;

	public AudioClip ActionSound;

	private bool gameEnded;

	void Awake(){
		gameController = this;
	}

	// Begin as soon as possible with the game flow;
	void Start () {
		gameEnded = false;
		StartCoroutine (ExecuteGameFlow ());
	}
	
	public void EndLevel(bool win){
		if (gameEnded) {
			return;
		}
		Time.timeScale = 0;
		gameEnded = true;
		if (win)
			WinStageText.SetActive (true);
		else
			GameOverText.SetActive (true);
		RetryButtons.SetActive (true);
	}

	//Button Command (TODO: doesn't need to be here);
	public void Retry(){
		Time.timeScale = 1;
		Application.LoadLevel (Application.loadedLevel);
	}

	//Button Command (TODO: doesn't need to be here);
	public void Quit(){
		Application.Quit ();
	}

	//used to enable / disable input while moving;
	private void EnableInput(bool enabled){
		GetComponent<PlayerWeapon> ().isInputDisabled = !enabled;
		GetComponent<PlayerDodge> ().isInputDisabled = !enabled;
	}

	//routine used to call the action sound;
	IEnumerator StartStageAction(){
		//play sound "ACTION"
		CustomManager.AudioManager.GetInstance ().PlaySoundEffect (ActionSound, 1f, 1f);
		ActionText.SetActive (true);
		yield return new WaitForSeconds (1f);
		ActionText.SetActive (false);
	}

	//the actual flow of the game, yielding to other coroutines;
	IEnumerator ExecuteGameFlow(){
		//setting initial data;
		StageData currentStage = firstStage;
		bool isThereAnotherStage = true;
		//setting player position in case it's somewhere else;
		player.movementPattern = currentStage.GetComponent<SplineData> ();
		player.SetTransformAt (0);

		//game loop;
		while (isThereAnotherStage) {
			//starts on 1st stage;
			currentStage.SpawnAllTargets();
			EnableInput(true);
			//wait for action message;
			if (currentStage.spawners.Count > 0)
				yield return StartCoroutine( StartStageAction());
			//now wait till all targets are dead;
			yield return StartCoroutine (currentStage.CheckIfStageClear ());
			//TODO:stop if player dead;
			//after killing all sets camera movement;
			player.movementPattern = currentStage.GetComponent<SplineData> ();
			//disable input while moving;
			EnableInput(false);
			//check if there is another stage!
			if (player.movementPattern.fixedFinalPosition == null) {
				isThereAnotherStage = false;
				EndLevel(true);
			} else {
				//coroutine to move the camera to the next stage;
				yield return StartCoroutine (player.MoveTransform ());
				//after moving switch to new stage;
				currentStage = player.movementPattern.fixedFinalPosition.GetComponent<StageData> ();
			}
		}
		yield return null;
	}
	
}
