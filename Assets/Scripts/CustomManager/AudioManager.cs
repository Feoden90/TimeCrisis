using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CustomManager
{
	public class AudioManager : MonoBehaviour
	{
		//the number of preinstantiated channels;
		public int startingNumber;

		//static instance of the manager;
		private static AudioManager audioManager;

		//pool manager (needed to keep track of audiochannels);
		private PoolManager<string,AudioObject> poolManager;

		//private variables to keep state of the manager;
		private bool pausedManager;
		private string bgmInstanceKey;
		//not used;
		private float bgmVolume;
		private float seVolume;

		void Awake ()
		{
			//instantiating the singleton manager instance;
			SetSingleInstance ();
			//audiomanager initialization;
			pausedManager = false;
			bgmInstanceKey = "";
			//pool manager initialization;
			poolManager = new PoolManager<string,AudioObject> (CreateObjectProperties);
			//preallocating new channels;
			poolManager.AllocateNewObjects (startingNumber);
		}

		//method used to make a singleton class (although not really necessary);
		void SetSingleInstance(){
			//if another audiomanager has already started, destroy this;
			if (audioManager != null) {
				Destroy(gameObject);
				return;
			}
			//if none, set this as the only instance;
			audioManager = this;

		}

		//method invoked by pool manager to setup object properties and receive data;
		private KeyValuePair<string,AudioObject> CreateObjectProperties ()
		{
			//setting object informations;
			int index = poolManager.getObjectList().Count;
			GameObject tmp = new GameObject ("Channel n°" + index.ToString ());
			tmp.transform.SetParent (transform);
			//adding components to the object;
			tmp.AddComponent<AudioSource> ();
			tmp.AddComponent<AudioObject> ();
			//creating an unique key for the pool manager. No need to tell AudioObject what the key is;
			string key = tmp.GetInstanceID ().ToString();
			//returning the customized object to the manager;
			return new KeyValuePair<string, AudioObject>(key,tmp.GetComponent<AudioObject>());
		}

		//static method to actually access the manager;
		public static AudioManager GetInstance ()
		{
			if (audioManager == null) {
				new GameObject("AudioManager",typeof(AudioManager));
			}
			//this is the last instantiated manager and a singleton;
			return audioManager;
		}

		//method to play a sound with no parameters;
		public void PlayAudioClip (AudioClip clip)
		{
			this.PlayAudioClip (clip, Vector3.zero, 1.0f, 1.0f, false);
		}

		//method to play a sound with only position parameter;
		public void PlayAudioClip (AudioClip clip, Vector3 position)
		{
			this.PlayAudioClip (clip, position, 1.0f, 1.0f, false);
		}

		//method to play a sound with position, volume and pitch;
		public void PlayAudioClip (AudioClip clip, Vector3 position, float volume, float pitch)
		{
			this.PlayAudioClip (clip, position, volume, pitch, false);
		}

		//most generic method to play a sound, invoked by all other methods;
		private GameObject PlayAudioClip (AudioClip clip, Vector3 position, float volume, float pitch, bool loop)
		{
			//if manager is paused for whatever reason we should not instantiate new sounds;
			if (pausedManager)
				return null;
			//fetch the next free object, if none create a new one;
			AudioObject channel = poolManager.GetNextFreeObjectInstance ();

			//invoke play method in audioobject;
			channel.PlayClip (clip, position, volume, pitch, loop);
			return channel.gameObject;
		}

		//method to play bg music (if a new is played, the old is stopped);
		public void PlayBackgroundMusic (AudioClip bgm, float volume, float pitch)
		{
			//command unified with the fadeIn command (fadetime = 0);
			FadeInBackGroundMusic (bgm, 0.0f, volume, pitch);
		}

		//method to play a generic 2D sound effect with volume and pitch;
		public void PlaySoundEffect (AudioClip se, float volume, float pitch)
		{
			this.PlayAudioClip (se, Vector3.zero, volume, pitch, false);
		}

		//method to make smooth transition between different bg musics;
		public void FadeInBackGroundMusic (AudioClip bgm, float fadetime, float volume, float pitch){
			//asking the manager to queue the new audioclip (volume = 0);
			GameObject newchannel = this.PlayAudioClip (bgm, Vector3.zero, 0, pitch, true);
			//if it couldn't start, then stop here (paused manager);
			if (newchannel == null)
				return;
			//if bg music was already running, then fade it Out;
			if (bgmInstanceKey != "") {
				StartCoroutine(poolManager.getObjectList()[bgmInstanceKey].FadeOut(fadetime));
			}
			//Fade in the new music;
			StartCoroutine(newchannel.GetComponent<AudioObject>().FadeSound(fadetime,volume));
			bgmInstanceKey = newchannel.GetInstanceID ().ToString();

		}

		//method to pause the manager. If paused, no sound can be instantiated;
		public void PauseSoundManager (bool paused)
		{
			pausedManager = paused;
			foreach (AudioObject channel in poolManager.getObjectList().Values) {
				channel.PauseClip (paused);
			}
		}

		//method to stop the manager; it will free all objects, including bgm;
		public void StopAllSounds ()
		{
			bgmInstanceKey = "";
			foreach (AudioObject channel in poolManager.getObjectList().Values) {
				channel.StopPlaying ();
			}
		}

		//method to release the object instance; invoked by AudioObject when audio is stopped;
		public void ReleaseAudioChannel(string key){
			poolManager.ReleaseObjectInstance (key);
		}

	}
}
