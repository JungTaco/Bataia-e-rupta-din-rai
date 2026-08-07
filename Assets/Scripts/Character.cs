using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    [SerializeField]
	protected Sprite _uncuredSprite;
	[SerializeField]
	protected Sprite _hitSprite;
	[SerializeField]
	protected Sprite _curedSprite;
	[SerializeField]
    protected List<PainPoint> _painPoints = new List<PainPoint>();
	[SerializeField]
	protected List<OrderSymbol> _orderSymbols = new List<OrderSymbol>();
	[SerializeField]
	protected Transform _wrongHitPosition;
	protected List<PainPoint> _enabledPainPoints = new List<PainPoint>();
	protected List<Key> _expectedPressingOrder = new List<Key>();
	protected List<OrderSymbol> _neededOrderSymbols = new List<OrderSymbol>();
	protected List<OrderSymbol> _spawnedOrderSymbols = new List<OrderSymbol>();
	protected SortedDictionary<int, PainPoint> _painPointsOrder = new SortedDictionary<int, PainPoint>();
	protected int _currentExpectedKeyIndex = 0;
	protected float _moveSpeed;
	protected SpriteRenderer _spriteRenderer;
	protected int _painPointNumber;
	protected float _timer;
	protected float _OrthographicScreenWidth;
	protected bool _isBeingHit;
	protected float _queueXPos;
	protected Transform _dialoguePos;
	protected bool _symbolsSpawned;
	protected bool _timerActive;
	protected bool _characterIsLeaving;
	protected float _isBeingHitTimerCurrentValue;
	protected float _isBeingHitTimer;
	

	public enum State {TALKING, QUEUEING, UNCURED, CURED, ANGRY};
	protected State _currentState; 

    public State GetCurrentState()
    {
        return _currentState;
    }

    public void SetState(State newState)
    {
        _currentState = newState;
	}

	public State GetState()
	{
		return _currentState;
	}

    public void SetMoveSpeed(float newMoveSpeed)
    {
        _moveSpeed = newMoveSpeed;
    }

    public void SetPainPointNumber(int painPointNumber)
    {
        _painPointNumber = painPointNumber;
    }

	public void SetTimer(float timer)
	{
        _timer = timer;
	}

	public void SetQueueXPos(float XPos)
	{
		_queueXPos = XPos;
	}
	public void SetOrthographicScreenWidth (float newOrthographicScreenWidth)
    {
        _OrthographicScreenWidth = newOrthographicScreenWidth;
    }

	public void SetDialoguePos(Transform dialoguePos)
	{
		_dialoguePos = dialoguePos;
	}

	protected void OnEnable()
	{
        Actions.OnCured += ChangeSprite;
		Actions.OnCured += StopTimer;
		Actions.OnHit += ChangeSprite;
        Actions.OnHitFinished += ChangeSprite;
		Actions.OnLostLevel += GetsAngry;
	}

	protected void OnDisable()
	{
		Actions.OnCured -= ChangeSprite;
		Actions.OnCured -= StopTimer;
		Actions.OnHit -= ChangeSprite;
		Actions.OnHitFinished -= ChangeSprite;
		Actions.OnLostLevel -= GetsAngry;
	}

	private void Awake()
	{
		_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		_symbolsSpawned = false;
	}

	protected void Start()
	{
		DecideVisiblePainPoints();
		SpawnSymbols();
		_expectedPressingOrder = GetExpectedPressingOrder();
        _isBeingHit = false;
		_timerActive = true;
		_characterIsLeaving = false;
		_isBeingHitTimer = .2f;
		_isBeingHitTimerCurrentValue = _isBeingHitTimer;
	}

	protected void FixedUpdate()
	{
		if ((_currentState == State.UNCURED || _currentState == State.TALKING) && transform.position.x < 0)
		{
			Move();
		}
		else if (_currentState == State.UNCURED)
		{
			CheckRightKeysArepressed();
		}
		else if ((_currentState == State.CURED || _currentState == State.ANGRY) && !_isBeingHit && transform.position.x < (_OrthographicScreenWidth * 1.5))
		{
			Leave();
		}
		else if (_currentState == State.QUEUEING && transform.position.x < _queueXPos)
		{
			Move();
		}
		else if (transform.position.x >= (_OrthographicScreenWidth * 1.5))
		{
			Destroy(gameObject);
		}
		if (_timerActive)
		{
			CalculatCharacterTimer();
		}
		if (_isBeingHit)
		{
			CalculateIsBeingHitTime();
		}
	}

	protected void CalculatCharacterTimer()
	{
		if (_timer >= 0)
		{
			_timer -= Time.deltaTime;
		}
		else
		{
			if (!_characterIsLeaving)
			{
				_currentState = State.ANGRY;
				Actions.OnMistakeMade?.Invoke();
				_characterIsLeaving = true;
				Actions.OnLeft?.Invoke();
			}
		}
	}

	protected void CalculateIsBeingHitTime()
	{
		if (_isBeingHitTimerCurrentValue > 0)
		{
			_isBeingHitTimerCurrentValue -= Time.deltaTime;
		}
		else
		{
			Actions.OnHitFinished?.Invoke();
			ResetIsBeingHitTimer();
			_isBeingHit = false;
		}
	}

	protected void ResetIsBeingHitTimer()
	{
		_isBeingHitTimerCurrentValue = _isBeingHitTimer;
	}

	public void ChangeOirderInLayer(int modification)
	{
		if (!_symbolsSpawned)
		{
			StartCoroutine(SymbolsSpawnedChecker(modification));
		}
		else
		{
			_spriteRenderer.sortingOrder += modification;
			var painPointsSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer ppsr in painPointsSpriteRenderers)
			{
				//ignore parent
				if (ppsr != _spriteRenderer)
				{
					ppsr.sortingOrder = _spriteRenderer.sortingOrder + 1;

					foreach(OrderSymbol os in _spawnedOrderSymbols)
					{
						os.GetComponent<SpriteRenderer>().sortingOrder = _spriteRenderer.sortingOrder + 2;
					}
					//var symbolsSpriteRenderers = ppsr.GetComponentsInChildren<SpriteRenderer>();
					//foreach (SpriteRenderer osr in symbolsSpriteRenderers)
					//{
					//	//ignore parent
					//	if (osr != ppsr)
					//	{
					//		Debug.Log("THIS: " + this + " PAIN POINT: " + ppsr + " SYMBOL: " + osr);
					//		//Debug.Log(_spriteRenderer.sortingOrder + 2);
					//		Debug.Log("INITAL ORDER: "+ osr.sortingOrder);
					//		//osr.sortingOrder = _spriteRenderer.sortingOrder + 2;
					//		osr.sortingOrder = 100;
					//		Debug.Log("FINAL ORDER: " + osr.sortingOrder);
					//	}
					//}
					//Debug.Log(ppsr + " "+symbolsSpriteRenderers.Count());
				}
			}
		}
	}

	protected void Move()
    {
		float newPosX = transform.position.x + 0.5f * _moveSpeed;
		transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);
	}

	protected void Leave()
    {
		float newPosX = transform.position.x + 0.5f * _moveSpeed;
		transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);
	}

    protected virtual void DecideVisiblePainPoints()
    {
		//shuffles list of pain points
		List<PainPoint> shuffledPainPoints = Shuffle(_painPoints);
		//chooses the pain points to keep enabled (truncates to how many are needed)
		List<PainPoint> truncatedPainPoints = TruncateList(shuffledPainPoints);
        _enabledPainPoints = truncatedPainPoints;

        //disables pain points
        for(int i = 0; i < _painPoints.Count; i++)
        {
            if (!_enabledPainPoints.Contains(_painPoints[i]))
            {
				_painPoints[i].gameObject.SetActive(false);
			}
        }
	}

	protected List<OrderSymbol> GetNeededOrderSymbols()
    {
        //truncated symbol list to how many are needed
        List<OrderSymbol> truncatedOrderSymbols = TruncateList(_orderSymbols);
        //shuffles list
        List<OrderSymbol> shuffledOrderSymbols = Shuffle(truncatedOrderSymbols);
        return shuffledOrderSymbols;
	}

	//override for different challenges
	protected virtual void SpawnSymbols()
    {
	    _neededOrderSymbols = GetNeededOrderSymbols();
        for(int i=0; i < _enabledPainPoints.Count; i++)
        {
            OrderSymbol orderSymbol = Instantiate(_neededOrderSymbols[i], _enabledPainPoints[i].transform.position, Quaternion.identity, _enabledPainPoints[i].transform);
			_spawnedOrderSymbols.Add(orderSymbol);
		}
		_symbolsSpawned = true;
	}

	protected List<Key> GetExpectedPressingOrder()
    {
        List<Key> expectedPressingOrder = new List<Key>();
        for (int i = 0; i < _painPointNumber; i++)
        {
            _painPointsOrder.Add(_neededOrderSymbols[i].GetOrderNumber()-1, _enabledPainPoints[i]);
        }
        for(int i = 0; i < _painPointsOrder.Count; i++)
        {
            expectedPressingOrder.Add(_painPointsOrder[i].GetNeededKey());
		}
        return expectedPressingOrder;
	}

	protected virtual void CheckRightKeysArepressed()
    {
		if (Keyboard.current[_expectedPressingOrder[_currentExpectedKeyIndex]].wasPressedThisFrame)
		{
			_painPointsOrder[_currentExpectedKeyIndex].gameObject.SetActive(false);
            _isBeingHit = true;
			Actions.OnHit?.Invoke();
            Actions.OnHitCoordinates?.Invoke(_painPointsOrder[_currentExpectedKeyIndex].transform.position);
			if (_currentExpectedKeyIndex < _painPointNumber - 1)
            {
				_currentExpectedKeyIndex++;
				_isBeingHit = true;
			}       
			else
			{
                _currentState = State.CURED;
				Actions.OnCured?.Invoke();
				Actions.OnLeft?.Invoke();
			}
		}
		else if (Keyboard.current.anyKey.wasPressedThisFrame)
		{
			Actions.OnHit?.Invoke();
			Actions.OnHitCoordinates?.Invoke(_wrongHitPosition.position);
			Actions.OnMistakeMade?.Invoke();
			_isBeingHit = true;
		}
	}

	protected void ChangeSprite()
    {
		if (_isBeingHit)
		{
			_spriteRenderer.sprite = _hitSprite;
		}
		else if (_currentState == State.CURED)
        {
			_spriteRenderer.sprite = _curedSprite;
		}
		else if (_currentState == State.UNCURED)
		{
			_spriteRenderer.sprite = _uncuredSprite;
		}
	}

	protected IEnumerator SymbolsSpawnedChecker(int modification)
	{
		while (!_symbolsSpawned)
		{
			yield return null;
		}
		ChangeOirderInLayer(modification);
		StopCoroutine(SymbolsSpawnedChecker(modification));
	}

	protected void StopTimer()
	{
		_timerActive = false;
	}

	protected List<OrderSymbol> TruncateList(List<OrderSymbol> list)
    {
        List<OrderSymbol> newList = list.GetRange(0, _painPointNumber);
        return newList;
    }

	protected List<PainPoint> TruncateList(List<PainPoint> list)
	{
		List<PainPoint> newList = list.GetRange(0, _painPointNumber);
		return newList;
	}

	protected List<PainPoint> Shuffle(List<PainPoint> list)
    {
		System.Random rng = new System.Random();
		List<PainPoint> shuffledList = list.OrderBy(x => rng.Next()).ToList();
        return shuffledList;
	}

	protected List<OrderSymbol> Shuffle(List<OrderSymbol> list)
    {
		System.Random rng = new System.Random();
		List<OrderSymbol> shuffledList = list.OrderBy(x => rng.Next()).ToList();
        return shuffledList;
	}

	private void GetsAngry()
	{
		_currentState = State.ANGRY;
	}
}
