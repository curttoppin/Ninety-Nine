using UnityEngine;
using System.Collections;
using System;

public enum Suits
{
	Spades,
	Hearts,
	Clubs,
	Diamonds,
	None
}

public enum CardNames
{
	c01, c02, c03, c04, c05,
	c06, c07, c08, c09,
	c10, c11, c12, c13,
	s01, s02, s03, s04, s05,
	s06, s07, s08, s09,
	s10, s11, s12, s13,
	h01, h02, h03, h04, h05,
	h06, h07, h08, h09,
	h10, h11, h12, h13,
	d01, d02, d03, d04, d05,
	d06, d07, d08, d09,
	d10, d11, d12, d13
}

[RequireComponent(typeof(Animation))]
public class Card : MonoBehaviour, IComparable {
	public CardStack Parent;

	static int CurrentAceValue = 1;

	int Value;
	Suits Suit;
	CardNames Name;
	bool IsFlipped;
	bool IsDimmed;

	Sprite FrontImage;
	SpriteRenderer MySpriteRenderer;
	GameLogic MyGameLogic;

	public Sprite BackImage;
	public Color HighlightColor;
	public Color DimColor;

	public void InitCard(Dealer parent, int value, Suits suit, CardNames name )
	{
		Parent = parent;
		Value = value;
		Suit = suit;
		Name = name;
		FrontImage = Resources.Load (Name.ToString (), typeof(Sprite)) as Sprite;
		MySpriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		MyGameLogic = GameObject.Find ("GameLogic").GetComponent<GameLogic> ();
	}


	public virtual void OnMouseUpAsButton()
	{
		Parent.CardClicked (this);
	}

	public bool GetIsFlipped()
	{
		return IsFlipped;
	}

	public void Flip()
	{
		if (IsFlipped)
			GetComponent<SpriteRenderer> ().sprite = BackImage;
		else
			GetComponent<SpriteRenderer> ().sprite = FrontImage;
		IsFlipped = !IsFlipped;
	}

	public struct RandomTransform
	{
		public Quaternion rotation;
		public Vector3 position;
	}
	public RandomTransform GetRandomTransform(float range, float rotationRange)
	{
		float _randZ = UnityEngine.Random.Range (-rotationRange, rotationRange);
		float _randX = UnityEngine.Random.Range (-range, range);
		float _randY = UnityEngine.Random.Range (-range, range);
		RandomTransform _results;
		_results.rotation = new Quaternion(transform.rotation.x,
		                                   transform.rotation.y, 
		                                   _randZ,
		                                   transform.rotation.w);
		_results.position = new Vector3(_randX, 
		                                _randY, 
		                                 0);
		return _results;
	}

	public void SetRandomTransform(float range, float rotationRange)
	{
		RandomTransform _newTransform = GetRandomTransform (range, rotationRange);
		Quaternion _currentRotation = transform.rotation;
		Vector3 _currentPosition = transform.position;
		transform.rotation = _newTransform.rotation;
		transform.position = _currentPosition + _newTransform.position;
	}

	public void SetRandomTransform(float range, float rotationRange, float z)
	{
		SetRandomTransform (range, rotationRange);
		transform.position = new Vector3 (transform.position.x, transform.position.y, z);
	}

	public void ResetTransform()
	{
		transform.position = new Vector3 (0, 0, 0);
		transform.rotation = Quaternion.identity;
	}

	public int GetStandardValue()
	{
		return Value;
	}

	public int GetValueIn99()
	{
		switch (Value) 
		{
		case (1):
			return CurrentAceValue;
		case (4):
			return 0;
		case (9):
			return 99 - MyGameLogic.GetCurrentScore();
		case (10):
			return -10;
		case (11):
			return 10;
		case (12):
			return 10;
		case (13):
			return 0;
		default:
			return Value;
		}
	}

	public static void SetCurrentAceValue(int value)
	{
		CurrentAceValue = value;
	}

	public bool IsPlayable()
	{
		return MyGameLogic.CanPlayCard(this);
	}

	public void SetDimUsingPlayable()
	{
		if (!IsPlayable ()) 
		{
			IsDimmed = true;
			MySpriteRenderer.color = DimColor;
		} 
		else 
		{
			IsDimmed = false;
			MySpriteRenderer.color = Color.white;
		}
	}

	public void SetDim(bool isDimmed)
	{
		IsDimmed = isDimmed;
		if (isDimmed)
			MySpriteRenderer.color = DimColor;
		else
			MySpriteRenderer.color = Color.white;
	}

	int IComparable.CompareTo (object card1)
	{
		try{
			int mySortValue = GetSortValueFor99();
			int _card1SortValue = ((Card)card1).GetSortValueFor99();
			
			if (mySortValue < _card1SortValue)
				return -1;
			else if (mySortValue > _card1SortValue)
				return 1;
			else
			{
				if (Suit < ((Card)card1).Suit)
					return -1;
				else
					return 1;
			}
		}catch
		{
			return 0;
		}
	}

	public int GetSortValueFor99()
	{
		//-3 , -2 , -1 ,  0  , 1 - 8
		//9's, 4's, K's, 10's, A's to 8's
		if (Value == 9)
			return -3;
		else if (Value == 4)
			return -2;
		else if (Value == 13)
			return -1;
		else if (Value == 10)
			return 0;
		else
			return Value;
	}

