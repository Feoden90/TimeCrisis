using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerLifeController : MonoBehaviour {

	public int maxHealthPoints;

	public GameObject healtBar;
	
	private int healthPoints;

	void Start(){
		healthPoints = maxHealthPoints;
		SetHpGUI ();
	}

	//method used by enemies to deal damage (can heal too);
	public void DealDamage (int damage){
		//if player dodging then no damage;
		if (GetComponent<PlayerDodge> ().isPlayerDown) {
			return;
		}
		healthPoints -= damage;
		//if dead then game over;
		if (healthPoints < 0) {
			healthPoints = 0;
			GameController.gameController.EndLevel (false);
		}
		SetHpGUI ();
	}

	//method to update the HP interface;
	private void SetHpGUI(){
		foreach (var text in healtBar.GetComponentsInChildren<Text>()) {
			text.text = "HP = " + healthPoints.ToString();
		}
	}

}
