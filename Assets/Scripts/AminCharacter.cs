using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AminCharacter : Character
{
	[SerializeField]
	private List<OrderSymbol> _orderSymbolsAmin = new List<OrderSymbol>();
	[SerializeField]
	private List<PainPoint> _painPointsAmin = new List<PainPoint>();
    private bool _showAmin;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	new void Start()
    {
		_showAmin = false;
		_painPointNumber = 4;
		_enabledPainPoints = _painPoints;
		base.Start();
    }

	new void FixedUpdate()
	{
		base.FixedUpdate();
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
				_isBeingHit = true;
			}
			else
			{
				if (!_showAmin)
				{
					_showAmin = true;
					ResetPainPoints();			
					ShowAminPainPoints();
					SpawnOrderSymbolsAmin();
					_expectedPressingOrder = GetExpectedPressingOrderAmin();
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

	protected override void DecideVisiblePainPoints()
	{
		//hide Amin pain points
		foreach(PainPoint pp in _painPointsAmin)
		{
			pp.gameObject.SetActive(false);
		}
	}

	private void ShowAminPainPoints()
	{
		foreach (PainPoint pp in _painPointsAmin)
		{
			pp.gameObject.SetActive(true);
		}
	}

	private void SpawnOrderSymbolsAmin()
	{
		_neededOrderSymbols = _orderSymbolsAmin;
		for (int i = 0; i < _enabledPainPoints.Count; i++)
		{
			Instantiate(_neededOrderSymbols[i], _enabledPainPoints[i].transform.position, Quaternion.identity, _enabledPainPoints[i].transform);
		}
	}

	private List<Key> GetExpectedPressingOrderAmin()
	{
		List<Key> expectedPressingOrder = new List<Key>();
		for (int i = 0; i < _painPointNumber; i++)
		{
			_painPointsOrder.Add(_neededOrderSymbols[i].GetOrderNumber() - 1, _enabledPainPoints[i]);
		}
		for (int i = 0; i < _painPointsOrder.Count; i++)
		{
			expectedPressingOrder.Add(_painPointsOrder[i].GetNeededKey());
		}
		return expectedPressingOrder;
	}

	private void ResetPainPoints()
	{
		_enabledPainPoints = _painPointsAmin;
		_painPointsOrder.Clear();
	}
}
