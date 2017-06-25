using UnityEngine;
using System.Collections;

namespace CustomManager
{
	//this class gets attached to an AudioSource by the AudioManager;
	public class AudioObject : MonoBehaviour
	{
		//the audio source component of the object
		private AudioSource audioPlayer;
		//private variables to keep track of the status;
		private bool hasStartedPlaying;
		private bool isPaused;

		//if instantiated at runtime, start is executed after play;
		void Awake ()
		{
			//initialization
			hasStartedPlaying = false;
			isPaused = false;
			audioPlayer = GetComponent<AudioSource> ();
		}

		//method invoked by audiomanager to play a sound;
		public void PlayClip (AudioClip clip, Vector3 position, float volume, float pitch, bool loop)
		{
			//assigning parameters
			audioPlayer.clip = clip;
			audioPlayer.volume = volume;
			audioPlayer.pitch = pitch;
			audioPlayer.loop = loop;
			transform.position = position;
			hasStartedPlaying = true;
			//executing play command;
			audioPlayer.Play ();
		}

		//method invoked by audiomanager to pause a sound;
		public void PauseClip (bool paused)
		{
			if (paused) {
				isPaused = true;
				audioPlayer.Pause ();
			} else {
				isPaused = false;
				audioPlayer.UnPause ();
			}
		}

		//method invoked by audiomanager to stop a sound;
		public void StopPlaying ()
		{
			audioPlayer.Stop ();
			ReleaseAudioObject ();
		}

		//routine invoked by audiomaneger to smoothly change volume;
		public IEnumerator FadeSound ( float fadeTime, float endVolume)
		{
			float startVolume = GetComponent<AudioSource> ().volume;
			float time = 0;
			while (time < fadeTime) {
				time += Time.deltaTime;
				//Linear interpolation for volume;
				GetComponent<AudioSource>().volume = Mathf.Lerp(startVolume,endVolume,time);
				yield return null;
			}
			//assigning final value to avoid unwanted fluctuations;
			GetComponent<AudioSource> ().volume = endVolume;
		}

		//routine that releases clips after fading to zero volume;
		public IEnumerator FadeOut (float fadetime){
			//start a fading routine and wait till end;
			yield return StartCoroutine( FadeSound (fadetime, 0));
			//clear the audiosource and flag this free;
			StopPlaying ();
		}

		//need only to check if a sound has stopped playing;
		void Update ()
		{
			//check if audio has started
			if (hasStartedPlaying) {
				//check if audio is not playing anymore (& is not paused)
				if (!isPaused && !audioPlayer.isPlaying && !audioPlayer.loop) {
					//if the object purpose is over, then reset its status;
					ReleaseAudioObject ();
				}
			}
		}

		//method called only after the audio has stopped playing;
		private void ReleaseAudioObject ()
		{
			//reset status;
			hasStartedPlaying = false;
			isPaused = false;
			//tell the manager to flag it as free;
			AudioManager.GetInstance ().ReleaseAudioChannel (this.gameObject.GetInstanceID ().ToString ());
		}

	}
}