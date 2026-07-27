using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Character : MonoBehaviour
{
    [SerializeField]
    private Texture2D _uncuredSprite;
	[SerializeField]
	private Texture2D _hitSprite;
	[SerializeField]
	private Texture2D _curedSprite;
	[SerializeField]
	private ChallengeType _challengeType;
	[SerializeField]
    private List<GameObject> _painPoints = new List<GameObject>();
	[SerializeField]
	private List<Object> _orderSymbols = new List<Object>();
	private List<GameObject> _enabledPainPoints = new List<GameObject>();
	private float _moveSpeed;
	private Texture2D _currentSprite;
    private int _painPointNumber;
    private float _timer;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		DecideVisiblePainPoints();
		SpawnSymbols();
	}

    // Update is called once per frame
    void Update()
    {
        if(_currentState == state.UNCURED && transform.position.x < 0)
        {
            float newPosX = transform.position.x+0.5f*_moveSpeed;
            transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);
        }
    }

    private void DecideVisiblePainPoints()
    {
        //shuffles list of pain points
        List<GameObject> shuffledPainPoints = Shuffle(_painPoints);
        //chooses the pain points to keep enabled (truncates to how many are needed)
        List<GameObject> truncatedPainPoints = TruncateList(shuffledPainPoints);
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

    private List<Object> GetNeededOrderSymbols()
    {
        //truncated symbol list to how many are needed
        List<Object> truncatedOrderSymbols = TruncateList(_orderSymbols);
        //shuffles list
        List<Object> shuffledOrderSymbols = Shuffle(truncatedOrderSymbols);
        return shuffledOrderSymbols;
	}

    //override for different challenges
    private void SpawnSymbols()
    {
        List<Object> neededOrderSymbols = GetNeededOrderSymbols();
        for(int i=0; i < _enabledPainPoints.Count; i++)
        {
            Instantiate(neededOrderSymbols[i], _enabledPainPoints[i].transform.position, Quaternion.identity, _enabledPainPoints[i].transform);
		}
    }

    private List<Object> TruncateList(List<Object> list)
    {
        List<Object> newList = list.GetRange(0, _painPointNumber);
        return newList;
    }

	private List<GameObject> TruncateList(List<GameObject> list)
	{
		List<GameObject> newList = list.GetRange(0, _painPointNumber);
		return newList;
	}

	private List<GameObject> Shuffle(List<GameObject> list)
    {
		System.Random rng = new System.Random();
		List<GameObject> shuffledList = list.OrderBy(x => rng.Next()).ToList();
        return shuffledList;
	}

	private List<Object> Shuffle(List<Object> list)
    {
		System.Random rng = new System.Random();
		List<Object> shuffledList = list.OrderBy(x => rng.Next()).ToList();
        return shuffledList;
	}
}
