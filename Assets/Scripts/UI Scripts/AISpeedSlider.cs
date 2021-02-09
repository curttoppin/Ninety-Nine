using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AISpeedSlider : MonoBehaviour {
	
	public Slider MySlider;
	public Text SliderText;
	private GameLogic MyGameLogic;
	
	public void OnSliderValueChange()
	{
		MyGameLogic.SetAISpeedDivider (MySlider.value);
		SliderText.text = MySlider.value.ToString ("F1");
		MyGameLogic.SaveSettings ();
	}
	
	void Start()
	{
		MyGameLogic = GameObject.Find ("GameLogic").GetComponent<GameLogic> ();
        MySlider.value = MyGameLogic.GetAISpeedDivider();
        SliderText.text = MySlider.value.ToString("F1");
	}
}
