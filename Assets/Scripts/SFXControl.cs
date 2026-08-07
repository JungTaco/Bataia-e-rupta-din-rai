using UnityEngine;

public class SFXControl : MonoBehaviour
{
    [SerializeField]
    private AudioSource _mistakeAudio;

	private void OnEnable()
	{
		Actions.OnMistakeMade += PlayMistkaeAudio;
	}

	private void OnDisable()
	{
		Actions.OnMistakeMade -= PlayMistkaeAudio;
	}

	private void PlayMistkaeAudio()
    {
        _mistakeAudio.Play();
	}
}
