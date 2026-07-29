using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public static CharacterSpawner Instance { get; private set; }
    private Character _currentCharacter;
    private Character _nextCharacter;
    public Canvas canvas;
    public Camera camera;

	private void Awake()
	{
		Instance = this;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCharacter(Character character, float moveSpeed, int painPointNumber, float timer)
    {
		float orthographicScreenWidth = camera.orthographicSize * camera.aspect;
		Vector3 pos = new Vector3(-orthographicScreenWidth*1.5f, 0, 0);
		_currentCharacter = Instantiate(character, pos, Quaternion.identity, canvas.transform);
        _currentCharacter.SetMoveSpeed(moveSpeed);
        _currentCharacter.SetCurrentState(Character.State.UNCURED);
		_currentCharacter.SetPainPointNumber(painPointNumber);
        _currentCharacter.SetTimer(timer);
        _currentCharacter.SetOrthographicScreenWidth(orthographicScreenWidth);
	}
}
