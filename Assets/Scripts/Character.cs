using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections;
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
	protected Sprite _uncuredSprite;
	[SerializeField]
	protected Sprite _hitSprite;
	[SerializeField]
	protected Sprite _curedSprite;
	[SerializeField]
    protected List<PainPoint> _painPoints = new List<PainPoint>();
	[SerializeField]
	protected List<OrderSymbol> _orderSymbols = new List<OrderSymbol>();
	protected List<PainPoint> _enabledPainPoints = new List<PainPoint>();
	protected List<Key> _expectedPressingOrder = new List<Key>();
	protected List<OrderSymbol> _neededOrderSymbols = new List<OrderSymbol>();
	protected SortedDictionary<int, PainPoint> _painPointsOrder = new SortedDictionary<int, PainPoint>();
	protected int _currentExpectedKeyIndex = 0;
	protected float _moveSpeed;
	protected SpriteRenderer _spriteRenderer;
	protected int _painPointNumber;
	protected float _timer;
	protected float _orthographicScreenWidth;
	protected bool _isBeingHit;

	public enum State {WAITING, UNCURED, CURED, ANGRY};
	protected State _currentState = State.WAITING; 

    public State GetCurrentState()
    {
        return _currentState;
    }

    public void SetCurrentState(State newState)
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

	public void SetTimer(float timer)
	{
        _timer = timer;
	}
	public void SetOrthographicScreenWidth (float newOrthographicScreenWidth)
    {
        _orthographicScreenWidth = newOrthographicScreenWidth;
    }

	protected void OnEnable()
	{
        Actions.OnCured += ChangeSprite;
		Actions.OnCured += StopTimer;
		Actions.OnHit += ChangeSprite;
        Actions.OnHitFinished += ChangeSprite;
		Actions.OnLostLevel += GetsAngry;
	}

	protected void OnDisable()
	{
		Actions.OnCured -= ChangeSprite;
		Actions.OnCured -= StopTimer;
		Actions.OnHit -= ChangeSprite;
		Actions.OnHitFinished -= ChangeSprite;
		Actions.OnLostLevel -= GetsAngry;
	}

	protected void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		DecideVisiblePainPoints();
		SpawnSymbols();
        _expectedPressingOrder = GetExpectedPressingOrder();
        _isBeingHit = false;
		StartCoroutine(CharacterTimer());
	}

	protected void Update()
    {
        if(_currentState == State.UNCURED && transform.position.x < 0)
        {
            MoveToCenter();
		}
        else if (_currentState == State.UNCURED)
        {
            CheckRightKeysArepressed();			
		}
        else if(_currentState == State.CURED && !_isBeingHit && transform.position.x < (_orthographicScreenWidth*1.5))
        {
            Leave();
        }
		else if(_currentState == State.ANGRY && !_isBeingHit && transform.position.x < (_orthographicScreenWidth * 1.5))
		{
			Leave();
		}
		else if (transform.position.x >= (_orthographicScreenWidth * 1.5))
		{
			Destroy(gameObject);
		}
	}

	protected void MoveToCenter()
    {
		float newPosX = transform.position.x + 0.5f * _moveSpeed;
		transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);
	}

	protected void Leave()
    {
		float newPosX = transform.position.x + 0.5f * _moveSpeed;
		transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);
	}

    protected virtual void DecideVisiblePainPoints()
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

	protected List<OrderSymbol> GetNeededOrderSymbols()
    {
        //truncated symbol list to how many are needed
        List<OrderSymbol> truncatedOrderSymbols = TruncateList(_orderSymbols);
        //shuffles list
        List<OrderSymbol> shuffledOrderSymbols = Shuffle(truncatedOrderSymbols);
        return shuffledOrderSymbols;
	}

	//override for different challenges
	protected virtual void SpawnSymbols()
    {
	    _neededOrderSymbols = GetNeededOrderSymbols();
        for(int i=0; i < _enabledPainPoints.Count; i++)
        {
            Instantiate(_neededOrderSymbols[i], _enabledPainPoints[i].transform.position, Quaternion.identity, _enabledPainPoints[i].transform);
		}
    }

	protected List<Key> GetExpectedPressingOrder()
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

	protected virtual void CheckRightKeysArepressed()
    {
		if (Keyboard.current[_expectedPressingOrder[_currentExpectedKeyIndex]].wasPressedThisFrame)
		{
			_painPointsOrder[_currentExpectedKeyIndex].gameObject.SetActive(false);
            _isBeingHit = true;
			Actions.OnHit?.Invoke();
            Actions.OnHitCoordinates?.Invoke(_painPointsOrder[_currentExpectedKeyIndex].transform.position);
			if (_currentExpectedKeyIndex < _painPointNumber - 1)
            {
				_currentExpectedKeyIndex++;
                StartCoroutine(HitTimer());
			}       
			else
			{
                _currentState = State.CURED;
				Actions.OnCured?.Invoke();
				Actions.OnLeft?.Invoke();
			}
		}
		else if (Keyboard.current.anyKey.wasPressedThisFrame)
		{
			Actions.OnMistakeMade?.Invoke();
		}
	}

	protected void ChangeSprite()
    {
		if (_isBeingHit)
		{
			_spriteRenderer.sprite = _hitSprite;
		}
		else if (_currentState == State.CURED)
        {
			_spriteRenderer.sprite = _curedSprite;
		}
		else if (_currentState == State.UNCURED)
		{
			//Debug.Log(_spriteRenderer);
			_spriteRenderer.sprite = _uncuredSprite;
		}
	}

	protected IEnumerator HitTimer()
    {
		while (true)
		{
			yield return new WaitForSeconds(.2f);
            _isBeingHit = false;
            Actions.OnHitFinished?.Invoke();
            StopCoroutine(HitTimer());
		}
	}

	protected IEnumerator CharacterTimer()
    {
		while (true)
		{
			yield return new WaitForSeconds(_timer);
			_currentState = State.ANGRY;
			Actions.OnMistakeMade?.Invoke();
			Actions.OnLeft?.Invoke();
			StopCoroutine(CharacterTimer());
		}
	}

	protected void StopTimer()
	{
		StopCoroutine(CharacterTimer());
	}

	protected List<OrderSymbol> TruncateList(List<OrderSymbol> list)
    {
        List<OrderSymbol> newList = list.GetRange(0, _painPointNumber);
        return newList;
    }

	protected List<PainPoint> TruncateList(List<PainPoint> list)
	{
		List<PainPoint> newList = list.GetRange(0, _painPointNumber);
		return newList;
	}

	protected List<PainPoint> Shuffle(List<PainPoint> list)
    {
		System.Random rng = new System.Random();
		List<PainPoint> shuffledList = list.OrderBy(x => rng.Next()).ToList();
        return shuffledList;
	}

	protected List<OrderSymbol> Shuffle(List<OrderSymbol> list)
    {
		System.Random rng = new System.Random();
		List<OrderSymbol> shuffledList = list.OrderBy(x => rng.Next()).ToList();
        return shuffledList;
	}

	private void GetsAngry()
	{
		_currentState = State.ANGRY;
	}
}
