using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public static CharacterSpawner Instance { get; private set; }
	[SerializeField]
	private Canvas _canvas;
	[SerializeField]
	private Camera _camera;
	private Character _currentCharacter;
	//private Character _nextCharacter;
	private float _OrthographicScreenWidth;
	

	private void Awake()
	{
		Instance = this;
	}

	void Start()
    {
		_OrthographicScreenWidth = _camera.orthographicSize * _camera.aspect;
	}

    void Update()
    {
        
    }

    public Character SpawnCharacter(Character character, float moveSpeed, int painPointNumber, float timer, bool isQueueing, float queueXPos, bool isTalking, Transform dialoguePos, DialogueBox dialogueBox)
    {
		//Instantiates character outside of the screen
		Vector3 pos = new Vector3(-_OrthographicScreenWidth*1.5f, 0, 0);
		_currentCharacter = Instantiate(character, pos, Quaternion.identity, _canvas.transform);
        _currentCharacter.SetMoveSpeed(moveSpeed);
        if (isTalking)
        {
			_currentCharacter.SetState(Character.State.TALKING);
            _currentCharacter.SetDialoguePos(dialoguePos);
		}
        else
        {
			if (isQueueing)
			{
				_currentCharacter.SetQueueXPos(queueXPos);
				_currentCharacter.SetState(Character.State.QUEUEING);
			}
			else
			{
				_currentCharacter.SetState(Character.State.UNCURED);
			}
		}
		_currentCharacter.SetPainPointNumber(painPointNumber);
        _currentCharacter.SetTimer(timer);
        
        _currentCharacter.SetOrthographicScreenWidth(_OrthographicScreenWidth);

		//FOR TESTING
		//if (isQueueing)
		//{
		//	_currentCharacter.SetTimer(3f);
		//}
		return _currentCharacter;
	}
}
