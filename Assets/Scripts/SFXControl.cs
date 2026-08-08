using UnityEngine;

public class SFXControl : MonoBehaviour
{
    [SerializeField]
    private AudioSource _mistakeAudio;
	[SerializeField]
	private AudioSource _hitAudio;

	private void OnEnable()
	{
		Actions.OnMistakeMade += PlayMistakeAudio;
		Actions.OnHit += PlayHitAudio;
	}

	private void OnDisable()
	{
		Actions.OnMistakeMade -= PlayMistakeAudio;
		Actions.OnHit -= PlayHitAudio;
	}

	private void PlayMistakeAudio()
    {
		_hitAudio.mute = true;
		_mistakeAudio.Play();
	}

	private void PlayHitAudio()
	{
		_hitAudio.mute = false;
		_hitAudio.Play();
	}
}
