using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerSelectSlider : MonoBehaviour {

	public Slider MySlider;
	public Text SliderText;
	private GameLogic MyGameLogic;

	public void OnSliderValueChange()
	{
		MyGameLogic.SetNumberOfPlayers ((int)MySlider.value);
		SliderText.text = MySlider.value.ToString ();
		MyGameLogic.SaveSettings ();
	}

	void Start()
	{
		MyGameLogic = GameObject.Find ("GameLogic").GetComponent<GameLogic> ();
        MySlider.value = MyGameLogic.GetNumberOfPlayers();
        SliderText.text = MySlider.value.ToString();
    }

}
