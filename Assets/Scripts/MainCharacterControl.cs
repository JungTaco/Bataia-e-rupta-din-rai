using UnityEngine;

public class MainCharacterControl : MonoBehaviour
{
    [SerializeField]
    private LifeSystem _lifeSystem;
    private int _lives;

	private void OnEnable()
	{
        Actions.OnWrongKeyPressed += LoseLife;
	}

	private void OnDisable()
	{
		Actions.OnWrongKeyPressed -= LoseLife;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        _lives = 3;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoseLife()
    {
		_lifeSystem.LoseLife();
		if (_lives > 1)
        {
            _lives--;  
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        //invoke action pentru game over?
        Debug.Log("ai murit");
    }
}
