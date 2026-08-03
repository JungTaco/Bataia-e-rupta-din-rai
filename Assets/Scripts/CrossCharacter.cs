using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrossCharacter : Character
{
    [SerializeField]
    private int _crossesNeeded;
    private int _crossCounter;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	new void Start()
    {
        _crossCounter = 0;
        _painPointNumber = 4;
        _enabledPainPoints = _painPoints;
		_neededOrderSymbols = _orderSymbols;
		_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		SpawnSymbols();
		_expectedPressingOrder = GetExpectedPressingOrder();
		_isBeingHit = false;
		StartCoroutine(CharacterTimerCoroutine());
	}

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

	protected override void SpawnSymbols()
	{
		for (int i = 0; i < _enabledPainPoints.Count; i++)
		{
			Instantiate(_orderSymbols[i], _enabledPainPoints[i].transform.position, Quaternion.identity, _enabledPainPoints[i].transform);
		}
	}

	protected override void CheckRightKeysArepressed()
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
				StartCoroutine(HitTimerCoroutine());
			}
			else
			{
				if (_crossCounter < _crossesNeeded - 1)
				{
					_crossCounter++;
					RespawnPainPoints();
					_currentExpectedKeyIndex = 0;
				}
				else
				{
					_currentState = State.CURED;
					Actions.OnCured?.Invoke();
					Actions.OnLeft?.Invoke();
				}
			}
		}
		else if (Keyboard.current.anyKey.wasPressedThisFrame)
		{
			Actions.OnMistakeMade?.Invoke();
		}
	}

	private void RespawnPainPoints()
	{
		foreach (PainPoint pp in _painPoints)
		{
			pp.gameObject.SetActive(true);
		}
	}
}
