using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MainCharacterControl : MonoBehaviour
{
    [SerializeField]
    private LifeSystem _lifeSystem;
    [SerializeField]
    private SpriteRenderer _idleHand;
	[SerializeField]
	private SpriteRenderer _hittingHand;
	private int _lives;

	private void OnEnable()
	{
        Actions.OnMistakeMade += LoseLife;
        Actions.OnHitCoordinates += Hit;
        Actions.OnHitFinished += FinishHit;
	}

	private void OnDisable()
	{
		Actions.OnMistakeMade -= LoseLife;
		Actions.OnHitCoordinates -= Hit;
		Actions.OnHitFinished -= FinishHit;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        _lives = 3;
        _hittingHand.gameObject.SetActive(false);
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

    private void Hit(Vector3 pos)
    {
        _idleHand.gameObject.SetActive(false);
        _hittingHand.transform.position = pos;
        _hittingHand.gameObject.SetActive(true);
    }

    private void FinishHit()
    {
		_idleHand.gameObject.SetActive(true);
		_hittingHand.gameObject.SetActive(false);
	}
}
