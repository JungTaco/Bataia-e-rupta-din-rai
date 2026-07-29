using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
	public enum ChallengeType { NORMAL, AMIN, CROSS, SUPERSICK };
	[SerializeField]
    private List<Character> _characterList = new List<Character>();
	private CharacterSpawner _spawner;
    //private Character _currentCharacter;
    //private Character _nextCharacter;
    private int _currentCharacterIndex;
    //serialized si il pun din inspector?
    private int _currentLevel = 1;
    private float _moveSpeed = 0.15f;
    private enum Phase {EASY, MEDIUM, HARD};
    private Phase _currentPhase = Phase.EASY;

	private void OnEnable()
	{
        Actions.OnLeft += SpawnCharacter;
	}

	private void OnDisable()
	{
		Actions.OnLeft -= SpawnCharacter;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {

        //switch (_currentLevel)
        //{
        //    case 1:
        //        switch (_currentPhase)
        //        {
        //            case Phase.EASY:
        //                break;
        //            case Phase.MEDIUM:
        //                break;
        //            case Phase.HARD:
        //                break;
        //            default:
        //                Debug.Log("invalid level phase");
        //                break;
        //        }
        //        break;
        //    case 2: 
        //        break;
        //    case 3:
        //        break;
        //    case 4:
        //        break;
        //    default:
        //        Debug.Log("Invalid level");
        //        break;
        //}
		_spawner = CharacterSpawner.Instance;
        _currentCharacterIndex = 0;
        SpawnCharacter();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnCharacter()
    {
        //if currentindex < count
        if (_currentCharacterIndex < _characterList.Count)
        {
            _spawner.SpawnCharacter(_characterList[_currentCharacterIndex], _moveSpeed, 2, 4f, ChallengeType.NORMAL);
            _currentCharacterIndex++;
        }
	}
}
