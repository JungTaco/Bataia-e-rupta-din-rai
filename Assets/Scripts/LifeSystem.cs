using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LifeSystem : MonoBehaviour
{
	private List<SpriteRenderer> _currentLives = new List<SpriteRenderer>();
	private List<SpriteRenderer> _lives = new List<SpriteRenderer>();

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Awake()
    {
        _currentLives = GetComponentsInChildren<SpriteRenderer>().ToList();
        _lives = GetComponentsInChildren<SpriteRenderer>().ToList();
	}

    // Update is called once per frame
    void Update()
    {

    }

    public void LoseLife()
    {
        if( _currentLives.Count > 0)
        {
			_currentLives[_currentLives.Count - 1].gameObject.SetActive(false);
			_currentLives.RemoveAt(_currentLives.Count - 1);
		}
	}
}
