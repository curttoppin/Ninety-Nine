using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardStack : MonoBehaviour {

	public List<Card> Cards = new List<Card>();
	private int TopZ = 900;

	public virtual void AddCard (Card card)
	{
		Cards.Add(card);
	}

	public virtual void CardClicked(Card card)
	{
	}

	public virtual void PlayCard (Card card, bool isForcePlay)
	{
		
	}

	public virtual void MoveCardToStack(Card card, CardStack stack, bool IsSloppy)
	{
		Cards.Remove (card);
		stack.AddCard (card);
		card.transform.SetParent (stack.transform, true);
		card.Parent = stack;
		stack.SetZPositionToTopOfStack (card);
		if (IsSloppy)
			card.SetRandomTransform (20.0f, 0.2f);
	}

	public int GetCardsCount()
	{
		return Cards.Count;
	}

	public float GetNextTopZPosition()
	{
		float _currentTopZ = TopZ;
		if (Cards.Count > 1)
			_currentTopZ = Cards [Cards.Count - 2].transform.position.z;
		return _currentTopZ - 1.0f;
	}

	public void SetZPositionToTopOfStack(Card _card)
	{
		Vector3 _currentPosition = _card.transform.position;
		_card.transform.position = new Vector3 (_currentPosition.x, _currentPosition.y, GetNextTopZPosition());
	}

	public void SetZPositionsOnAllCards()
	{
		Vector3 _currentPosition;
		for (int i = 0; i < Cards.Count; i++) 
		{
			_currentPosition = Cards[i].transform.position;
			Cards[i].transform.position = new Vector3(_currentPosition.x, _currentPosition.y, TopZ - i);
		}
	}

	public void StraightenStack()
	{
		Vector3 _currentPosition = transform.position;
		for (int i = 1; i <= Cards.Count; i++) 
		{
			Cards[i - 1].transform.position = _currentPosition;
			if (i % 10 == 0)
				_currentPosition = new Vector3(_currentPosition.x + -2.0f,
				                               _currentPosition.y + 2.0f,
				                               _currentPosition.z + -1.0f);
		}
	}
	
	public void JumbleStack()
	{
		for (int i = 0; i < Cards.Count; i++) 
		{
			Cards[i].SetRandomTransform(20.0f, 0.2f, transform.position.z - i);
		}
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
}
