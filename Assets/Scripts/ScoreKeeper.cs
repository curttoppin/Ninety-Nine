using UnityEngine;
using System.Collections;

public class ScoreKeeper : MonoBehaviour {
	public UnityEngine.UI.Text ScoreText;

	GameLogic MyGameLogic;

	void Start()
	{
		MyGameLogic = GameObject.Find ("GameLogic").GetComponent<GameLogic> ();
		Reset();
	}

	public void Reset()
	{
		MyGameLogic.ResetScore ();
	}
	
	public void UpdateScoreText()
	{
		ScoreText.text = MyGameLogic.GetCurrentScore().ToString();
		if (MyGameLogic.GetCurrentScore() == 99) {
			ScoreText.text += "!";
			ScoreText.fontSize = 99;
		} else if (MyGameLogic.GetCurrentScore() > 99) {
			ScoreText.color = Color.red;
			ScoreText.fontSize = 99;
		} else if (MyGameLogic.GetCurrentScore() > 30) {
			ScoreText.fontSize = MyGameLogic.GetCurrentScore();
		} else if (MyGameLogic.GetCurrentScore() <= 30) {
			ScoreText.fontSize = 30;
		}
	}
}
