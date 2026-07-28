using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
    [SerializeField]
    private List<Character> _characterList = new List<Character>();
	private CharacterSpawner _spawner;
    //serialized si il pun din inspector?
    private int _currentLevel = 1;
    private float _moveSpeed = 0.25f;
    private enum Phase {EASY, MEDIUM, HARD};
    private Phase _currentPhase = Phase.EASY;

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
        _spawner.SpawnCharacter(_characterList[0], _moveSpeed, 2);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
