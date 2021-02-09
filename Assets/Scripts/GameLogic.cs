using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour 
{
	private static int NumberOfPlayers = 0;
	
	public ScoreKeeper MyScoreKeeper;
	public List<AudioClip> WinSounds;
	public List<AudioClip> LoseSounds;
	public Text WinText;
	public Text LoseText;
	public PauseButton MyPauseButton;
	public Dealer MyDealer;

	private bool IsGameOver = false;
	private bool IsReversed = false;
	private bool IsPaused = false;
	private bool IsWaitingForAceSelction = false;
    private bool IsPlayerOut = false;

	static int CurrentScore = 0;

	static float AIBaseSpeedMax = 15.0f;
	static float AIBaseSpeedMin = 4.0f;
	static float AISpeedDivider = 2.0f;
	static float AISpeedMin = 0.25f;

	void Start()
	{
		//Init player prefs
		if (PlayerPrefs.HasKey ("PlayerCount"))
			SetNumberOfPlayers (PlayerPrefs.GetInt ("PlayerCount"));
		else
			SetNumberOfPlayers (2); //default

		if (PlayerPrefs.HasKey ("ComputerSpeed"))
			SetAISpeedDivider (PlayerPrefs.GetFloat ("ComputerSpeed"));
		else
			SetAISpeedDivider (2.0f); //default
	}

	public void SaveSettings()
	{
		PlayerPrefs.SetInt ("PlayerCount", NumberOfPlayers);
		PlayerPrefs.SetFloat ("ComputerSpeed", AISpeedDivider);
		PlayerPrefs.Save ();
	}

	public float GetAITurnLength()
	{
		float _turnLength = Random.Range (Mathf.Max (AISpeedMin, AIBaseSpeedMin / AISpeedDivider), 
		                                  AIBaseSpeedMax / AISpeedDivider);
		return _turnLength;
	}

    public float GetCardSpeedMultiplier()
    {
        if (AISpeedDivider >= 8.0f)
            return 1.0f;
        else if (AISpeedDivider >= 6f)
            return 2.0f;
        else
            return 3.0f;
    }

    public float GetAISpeedDivider()
    {
        return AISpeedDivider;
    }

	public void SetAISpeedDivider(float _divider)
	{
		AISpeedDivider = _divider;
	}

	public int GetCurrentScore()
	{
		return CurrentScore;
	}

	public bool CanPlayCard(Card card)
	{
		if (card.GetStandardValue() == 1) //if ace test if 1 can be added
			return (CurrentScore + 1 <= 99);
		return (CurrentScore + card.GetValueIn99 () <= 99);
	}

	public void AddScore(int points)
	{
		CurrentScore += points;
		if (CurrentScore <= 88)
			Card.SetCurrentAceValue (11); //computer always plays Ace as 11 if possible
		else
			Card.SetCurrentAceValue (1);
		MyScoreKeeper.UpdateScoreText ();
	}

	public void ResetScore()
	{
		CurrentScore = 0;
	}

	public void EndGame(bool isWinner)
	{
		MyPauseButton.SetImageToPlay ();
		IsPaused = true;
		IsGameOver = true;
		if (isWinner) {
			Invoke ("SetUpWin", 2.0f);
		} else {
			Invoke ("SetUpLose", 2.0f);
		}
	}

	public void SetUpSoftEnd()
	{
		MyPauseButton.SetImageToPlay ();
        IsPlayerOut = true;
	}

	public void SetUpWin()
	{
		PlayWinSound ();
		WinText.color = new Color(WinText.color.r, WinText.color.g,
		                          WinText.color.b, 255);
	}

	public void SetUpLose()
	{
		PlayLoseSound();
		LoseText.color = new Color(LoseText.color.r, LoseText.color.g,
		                           LoseText.color.b, 255);
	}

	public bool GetIsGameOver()
	{
		return IsGameOver;
	}

	public void SetIsGameOver(bool value)
	{
		IsGameOver = value;
	}

	public bool GetIsReversed()
	{
		return IsReversed;
	}

	public void ReverseOrder()
	{
		IsReversed = !IsReversed;
	}

	public bool GetIsPaused()
	{
		return IsPaused;
	}

	public void SetIsPaused(bool _isPaused)
	{
		IsPaused = _isPaused;
	}

	public void PlayWinSound()
	{
		AudioSource.PlayClipAtPoint (WinSounds [Random.Range (0, WinSounds.Count)], transform.position);
	}

	public void PlayLoseSound()
	{
		AudioSource.PlayClipAtPoint (LoseSounds [Random.Range (0, LoseSounds.Count)], transform.position);
	}

	public int GetNumberOfPlayers()
	{
		return NumberOfPlayers;
	}
	
	public void SetNumberOfPlayers(int playerCount)
	{
		NumberOfPlayers = playerCount;
	}

	public bool IsPlayAceAs11Valid()
	{
		return CurrentScore < 89;
	}

	public bool GetIsWaitingForAceSelection()
	{
		return IsWaitingForAceSelction;
	}

	public void SetIsWaitingForAceSelection(bool value)
	{
		RectTransform _aceSelectionRectTransform = GameObject.Find ("AceSelecter").GetComponent<RectTransform> ();
		IsWaitingForAceSelction = value;
		if (!value) {
			IsPaused = false;
			for (int i = 0; i < MyDealer.Players.Count; i++) {
				MyDealer.Players [i].PlaySelectedAce ();
			}
			if (_aceSelectionRectTransform)
				_aceSelectionRectTransform.position = new Vector3(_aceSelectionRectTransform.position.x,
				                                                  _aceSelectionRectTransform.position.y,
				                                                  -10000000);
		} 
		else 
		{
			if (_aceSelectionRectTransform)
				_aceSelectionRectTransform.position = new Vector3(_aceSelectionRectTransform.position.x,
				                                                  _aceSelectionRectTransform.position.y,
				                                                  -1.0f);
		}
	}

	public void SetCurrentAceValue(int value)
	{
		Card.SetCurrentAceValue (value);
		SetIsWaitingForAceSelection (false);
	}

    public bool GetIsPlayerOut()
    {
        return IsPlayerOut;
    }

    public void SetIsPlayerOut(bool isOut)
    {
        IsPlayerOut = isOut;
    }

/*
	float _timeSinceScreenShot = 0;
	int _count = 0;
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
		if (_timeSinceScreenShot < 3.0f)
			_timeSinceScreenShot += Time.deltaTime;
		else {
			Debug.Log ("Taking SS");
			_timeSinceScreenShot = 0.0f;
			_count++;
			Application.CaptureScreenshot("BannerShots/SS-"+_count.ToString()+".png");
		}
	}
*/


}
