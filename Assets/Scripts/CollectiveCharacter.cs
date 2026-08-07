using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CollectiveCharacter : Character
{
	[SerializeField]
	protected List<PainPoint> _painPoints2 = new List<PainPoint>();
	[SerializeField]
	protected List<PainPoint> _painPoints3 = new List<PainPoint>();
	[SerializeField]
	protected List<PainPoint> _painPoints4 = new List<PainPoint>();
	[SerializeField]
	protected List<PainPoint> _painPoints5 = new List<PainPoint>();
	protected List<List<PainPoint>> _characterPainPointsLists = new List<List<PainPoint>>();
	private int _currentCharacterPainPointsListsIndex;

	new void Start()
    {
		_timer = 9f;
		_currentCharacterPainPointsListsIndex = 0;
		_painPointNumber = 4;
		_enabledPainPoints = _painPoints;
		InitializeCharacterPainPointsLists();
		base.Start();
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
    }

	protected void InitializeCharacterPainPointsLists()
	{
		_characterPainPointsLists.Add(_painPoints);
		_characterPainPointsLists.Add(_painPoints2);
		_characterPainPointsLists.Add(_painPoints3);
		_characterPainPointsLists.Add(_painPoints4);
		_characterPainPointsLists.Add(_painPoints5);
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
				if (_currentCharacterPainPointsListsIndex < _characterPainPointsLists.Count-1)
				{
					_currentCharacterPainPointsListsIndex++;
					DecideVisiblePainPoints();
					SpawnSymbols();
					_painPointsOrder.Clear();
					_expectedPressingOrder = GetExpectedPressingOrder();
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
			Actions.OnHit?.Invoke();
			Actions.OnHitCoordinates?.Invoke(_wrongHitPosition.position);
			Actions.OnMistakeMade?.Invoke();
			_isBeingHit = true;
		}
	}

	protected override void DecideVisiblePainPoints()
	{
		//disable all pain points from inactive lists and enable the ones from the active list
		for(int i = 0; i < _characterPainPointsLists.Count; i++)
		{
			if (i != _currentCharacterPainPointsListsIndex)
			{
				DisablePainPointsInList(_characterPainPointsLists[i]);
			}
			else
			{
				EnablePainPointsInList(_characterPainPointsLists[i]);
			}
		}
		_enabledPainPoints = _characterPainPointsLists[_currentCharacterPainPointsListsIndex];
	}

	protected void DisablePainPointsInList(List<PainPoint> painPoints)
	{
		foreach (PainPoint pp in painPoints)
		{
			pp.gameObject.SetActive(false);
		}
	}

	protected void EnablePainPointsInList(List<PainPoint> painPoints)
	{
		foreach (PainPoint pp in painPoints)
		{
			pp.gameObject.SetActive(true);
		}
	}
}
