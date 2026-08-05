using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LifeSystem : MonoBehaviour
{
	private List<SpriteRenderer> _lives = new List<SpriteRenderer>();

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Awake()
    {
        _lives = GetComponentsInChildren<SpriteRenderer>().ToList();
	}

    // Update is called once per frame
    void Update()
    {

    }

    public void LoseLife()
    {
        if( _lives.Count > 0)
        {
			_lives[_lives.Count - 1].gameObject.SetActive(false);
			_lives.RemoveAt(_lives.Count - 1);
		}
        
	}
}
