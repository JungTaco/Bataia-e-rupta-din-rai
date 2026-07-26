using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
    [SerializeField]
    private List<Character> _characterList = new List<Character>();
	private CharacterSpawner _spawner;
    private int _currentLevel = 1;
    private float _moveSpeed = 0.25f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		_spawner = CharacterSpawner.Instance;
        _spawner.SpawnCharacter(_characterList[0], _moveSpeed, 2);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
