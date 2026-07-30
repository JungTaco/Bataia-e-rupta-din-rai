using System;
using UnityEngine;

public static class Actions
{
	public static Action OnCured;
	public static Action OnHit;
	public static Action<Vector3> OnHitCoordinates;
	public static Action OnHitFinished;
	public static Action OnWaitingTimerEnded;
	public static Action OnMistakeMade;
	public static Action OnLeft;
	public static Action OnLostLevel;
	public static Action OnWonLevel;
}
