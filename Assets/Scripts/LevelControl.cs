using NUnit.Framework;
using System.Collections;
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
	[SerializeField]
	private Camera _camera;
	[SerializeField]
	private List<Transform> _queuePositions = new List<Transform>();
	private CharacterSpawner _spawner;
    //private Character _currentCharacter;
    //private Character _nextCharacter;
    private int _currentCharacterIndex;
	private float _currentCharacterMoveSpeed;
    private int _currentCharacterPointNumber;
    private float _currentCharacterPatienceTimer;
	private float _nextQueueCharacterTimer;
	private int _maxQueueNumber = 3;
	private List<Character> _queue = new List<Character>();
	private enum Phase {EASY, MEDIUM, HARD};
    private Phase _currentPhase = Phase.EASY;

	private void OnEnable()
	{
        Actions.OnLeft += CheckCharacterInQueue;
        Actions.OnWaitingTimerEnded += CheckCharacterInQueue;
        Actions.OnLostLevel += LevelLost;
		Actions.OnLevelPhaseChanged += DecideCurrentCharacterVariables;
	}

	private void OnDisable()
	{
		Actions.OnLeft -= CheckCharacterInQueue;
		Actions.OnWaitingTimerEnded -= CheckCharacterInQueue;
		Actions.OnLostLevel -= LevelLost;
		Actions.OnLevelPhaseChanged -= DecideCurrentCharacterVariables;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		DecideCurrentCharacterVariables();
		//CalculateQueuePositions();
		_spawner = CharacterSpawner.Instance;
        _currentCharacterIndex = 0;
        _winPanel.gameObject.SetActive(false);
        _losePanel.gameObject.SetActive(false);
		StartCoroutine(QueueTimerCoroutine());
		SpawnCharacter(false, 0);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    private Character SpawnCharacter(bool isQueueing, float queueXPos)
    {
		Character character = null;
        if (_currentCharacterIndex < _characterList.Count)
        {
			//null unde nu vreau sa dau dialogueBox
			//_spawner.SpawnCharacter(_characterList[_currentCharacterIndex], _currentCharacterMoveSpeed, _currentCharacterPointNumber, _currentCharacterPatienceTimer, 0, false, _dialoguePos, _dialogueBox);
			character = _spawner.SpawnCharacter(_characterList[_currentCharacterIndex], _currentCharacterMoveSpeed, 4, 1000, isQueueing, queueXPos, false, _dialoguePos, _dialogueBox);
            _currentCharacterIndex++;
			DecidePhase();
		}
        else
        {
            LevelWon();
        }
		return character;
	}

    private void DecideCurrentCharacterVariables()
    {
        DecideCurrentCharacterMoveSpeed();
		DecideCurrentCharacterPointNumber();
		DecideCurrentCharacterTimer();
		DecideNextQueueCharacterTimer();
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
				_currentCharacterPatienceTimer = 6;
				break;
			case Phase.MEDIUM:
				_currentCharacterPatienceTimer = 5;
				break;
			case Phase.HARD:
				_currentCharacterPatienceTimer = 3;
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
		else if (_currentLevel == 3)
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

	private void DecideNextQueueCharacterTimer()
	{
		_nextQueueCharacterTimer = 2f;
	}

	//private void CalculateQueuePositions()
	//{
	//	_queueXPos.Add(3 * _camera.orthographicSize / 8);
	//	_queueXPos.Add(_camera.orthographicSize / 4);
	//	_queueXPos.Add(_camera.orthographicSize / 8);
	//}

	private void CheckCharacterInQueue()
	{
		if(_queue.Count > 0)
		{
			AdvanceQueue();
		}
		else
		{
			SpawnCharacter(false,0);
		}
	}

	private void AdvanceQueue()
	{
		if (_queue[0].GetCurrentState() == Character.State.QUEUEING)
		{
			_queue[0].SetState(Character.State.UNCURED);
		}
		_queue[0].ChangeOirderInLayer(1);
		_queue.RemoveAt(0);
		for(int i=0;i<_queue.Count;i++)
		{
			_queue[i].SetQueueXPos(_queuePositions[i].transform.position.x);
			_queue[i].ChangeOirderInLayer(1);
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

	private IEnumerator QueueTimerCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(_nextQueueCharacterTimer);
			if (_currentCharacterIndex < _characterList.Count && _queue.Count < _maxQueueNumber)
			{
				Character c = SpawnCharacter(true, _queuePositions[_queue.Count].position.x);
				_queue.Add(c);
				c.ChangeOirderInLayer(-_queue.Count*3);
			}
		}
	}
}
