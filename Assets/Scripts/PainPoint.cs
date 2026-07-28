using UnityEngine;
using UnityEngine.InputSystem;

public class PainPoint : MonoBehaviour
{
	[SerializeField]
	private Key _neededKey;
	//public enum Key { UP, DOWN, LEFT, RIGHT, A, M, I, N };

	public Key GetNeededKey()
	{
		return _neededKey;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {

	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
