using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseButton : MonoBehaviour {
	public Sprite PlayImage;
	public Sprite PauseImage;
	Image MyImage;
	Image BackButtonImage;
	GameLogic MyGameLogic;
	LevelManager MyLevelManager;


	// Use this for initialization
	void Start () {
		MyGameLogic = GameObject.Find ("GameLogic").GetComponent<GameLogic> ();
		MyLevelManager = GameObject.Find ("LevelManager").GetComponent<LevelManager> ();
		MyImage = GetComponent<Image> ();
		BackButtonImage = GameObject.Find ("BackButton").GetComponent<Image> ();
	}

	public void PausePlayButtonPress()
	{
		if (!MyGameLogic.GetIsPlayerOut()) {
			MyGameLogic.SetIsPaused (!MyGameLogic.GetIsPaused ());
			if (MyGameLogic.GetIsPaused ()) {
				SetImageToPlay ();
			} else {
				MyImage.sprite = PauseImage;
				BackButtonImage.color = new Color (BackButtonImage.color.r, BackButtonImage.color.g,
			                                 BackButtonImage.color.b, 0);
			}
		}
		else
		{
			MyLevelManager.ReloadLevel();
		}
	}

	public void SetImageToPlay()
	{
		MyImage.sprite = PlayImage;
		BackButtonImage.color = new Color (BackButtonImage.color.r, BackButtonImage.color.g,
		                                   BackButtonImage.color.b, 255);
	}

	public void BackButtonPressed()
	{
		if (MyGameLogic.GetIsPaused () || MyGameLogic.GetIsGameOver() || MyGameLogic.GetIsPlayerOut()) {
			MyGameLogic.SetIsPaused (false);
            MyGameLogic.SetIsGameOver(false);
            MyGameLogic.SetIsPlayerOut(false);
			MyLevelManager.LoadLevel ("01 Start");
		}
	}

}
