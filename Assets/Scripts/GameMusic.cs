using UnityEngine;
using System.Collections;

public class GameMusic : MonoBehaviour 
{
	private static GameMusic instance = null;
	public AudioClip StartMusic;
	public AudioClip PlayMusic;
	public AudioClip EndMusic;
	
	void Awake()
	{
		if (instance != null)
		{
			Destroy (gameObject);//destroy duplicate
		}
		else
		{
			instance = this;
			GameObject.DontDestroyOnLoad(gameObject);
			SetMusic(Track.Start);
		}
	}
	public enum Track
	{
		Start,
		Play,
		End
	}
	
	public void SetMusic(Track _track)
	{
		if (_track == Track.Start)
			audio.clip = StartMusic;
		else if (_track == Track.Play)
			audio.clip = PlayMusic;
		else if (_track == Track.End)
		    audio.clip = EndMusic;
		audio.loop = true;
		audio.Play();
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	
	
	
}
