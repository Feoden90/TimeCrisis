using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutomatedShooter : MonoBehaviour {

	public int damage;

	public float animationTime;

	public AudioClip sound;

	public List<float> splineShootingPositions;

	private Renderer renderComponent;

	private TransformMover moverComponent;

	//for the first time, start too is a routine...
	IEnumerator Start () {
		//sort the shooting pattern in order to launch just one routine;
		splineShootingPositions.Sort ();
		//setting the variables;
		moverComponent = GetComponent<TransformMover> ();
		bool repeated = true;//(moverComponent.pathMode != PathMode.Normal);
		//repeat the routine every spline cycle (if movement is repeated then launch a coroutine every time it resets);
		while (repeated) {
			StartCoroutine (ShootRoutine ());
			//wait for the time it takes to complete the spline;
			yield return new WaitForSeconds (1f/moverComponent.speed);
		}
	}

	//routine used by enemy to shoot;
	IEnumerator ShootRoutine(){
		float previousTime = 0f;
		int index = 0;
		//substantially waits for the next shot and then shoots;
		while (index < splineShootingPositions.Count) {
			//wait for the time difference between two shots;
			yield return new WaitForSeconds((splineShootingPositions[index] - previousTime)/moverComponent.speed);
			//then execute shooting animation;
			StartCoroutine(Shoot());
			previousTime = splineShootingPositions[index];
			index++;
		}
	}

	//routine that animates the enemy shot;
	IEnumerator Shoot(){
		renderComponent = GetComponent<Renderer> ();
		Color startColor = renderComponent.material.color;
		float time = 0;
		//substantially changes enemy color to white, so you know you are being shot;
		while (time < animationTime) {
			renderComponent.material.color = Color.Lerp(startColor,Color.white,time/animationTime);
			time += Time.deltaTime;
			yield return null;
		}
		renderComponent.material.color = Color.white;
		CustomManager.AudioManager.GetInstance ().PlaySoundEffect (sound, 1f, 1f);
		//if you didn't kill the enemy, then you receive damage;
		GameController.gameController.GetComponent<PlayerLifeController> ().DealDamage (damage);
		renderComponent.material.color = startColor;
	}

}
