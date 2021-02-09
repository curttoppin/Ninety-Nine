using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hand : CardStack {

	public int MaxCardCount;
	public bool IsHuman;
	public CardStack PlayStack;
	public bool IsMyTurn = false;
	public bool IsVertical = false;	
	public AudioClip PlayCardSound;

    public const float BaseCardBumpSpeed = 0.2f;
    public const float BaseCardFanSpeed = 0.4f;
    public const float BaseCardPlaySpeed = 0.35f;
    public const float BaseCardFoldSpeed = 0.25f;

	bool IsFolded = false;

	GameLogic MyGameLogic;
	Dealer MyDealer;

	Card SelectedAce = null;

	public override void AddCard(Card _card)
	{
		base.AddCard (_card);
		if (IsHuman && !_card.GetIsFlipped ())
			_card.Flip ();
		else if (_card.GetIsFlipped ())
			_card.Flip ();
		SortCards ();
	}

	public override void CardClicked(Card card)
	{
		if (IsHuman && IsMyTurn && !MyGameLogic.GetIsPaused ())
			PlayCard(card, false);
	}

	public override void PlayCard (Card card, bool isForcePlay)
	{
		if ((IsHuman && card.GetStandardValue () == 1 && MyGameLogic.GetCurrentScore() + 11 <= 99) && !isForcePlay) 
		{
			SelectedAce = card;
			MyGameLogic.SetIsPaused(true);
			MyGameLogic.SetIsWaitingForAceSelection(true);
		} 
		else 
		{
			if (MyGameLogic.CanPlayCard (card)) {
				card.MoveCard(PlayStack, true, BaseCardPlaySpeed * MyGameLogic.GetCardSpeedMultiplier(), true);
				MyGameLogic.AddScore (card.GetValueIn99 ());
				Invoke ("InvokedRequestNewCard", 0.3f);
				Invoke ("FanCards", 0.4f);
				if (card.GetStandardValue () == 4)
					MyGameLogic.ReverseOrder ();
				MyDealer.Invoke ("StartNextPlayersTurn", 0.5f);
				AudioSource.PlayClipAtPoint (PlayCardSound, transform.position);
			}
		}
	}

	public void InvokedRequestNewCard()
	{
		MyDealer.DealCard (this);
		if (IsHuman)
			DimUnplayableCards ();
	}

	public void PlaySelectedAce()
	{
		if (SelectedAce == null) 
		{
			return;
		}
		PlayCard (SelectedAce, true);
		SelectedAce = null;
	}

	public void AIPlayCard()
	{
		if (MyGameLogic.GetIsPaused()) 
		{
			Invoke ("AIPlayCard", 2.0f);
			return;
		}
		List<Card> _playableCards = GetPlayableCards ();
		int _playCardIndex = -1;
		int _playValue = -1;
		Card _card;

		// if holding 3 or more winner cards, play a 9 if you have one
		if (GetWinnerCards (_playableCards).Count >= 3)
			_playCardIndex = Find9 (_playableCards);

		if (_playCardIndex == -1) 
		{
			//find highest value playable card that's not a 9 or 10
			for (int i = 0; i < _playableCards.Count; i++) {
				_card = _playableCards [i];
				if (_card.GetValueIn99 () > _playValue && _card.GetStandardValue() != 9) {
					_playCardIndex = i;
					_playValue = _playableCards [i].GetValueIn99 ();
				}
			}
		}
		if (_playCardIndex == -1) //if no other options play a 9
			_playCardIndex = Find9 (_playableCards);

		if (_playCardIndex == -1)
			_playCardIndex = Find10 (_playableCards);

		if (_playCardIndex != -1)
			PlayCard (_playableCards [_playCardIndex], false);
		else 
		{
			FoldCards (true);
			MyDealer.StartNextPlayersTurn();
		}
	}

	public List<Card> GetPlayableCards()
	{
		List<Card> _playableCards = new List<Card> ();
		for (int i = 0; i < Cards.Count; i++) 
		{
			if (MyGameLogic.CanPlayCard(Cards[i]))
				_playableCards.Add (Cards[i]);
		}
		return _playableCards;
	}

	//returns list of all cards in list that add 0 and 9s
	public List<Card> GetWinnerCards(List<Card> cards)
	{
		List<Card> _winnerCards = new List<Card> ();
		Card _card;
		for (int i = 0; i < cards.Count; i++) 
		{
			_card = cards[i];
			if (_card.GetValueIn99() == 0 || _card.GetStandardValue() == 9)
				_winnerCards.Add (_card);
		}
		return _winnerCards;
	}

	public int Find9(List<Card> cards)
	{
		for (int i = 0; i < cards.Count; i++) 
		{
			if (cards[i].GetStandardValue() == 9) return i;
		}
		return -1;
	}

	public int Find10(List<Card> cards)
	{
		for (int i = 0; i < cards.Count; i++) 
		{
			if (cards[i].GetStandardValue() == 10) return i;
		}
		return -1;
	}

	public void StartTurn()
	{
		IsMyTurn = true;
		if (!IsFolded)
		{
			BumpCardsTowardsCenter ();
			if (IsHuman) {
				DimUnplayableCards ();
				TestLoseCondition ();
			} else {
				Invoke ("AIPlayCard", MyGameLogic.GetAITurnLength());
			}
		}else {
			MyDealer.StartNextPlayersTurn();
		}
	}

	public void BumpCardsTowardsCenter()
	{
		Vector3 _currentPosition;
		Vector3 _anchoredPosition = GetAnchoredPosition ();
		float _bumpDirectionX = 40;
		float _bumpDirectionY = 60;
		
		if (_anchoredPosition.x > 0)
			_bumpDirectionX *= -1;
		else if (_anchoredPosition.x == 0)
			_bumpDirectionX = 0;
		
		if (_anchoredPosition.y > 0)
			_bumpDirectionY *= -1;
		else if (_anchoredPosition.y == 0)
			_bumpDirectionY = 0;
		
		for (int i = 0; i < Cards.Count; i++) 
		{
			_currentPosition = Cards[i].transform.localPosition;
			_currentPosition = new Vector3(_currentPosition.x + _bumpDirectionX, 
			                               _currentPosition.y + _bumpDirectionY, 
			                               _currentPosition.z);
			Cards[i].MoveCard (_currentPosition, IsHuman, BaseCardBumpSpeed * MyGameLogic.GetCardSpeedMultiplier(), false);
		}
	}

	public void TestLoseCondition()
	{
		if (GetPlayableCards ().Count == 0) 
		{
			Invoke ("FoldCardsAndShow", 2.0f);
		}
	}

	public void DimUnplayableCards ()
	{
		for (int i = 0; i < Cards.Count; i++) 
		{
			Cards[i].SetDimUsingPlayable();
		}
	}

	public void UnDimAllCards()
	{
		for (int i = 0; i < Cards.Count; i++) 
		{
			Cards[i].SetDim (false);
		}
	}

	public void EndTurn()
	{
		IsMyTurn = false;
	}

	public void SortCards()
	{
		//starting from index 0: 9's, 4's, K's, 10's, A's to 8's
		Cards.Sort ();
	}

	public void FanCards()
	{
		if (IsVertical)
			FanCardVertically ();
		else
			FanCardsHorizontally ();
	}

	public void FanCardsHorizontally()
	{
		float _distanceBetweenCards = 40;
#if UNITY_ANDROID
		if (IsHuman)
			_distanceBetweenCards = 90;
#endif
		Vector3 _currentPosition = new Vector3 (0 - (_distanceBetweenCards * (MaxCardCount / 2.0f)), 0, 0);
		for (int i = 0; i < Cards.Count; i++) 
		{
			Cards[i].MoveCard ( _currentPosition, IsHuman, BaseCardFanSpeed * MyGameLogic.GetCardSpeedMultiplier(), false);
			_currentPosition = new Vector3(_currentPosition.x + _distanceBetweenCards, _currentPosition.y, _currentPosition.z - 1.0f);
		}
	}

	public void FanCardVertically ()
	{
		float _distanceBetweenCards = 40;
		Vector3 _currentPosition = new Vector3 (0, 0 - (_distanceBetweenCards * (MaxCardCount / 2.0f)), 0);
		for (int i = 0; i < Cards.Count; i++) 
		{
			Cards[i].MoveCard ( _currentPosition, IsHuman, BaseCardFanSpeed * MyGameLogic.GetCardSpeedMultiplier(), false);
			_currentPosition = new Vector3(_currentPosition.x, _currentPosition.y + _distanceBetweenCards, _currentPosition.z - 1.0f);
		}
	}

	public void FoldCardsAndShow()
	{
		FoldCards (true);
	}

	public Vector3 GetAnchoredPosition()
	{
		return GetComponent<RectTransform>().anchoredPosition;
	}

	public void FoldCards(bool isShowHand)
	{
		IsFolded = true;
		Vector3 _currentPosition;
		Vector3 _anchoredPosition = GetAnchoredPosition ();
		float _foldDirectionX = 40;
		float _foldDirectionY = 60;

		if (_anchoredPosition.x > 0)
			_foldDirectionX *= -1;
		else if (_anchoredPosition.x == 0)
			_foldDirectionX = 0;

		if (_anchoredPosition.y > 0)
			_foldDirectionY *= -1;
		else if (_anchoredPosition.y == 0)
			_foldDirectionY = 0;

		for (int i = 0; i < Cards.Count; i++) 
		{
			_currentPosition = Cards[i].transform.localPosition;
			_currentPosition = new Vector3( _currentPosition.x + _foldDirectionX, 
			                                _currentPosition.y + _foldDirectionY, 
			                                _currentPosition.z);
			Cards[i].MoveCard(_currentPosition, true, BaseCardFoldSpeed * MyGameLogic.GetCardSpeedMultiplier(), true);
		}

		if (IsHuman) 
		{
			UnDimAllCards();
			MyDealer.StartNextPlayersTurn();
			MyGameLogic.SetUpSoftEnd();
		}
		MyDealer.TestWinCondition();

	}

	public bool GetIsFolded()
	{
		return IsFolded;
	}
	
	// Use this for initialization
	void Start () 
	{
		MyGameLogic = GameObject.Find ("GameLogic").GetComponent<GameLogic> ();
		MyDealer = GameObject.Find ("DrawStack").GetComponent<Dealer> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
