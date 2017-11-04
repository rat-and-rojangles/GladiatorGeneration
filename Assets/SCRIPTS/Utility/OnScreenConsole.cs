using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnScreenConsole : MonoBehaviour {

	public Color m_color;

	private static OnScreenConsole m_staticRef;
	private static OnScreenConsole staticRef {
		get {
			try {
				return m_staticRef;
			}
			catch {
				CreateStaticRef ();
				return m_staticRef;
			}
		}
	}

	private static void CreateStaticRef () {
		GameObject empty = new GameObject ();
		empty.name = "On Screen Console";
		m_staticRef = empty.AddComponent<OnScreenConsole> ();
		m_staticRef.m_color = Color.white;
	}

	public static Color color {
		get {
			try {
				return staticRef.m_color;
			}
			catch {
				CreateStaticRef ();
				return staticRef.m_color;
			}
		}
		set {
			try {
				staticRef.m_color = color;
			}
			catch {
				CreateStaticRef ();
				staticRef.m_color = color;
			}
		}
	}

	private string text = "";

	void Awake () {
		m_staticRef = this;
	}
	void OnDestroy () {
		m_staticRef = null;
	}

	public static void Log (object message) {
		try {
			staticRef.text += message.ToString () + "\n";
		}
		catch {
			CreateStaticRef ();
			staticRef.text += message.ToString () + "\n";
		}
		if (staticRef.text.Length > 1000) {
			staticRef.text = staticRef.text.Substring (staticRef.text.Length - 1000);
		}
	}

	public static void ClearConsole () {
		try {
			staticRef.text = "";
		}
		catch {
			CreateStaticRef ();
			staticRef.text = "";
		}
	}

	void OnGUI () {
		WriteGUI ();
	}

	private void WriteGUI () {
		int w = Screen.width, h = Screen.height;
		GUIStyle style = new GUIStyle ();

		style.alignment = TextAnchor.LowerLeft;
		style.fontSize = h / 12;
		style.normal.textColor = staticRef.m_color;

		Rect rect = new Rect (0, h - style.fontSize * 2, w, h / 4);

		GUI.Label (rect, text, style);
	}
}
