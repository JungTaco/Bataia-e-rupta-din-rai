using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static UnityEditor.PlayerSettings;

public class Character : MonoBehaviour
{
    [SerializeField]
    private Sprite _uncuredSprite;
	[SerializeField]
	private Sprite _hitSprite;
	[SerializeField]
	private Sprite _curedSprite;
	[SerializeField]
	private ChallengeType _challengeType;
	[SerializeField]
    private List<PainPoint> _painPoints = new List<PainPoint>();
	[SerializeField]
	private List<OrderSymbol> _orderSymbols = new List<OrderSymbol>();
	private List<PainPoint> _enabledPainPoints = new List<PainPoint>();
	private List<Key> _expectedPressingOrder = new List<Key>();
	List<OrderSymbol> _neededOrderSymbols = new List<OrderSymbol>();
	SortedDictionary<int, PainPoint> _painPointsOrder = new SortedDictionary<int, PainPoint>();
	private int _currentExpectedKeyIndex = 0;
	private float _moveSpeed;
	private SpriteRenderer _spriteRenderer;
    private int _painPointNumber;
    private float _timer;
    private float _orthographicScreenWidth;

	private enum ChallengeType { NORMAL, AMIN, CROSS };
	
	public enum state {WAITING, UNCURED, HIT, CURED};
    private state _currentState = state.WAITING; 

    public state GetCurrentState()
    {
        return _currentState;
    }

    public void SetCurrentState(state newState)
    {
        _currentState = newState;
    }

    public void SetMoveSpeed(float newMoveSpeed)
    {
        _moveSpeed = newMoveSpeed;
    }

    public void SetPainPointNumber(int painPointNumber)
    {
        _painPointNumber = painPointNumber;
    }

    public void SetOrthographicScreenWidth (float newOrthographicScreenWidth)
    {
        _orthographicScreenWidth = newOrthographicScreenWidth;
    }

	private void OnEnable()
	{
        Actions.OnCured += ChangeSprite;
        Actions.OnHit += ChangeSprite;
        Actions.OnHitFinished += ChangeSprite;
	}

	private void OnDisable()
	{
		Actions.OnCured -= ChangeSprite;
		Actions.OnHit -= ChangeSprite;
		Actions.OnHitFinished -= ChangeSprite;
	}

	void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		DecideVisiblePainPoints();
		SpawnSymbols();
        _expectedPressingOrder = GetExpectedPressingOrder();
    }

    void Update()
    {
        if(_currentState == state.UNCURED && transform.position.x < 0)
        {
            MoveToCenter();
		}
        else if (_currentState == state.UNCURED)
        {
            CheckRightKeysArepressed();			
		}
        else if(_currentState == state.CURED && transform.position.x < (_orthographicScreenWidth*1.5))
        {
            Leave();
        }
		else if (_currentState == state.CURED && transform.position.x >= (_orthographicScreenWidth * 1.5))
        {
            Destroy(this.gameObject);
        }
	}

    private void MoveToCenter()
    {
		float newPosX = transform.position.x + 0.5f * _moveSpeed;
		transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);
	}

    private void Leave()
    {
		float newPosX = transform.position.x + 0.5f * _moveSpeed;
		transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);
	}

    private void DecideVisiblePainPoints()
    {
		//shuffles list of pain points
		List<PainPoint> shuffledPainPoints = Shuffle(_painPoints);
		//chooses the pain points to keep enabled (truncates to how many are needed)
		List<PainPoint> truncatedPainPoints = TruncateList(shuffledPainPoints);
        _enabledPainPoints = truncatedPainPoints;

        //disables pain points
        for(int i = 0; i < _painPoints.Count; i++)
        {
            if (!_enabledPainPoints.Contains(_painPoints[i]))
            {
				_painPoints[i].gameObject.SetActive(false);
			}
        }
	}

    private List<OrderSymbol> GetNeededOrderSymbols()
    {
        //truncated symbol list to how many are needed
        List<OrderSymbol> truncatedOrderSymbols = TruncateList(_orderSymbols);
        //shuffles list
        List<OrderSymbol> shuffledOrderSymbols = Shuffle(truncatedOrderSymbols);
        return shuffledOrderSymbols;
	}

    //override for different challenges
    private void SpawnSymbols()
    {
	    _neededOrderSymbols = GetNeededOrderSymbols();
        for(int i=0; i < _enabledPainPoints.Count; i++)
        {
            Instantiate(_neededOrderSymbols[i], _enabledPainPoints[i].transform.position, Quaternion.identity, _enabledPainPoints[i].transform);
		}
    }

    private List<Key> GetExpectedPressingOrder()
    {
        List<Key> expectedPressingOrder = new List<Key>();
        for (int i = 0; i < _painPointNumber; i++)
        {
            _painPointsOrder.Add(_neededOrderSymbols[i].GetOrderNumber()-1, _enabledPainPoints[i]);
        }
        for(int i = 0; i < _painPointsOrder.Count; i++)
        {
            expectedPressingOrder.Add(_painPointsOrder[i].GetNeededKey());
		}
        return expectedPressingOrder;
	}

    private void CheckRightKeysArepressed()
    {
		if (Keyboard.current[_expectedPressingOrder[_currentExpectedKeyIndex]].wasPressedThisFrame)
		{
			_painPointsOrder[_currentExpectedKeyIndex].gameObject.SetActive(false);
            if (_currentExpectedKeyIndex < _painPointNumber - 1)
            {
                //_currentState = state.HIT;
				//Actions.OnHit?.Invoke();
				_currentExpectedKeyIndex++;
			}       
			else
			{
				_currentState = state.CURED;
				Actions.OnCured?.Invoke();
			}
		}
		else if (Keyboard.current.anyKey.wasPressedThisFrame)
		{
			Actions.OnWrongKeyPressed?.Invoke();
		}
	}

    private void ChangeSprite()
    {
        if(_currentState == state.CURED)
        {
			_spriteRenderer.sprite = _curedSprite;
		}
		else if (_currentState == state.UNCURED)
		{
			_spriteRenderer.sprite = _uncuredSprite;
		}
		else if (_currentState == state.HIT)
        {
            _spriteRenderer.sprite = _hitSprite;
        }
	}

	private List<OrderSymbol> TruncateList(List<OrderSymbol> list)
    {
        List<OrderSymbol> newList = list.GetRange(0, _painPointNumber);
        return newList;
    }

	private List<PainPoint> TruncateList(List<PainPoint> list)
	{
		List<PainPoint> newList = list.GetRange(0, _painPointNumber);
		return newList;
	}

	private List<PainPoint> Shuffle(List<PainPoint> list)
    {
		System.Random rng = new System.Random();
		List<PainPoint> shuffledList = list.OrderBy(x => rng.Next()).ToList();
        return shuffledList;
	}

	private List<OrderSymbol> Shuffle(List<OrderSymbol> list)
    {
		System.Random rng = new System.Random();
		List<OrderSymbol> shuffledList = list.OrderBy(x => rng.Next()).ToList();
        return shuffledList;
	}
}
