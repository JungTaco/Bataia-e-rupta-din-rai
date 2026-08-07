using UnityEngine;
using UnityEngine.UI;

public class CountdownSlider : MonoBehaviour
{
    [SerializeField]
    private float _timer;
    private Slider _slider;
    private bool _timerInProgress;

	private void OnEnable()
	{
		Actions.OnWonLevel += StopTimer;
	}

	private void OnDisable()
	{
		Actions.OnWonLevel -= StopTimer;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		_slider = GetComponent<Slider>();
        _slider.maxValue = _timer;
        _slider.value = _timer;
		_timerInProgress = true;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_timerInProgress)
        {
			if (_timer >= 0)
			{
				_timer -= Time.deltaTime;
				_slider.value = _timer;
			}
			else
			{
				Actions.OnLostLevel?.Invoke();
			}
		}
    }

    private void StopTimer()
    {
        _timerInProgress = false;
    }
}
