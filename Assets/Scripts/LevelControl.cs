using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
	[SerializeField]
    private List<Character> _characterList = new List<Character>();
    [SerializeField]
    private SpriteRenderer _winPanel;
	[SerializeField]
	private SpriteRenderer _losePanel;
    [SerializeField]
    private Transform _dialoguePos;
    [SerializeField]
    private DialogueBox _dialogueBox;
	[SerializeField]
	private int _currentLevel;
	private CharacterSpawner _spawner;
    //private Character _currentCharacter;
    //private Character _nextCharacter;
    private int _currentCharacterIndex;
	
	private float _currentCharacterMoveSpeed;
    private int _currentCharacterPointNumber;
    private float _currentCharacterTimer;
    private enum Phase {EASY, MEDIUM, HARD};
    private Phase _currentPhase = Phase.EASY;

	private void OnEnable()
	{
        Actions.OnLeft += SpawnCharacter;
        Actions.OnWaitingTimerEnded += SpawnCharacter;
        Actions.OnLostLevel += LevelLost;
		Actions.OnLevelPhaseChanged += DecideCurrentCharacterVariables;
	}

	private void OnDisable()
	{
		Actions.OnLeft -= SpawnCharacter;
		Actions.OnWaitingTimerEnded -= SpawnCharacter;
		Actions.OnLostLevel -= LevelLost;
		Actions.OnLevelPhaseChanged -= DecideCurrentCharacterVariables;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		DecideCurrentCharacterVariables();
		_spawner = CharacterSpawner.Instance;
        _currentCharacterIndex = 0;
        _winPanel.gameObject.SetActive(false);
        _losePanel.gameObject.SetActive(false);
		SpawnCharacter();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnCharacter()
    {
        //get _moveSpeed
        if (_currentCharacterIndex < _characterList.Count)
        {
            //null unde nu vreau sa dau dialogueBox
            _spawner.SpawnCharacter(_characterList[_currentCharacterIndex], _currentCharacterMoveSpeed, _currentCharacterPointNumber, _currentCharacterTimer, false, _dialoguePos, _dialogueBox);
            _currentCharacterIndex++;
			DecidePhase();
		}
        else
        {
            LevelWon();
        }
	}

    private void DecideCurrentCharacterVariables()
    {
        DecideCurrentCharacterMoveSpeed();
		DecideCurrentCharacterPointNumber();
		DecideCurrentCharacterTimer();
	}

	private void DecideCurrentCharacterMoveSpeed()
    {
        switch (_currentPhase)
        {
            case Phase.EASY:
				_currentCharacterMoveSpeed = .15f;
                break;
            case Phase.MEDIUM:
                _currentCharacterMoveSpeed = .15f;
				break;
			case Phase.HARD:
                _currentCharacterMoveSpeed = .5f;
				break;
			default:
                break;
		}     
    }

	private void DecideCurrentCharacterPointNumber()
	{
		switch (_currentPhase)
		{
			case Phase.EASY:
				_currentCharacterPointNumber = 2;
				break;
			case Phase.MEDIUM:
				_currentCharacterPointNumber = 3;
				break;
			case Phase.HARD:
				_currentCharacterPointNumber = 4;
				break;
			default:
				break;
		}
	}

	private void DecideCurrentCharacterTimer()
	{
		switch (_currentPhase)
		{
			case Phase.EASY:
				_currentCharacterTimer = 6;
				break;
			case Phase.MEDIUM:
				_currentCharacterTimer = 5;
				break;
			case Phase.HARD:
				_currentCharacterTimer = 3;
				break;
			default:
				break;
		}
	}

	private void DecidePhase()
	{
		if (_currentLevel == 1)
		{
			if (_currentCharacterIndex == 5)
			{
				_currentPhase = Phase.MEDIUM;
				Actions.OnLevelPhaseChanged?.Invoke();
			}
			else if (_currentCharacterIndex == 10)
			{
				_currentPhase = Phase.HARD;
				Actions.OnLevelPhaseChanged?.Invoke();
			}
		}
	}

	private void LevelLost()
    {
		_characterList.Clear();
		_losePanel.gameObject.SetActive(true);
	}

    private void LevelWon()
    {
		_winPanel.gameObject.SetActive(true);
	}
}
