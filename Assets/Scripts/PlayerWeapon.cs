using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerWeapon : MonoBehaviour {

	public AudioClip weaponSound;

	public AudioClip reloadSound;

	public AudioClip weaponReloadSound;

	public float cooldownTime;
	
	public int ammoMax;

	public bool isInputDisabled;

	public GameObject ammoText;

	private int ammoLeft;

	void Start(){
		StartCoroutine (WeaponRoutine ());
	}

	//routine that enables the player to shoot;
	IEnumerator WeaponRoutine(){
		//setting data;
		ammoLeft = ammoMax;
		//infinte loop (should be avoided....)
		while (true) {
			//if input is disabled wait for next frame;
			if (isInputDisabled){
				yield return null;
				continue;
			}
			//if input is enabled and mouse button is pressed then shoot
			if (Input.GetMouseButtonDown(0)){
				ammoLeft--;
				UpdateAmmoText();
				CustomManager.AudioManager.GetInstance().PlaySoundEffect(weaponSound,1f,1f);
				//if there is no ammo left, reload weapon;
				if (ammoLeft == 0){
					CustomManager.AudioManager.GetInstance().PlaySoundEffect(reloadSound,1f,1f);
					yield return new WaitForSeconds(cooldownTime);
					ammoLeft = ammoMax;
					UpdateAmmoText();
					CustomManager.AudioManager.GetInstance().PlaySoundEffect(weaponReloadSound,1f,1f);
				}
			}
			//wait next update cycle;
			yield return null;

		}

	}

	//update the UI text to display ammo;
	void UpdateAmmoText(){
		foreach (var text in ammoText.GetComponentsInChildren<Text>()) {
			text.text = "Ammo = " + ammoLeft.ToString();
		}
	}

}
