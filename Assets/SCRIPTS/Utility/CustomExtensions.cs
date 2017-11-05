using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomExtensions {

	/// <summary>
	/// Returns a random element from the array.
	/// </summary>
	public static T RandomElement<T> (this T [] array) {
		return array [Random.Range (0, array.Length)];
	}

	/// <summary>
	/// Is this index a valid element for this array?
	/// </summary>
	public static bool WithinBounds<T> (this T [] array, int index) {
		return index >= 0 && index < array.Length;
	}

	/// <summary>
	/// The same color but with a different alpha value.
	/// </summary>
	public static Color ChangedAlpha (this Color c, float a) {
		return new Color (c.r, c.g, c.b, a);
	}

	/// <summary>
	/// non-disruptively set the transform local position
	/// </summary>

	public static void SetLocalPosition (this Transform t, float? x, float? y, float? z) {
		float xPrime = x == null ? t.localPosition.x : x.GetValueOrDefault ();
		float yPrime = y == null ? t.localPosition.y : y.GetValueOrDefault ();
		float zPrime = z == null ? t.localPosition.z : z.GetValueOrDefault ();
		t.localPosition += -t.localPosition + new Vector3 (xPrime, yPrime, zPrime);
	}

	/// <summary>
	/// non-disruptively set the transform world position
	/// </summary>
	public static void SetPosition (this Transform transform, Vector3 position) {
		transform.position += -transform.position + position;
	}

	/// <summary>
	/// non-disruptively set the rigidbody velocity
	/// </summary>
	public static void SetVelocity (this Rigidbody2D rigidbody2D, Vector2 velocity) {
		rigidbody2D.velocity += -rigidbody2D.velocity + velocity;
	}

	/// <summary>
	/// returns 1 if positive, -1 if negative, 0 if zero. epsilon = 0.000001f
	/// </summary>
	public static int Sign (this float x) {
		float epsilon = 0.000001f;
		if (x > epsilon) {
			return 1;
		}
		else if (x < -epsilon) {
			return -1;
		}
		else {
			return 0;
		}
	}

	/// <summary>
	/// like a reverse truncate
	/// </summary>
	public static float DecimalPart (this float value) {
		return value - ((int)value);
	}

	/// <summary>
	/// Normalize the number until it is within a range of 0 (inclusive) and 1(exclusive).
	/// </summary>
	public static float Normalized01 (this float value) {
		float newValue = value;
		while (newValue < 0f) {
			newValue += 1f;
		}
		while (newValue >= 1f) {
			newValue -= 1f;
		}
		return newValue;
	}

	/// <summary>
	/// Return the value clamped between 0 and 1.
	/// </summary>
	public static float Clamped01 (this float value) {
		return Mathf.Clamp01 (value);
	}

	/// <summary>
	/// Checks if some number is close to this within some percent of this value.
	/// </summary>
	public static bool RoughlyEquals (this float x, float other, float fraction = 0.001f) {
		float fractionPrime = Mathf.Clamp01 (fraction);
		return other < x + x * fractionPrime && other > x - x * fractionPrime;
	}

	/// <summary>
	/// If 0, returns 1. Else, returns 0. Intended to swap 0 and 1
	/// </summary>
	public static int Other (this int x) {
		if (x == 0) {
			return 1;
		}
		else {
			return 0;
		}
	}

	/// <summary>
	/// Insert an element before index 0
	/// </summary>
	public static void PushBeginning<T> (this List<T> list, T element) {
		list.Insert (0, element);
	}

	/// <summary>
	/// Remove element at index 0
	/// </summary>
	public static T PopBeginning<T> (this List<T> list) {
		T temp = list [0];
		list.RemoveAt (0);
		return temp;
	}

	/// <summary>
	/// Remove last element
	/// </summary>
	public static T PopLast<T> (this List<T> list) {
		if (list.Count == 0) {
			return default (T);
		}
		else {
			T temp = list [list.Count - 1];
			list.RemoveAt (list.Count - 1);
			return temp;
		}
	}

	/// <summary>
	/// Last element in list
	/// </summary>
	public static T Last<T> (this List<T> list) {
		if (list.Count == 0) {
			return default (T);
		}
		else {
			return list [list.Count - 1];
		}
	}
}