	public void MoveCard(Vector3 newPosition, bool isEndFlipped, float playTime, bool isSloppy)
	{	
		float _zRotEndValue = 0;
		if (isSloppy) 
		{
			RandomTransform _addSlop = GetRandomTransform(40.0f, 0.4f);
			newPosition = newPosition + _addSlop.position;
			_zRotEndValue = _addSlop.rotation.z;
		}

		Animation _anim = GetComponent<Animation>();
		
		AnimationCurve _xCurve = AnimationCurve.Linear(0, transform.localPosition.x,  //start time, startValue,
		                                               playTime, newPosition.x); //endTime, endValue
		
		AnimationCurve _yCurve = AnimationCurve.Linear(0, transform.localPosition.y,
		                                               playTime, newPosition.y); 
		
		AnimationCurve _zCurve = AnimationCurve.Linear (0, newPosition.z,
		                                                playTime, newPosition.z);
		
		AnimationCurve _xRot = AnimationCurve.Linear (0, transform.localRotation.x,
		                                              playTime, 0);
		
		AnimationCurve _yRot;
		if (IsFlipped == isEndFlipped) {
			_yRot = AnimationCurve.Linear (0, transform.localRotation.y,
			                               playTime, 0);
		} else {
			_yRot = AnimationCurve.Linear (0, 3.14f,
			                               playTime, 0.0f);
			Invoke ("Flip", playTime/2);
		}
		
		AnimationCurve _zRot = AnimationCurve.Linear (0, transform.localRotation.z,
		                                              playTime, _zRotEndValue);
		
		AnimationCurve _wRot = AnimationCurve.Linear (0, transform.localRotation.w,
		                                              playTime, transform.localRotation.w);

		AnimationClip _clip = new AnimationClip();

		_clip.SetCurve("", typeof(Transform), "localPosition.x", _xCurve);
		_clip.SetCurve("", typeof(Transform), "localPosition.y", _yCurve);
		_clip.SetCurve("", typeof(Transform), "localPosition.z", _zCurve);
		_clip.SetCurve("", typeof(Transform), "localRotation.x", _xRot);
		_clip.SetCurve("", typeof(Transform), "localRotation.y", _yRot);
		_clip.SetCurve("", typeof(Transform), "localRotation.z", _zRot);
		_clip.SetCurve("", typeof(Transform), "localRotation.w", _wRot);
		
		_anim.AddClip(_clip, "moveCard" + Name.ToString());
		_anim.Play("moveCard" + Name.ToString());
	}

	public void MoveCard(CardStack newStack, bool isEndFlipped, float playTime, bool isSloppy)
	{
		Parent.MoveCardToStack (this, newStack, false);
		RandomTransform _newTransform;
		if (isSloppy) {
			_newTransform = GetRandomTransform (40.0f, 0.4f);
		} else {
			_newTransform.position = Vector3.zero;
			_newTransform.rotation = Quaternion.identity;
		}

		Animation _anim = GetComponent<Animation>();

		AnimationCurve _xCurve = AnimationCurve.Linear(0, transform.localPosition.x,  //start time, startValue,
		                                               playTime, _newTransform.position.x / transform.localScale.x); //endTime, endValue

		AnimationCurve _yCurve = AnimationCurve.Linear(0, transform.localPosition.y,
		                                               playTime, _newTransform.position.y / transform.localScale.y); 

		AnimationCurve _zCurve = AnimationCurve.Linear (0, transform.localPosition.z,
		                                                playTime, transform.localPosition.z);

		AnimationCurve _xRot = AnimationCurve.Linear (0, transform.localRotation.x,
		                                             playTime, _newTransform.rotation.x);

		AnimationCurve _yRot;
		if (IsFlipped == isEndFlipped) {
			 _yRot = AnimationCurve.Linear (0, transform.localRotation.y,
		                                    playTime, _newTransform.rotation.y);
		} else {
			_yRot = AnimationCurve.Linear (0, 3.14f,
			                               playTime, 0.0f);
			Invoke ("Flip", playTime/2);
		}

		AnimationCurve _zRot = AnimationCurve.Linear (0, transform.localRotation.z,
		                                              playTime, _newTransform.rotation.z);

		AnimationCurve _wRot = AnimationCurve.Linear (0, transform.localRotation.w,
		                                              playTime, _newTransform.rotation.w);

		// we'll need a curve for each component I want to move (4 x,y and rotation x or y)
		AnimationClip _clip = new AnimationClip();

		//_clip.legacy = true;
		_clip.SetCurve("", typeof(Transform), "localPosition.x", _xCurve);
		_clip.SetCurve("", typeof(Transform), "localPosition.y", _yCurve);
		_clip.SetCurve("", typeof(Transform), "localPosition.z", _zCurve);
		_clip.SetCurve("", typeof(Transform), "localRotation.x", _xRot);
		_clip.SetCurve("", typeof(Transform), "localRotation.y", _yRot);
		_clip.SetCurve("", typeof(Transform), "localRotation.z", _zRot);
		_clip.SetCurve("", typeof(Transform), "localRotation.w", _wRot);

		_anim.AddClip(_clip, "moveCard" + Name.ToString());
		_anim.Play("moveCard" + Name.ToString());
	}

	public void SetZPosition(float z)
	{
		Debug.Log ("set z to: " + z.ToString());
		transform.position = new Vector3 (transform.position.x, transform.position.y, z);
	}

	// Use this for initialization
	void Start() 
	{
	}
	
	// Update is called once per frame
	void Update() 
	{
	}

	#if UNITY_STANDALONE	
	public void OnMouseEnter()
	{
		if (Parent.GetType() == typeof(Hand) && ((Hand)Parent).IsHuman && ((Hand)Parent).IsMyTurn && IsPlayable() && !IsDimmed)
		{
			MySpriteRenderer.color = HighlightColor;
		}
	}
	
	
	public void OnMouseExit()
	{
		if (!IsDimmed)
			MySpriteRenderer.color = Color.white;
	}
	#endif
}

