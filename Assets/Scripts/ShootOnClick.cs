using UnityEngine;
using System.Collections;

public class ShootOnClick : MonoBehaviour {

	public AudioClip deathSound;

	//on mouse down makes implicit calculations about clicking with 3D colliders;
	void OnMouseDown(){
		//invoking the audiomanager to play sound;
		CustomManager.AudioManager.GetInstance ().PlaySoundEffect (deathSound, 1f, 1f);
		//TODO: make and play death animation;
		Destroy (gameObject);
	}
}
