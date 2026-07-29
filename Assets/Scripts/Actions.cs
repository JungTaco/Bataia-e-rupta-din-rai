using System;
using UnityEngine;

public static class Actions
{
	public static Action OnCured;
	public static Action OnHit;
	public static Action<Vector3> OnHitCoordinates;
	public static Action OnHitFinished;
	public static Action OnWrongKeyPressed;
}
