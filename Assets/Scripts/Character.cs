using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private Texture2D _uncuredSprite;
	[SerializeField]
	private Texture2D _hitSprite;
	[SerializeField]
	private Texture2D _curedSprite;
    [SerializeField]
    private List<Object> _painPointsPos = new List<Object>();
	private float _moveSpeed;
	private Texture2D _currentSprite;
    private int _painPointNumber;
    private float _timer;
    private int _paintPointsNumber;
    //enum state (waiting, uncured, hit, cured)
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
}
