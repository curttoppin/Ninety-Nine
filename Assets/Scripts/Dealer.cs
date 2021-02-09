using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dealer : CardStack {
	public GameObject CardPrefab;
	public CardStack PlayedStack;
	public AudioClip ShuffleSound;

	public List<Hand> Players;

    public const float DealCardSpeed = 0.2f;

	GameLogic MyGameLogic;

	// Use this for initialization
	void Start ()
	{
		MyGameLogic = GameObject.Find ("GameLogic").GetComponent<GameLogic> ();
		MyGameLogic.ResetScore ();
		if (MyGameLogic.GetNumberOfPlayers() > 1)
			SetUpPlayers ();
		Cards = new List<Card> ();
		for (int i  = 0; i < 52; i++) 
		{
			Cards.Add (CreateNewCard ((CardNames)i));
		}
		Shuffle ();
		StraightenStack ();
		Invoke ("AfterInit", 2.8f);
	}

	private void AfterInit()
	{
		float _dealTime = Deal99 ();
		Players [Random.Range (0, Players.Count)].Invoke ("StartTurn", _dealTime + 2.0f);
	}

	public void Shuffle()
	{
		int i = Cards.Count;
		int _randomPlace;
		Card _holder;
		while (i > 1) 
		{
			i--;
			_randomPlace = Random.Range (0, Cards.Count);
			_holder = Cards [_randomPlace];
			Cards [_randomPlace] = Cards [i];
			Cards [i] = _holder;
		}
		AudioSource.PlayClipAtPoint (ShuffleSound, transform.position);
	}

	public float Deal99()
	{
        float _invokeTimer = 0.0f;
		for (int i = 0; i < 5; i++) 
		{
			foreach (Hand player in Players)
			{
                player.Invoke("InvokedRequestNewCard", _invokeTimer);
                _invokeTimer += 0.05f;
			}
		}
        _invokeTimer += 0.5f;
		foreach (Hand player in Players)
			player.Invoke ("FanCards", _invokeTimer);
        return _invokeTimer;
	}

	public void DealCard(Hand hand)
	{
		if (Cards.Count > 0) 
		{
			Card _card = Cards [Cards.Count - 1];
			_card.MoveCard(hand, hand.IsHuman, DealCardSpeed, true); 
		}
	}

	public void StartNextPlayersTurn()
	{
		int _nextI;
		int _nextDirection = 1;
		if (MyGameLogic.GetIsReversed())
			_nextDirection = -1;
		for (int i = 0; i < Players.Count; i++) 
		{
			if (Players[i].IsMyTurn)
			{
				Players[i].EndTurn();
				_nextI = i + _nextDirection;
				if (_nextI >= Players.Count) _nextI = 0;
				if (_nextI < 0) _nextI = Players.Count - 1;
				Players[_nextI].StartTurn();
				break;
			}
		}
	}

	public void TestWinCondition()
	{
		int _winnerIndex = -1;
		for (int i = 0; i < Players.Count; i++) 
		{
			if (!Players[i].GetIsFolded()) 
			{	
				if (_winnerIndex != -1)
					return; //game not over yet
				_winnerIndex = i;
			}
		}
		MyGameLogic.EndGame (Players[_winnerIndex].IsHuman); //if winner is human Win, else Lose
	}

	// Update is called once per frame
	void Update() 
	{
	}

	private void SetUpPlayers()
	{
		switch (MyGameLogic.GetNumberOfPlayers ()) 
		{
		case (6):
			break; //this is how it's set up by default
		case (5):
			Players.RemoveAt (3);
			break;
		case (4):
			Players.RemoveAt (4);
			Players.RemoveAt (2);
			break;
		case (3):
			Players.RemoveAt (5);
			Players.RemoveAt (3);
			Players.RemoveAt (1);
			break;
		case (2):
			Players.RemoveAt (5);
			Players.RemoveAt (4);
			Players.RemoveAt (2);
			Players.RemoveAt (1);
			break;
		}
	}

	private Card CreateNewCard(CardNames name)
	{
		string _stringName = name.ToString ();
		Suits _suit;
		int _value;
		switch (_stringName [0]) 
		{
		    case 'c':
			    _suit = Suits.Clubs;
			    break;
		    case 'd':
			    _suit = Suits.Diamonds;
			    break;
		    case 'h':
			    _suit = Suits.Hearts;
			    break;
		    case 's':
			    _suit = Suits.Spades;
			    break;
		    default:
			    _suit = Suits.None;
			    break;
		}
		_value = int.Parse(_stringName.Substring (1));

		GameObject _cardGameObject = Instantiate(CardPrefab, transform.position, Quaternion.identity) as GameObject;
		_cardGameObject.transform.SetParent(transform);
		
		Card _card = _cardGameObject.GetComponent<Card>();
		_card.InitCard (this, _value, _suit, name);
		return _card;
	}
}
