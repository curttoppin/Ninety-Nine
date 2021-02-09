using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour 
{
	
	void Start()
	{
	}
	
	public void LoadLevel(string name)
	{
		//Debug.Log("Level load requested for: " + name);
		Application.LoadLevel(name);
	}
	
	public void QuitRequested()
	{
		//Debug.Log("Quit requested.");
		Application.Quit();
	}
	
	public void LoadNextLevel()
	{
		//Debug.Log("Level load requested for: " + (Application.loadedLevel + (int)1));
		Application.LoadLevel (Application.loadedLevel + 1);
	}

	public void ReloadLevel()
	{
		//Debug.Log("Level reload requested.");
		Application.LoadLevel (Application.loadedLevelName);
	}
}
