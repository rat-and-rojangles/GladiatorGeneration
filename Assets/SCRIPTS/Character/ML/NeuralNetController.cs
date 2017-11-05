using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetController : MonoBehaviour {

	public float generationDuration;
	public float maxGenerationVariation;
	private float timeElapsed = 0f;

	[SerializeField]
	private int m_numberOfEnemies = 10;
	public int numberOfEnemies { get { return m_numberOfEnemies; } }
	private EvolutionaryNeuralNetwork net;

	public static NeuralNetController staticRef = null;

	private ControlCharacterML [] enemies;
	private int intialEnemyIndex = 0;
	private Character player = null;

	private float currentGenerationDuration;

	void Awake () {
		NewGenerationDuration ();
		enemies = new ControlCharacterML [numberOfEnemies];
		staticRef = this;
	}

	void OnDestroy () {
		staticRef = null;
	}

	void Update () {
		if (net == null) {
			net = new EvolutionaryNeuralNetwork (player, enemies);
		}
		net.Update ();
		timeElapsed += Time.deltaTime;
		if (timeElapsed >= currentGenerationDuration + ControlCharacterML.RESPAWN_TIME) {
			timeElapsed = 0f;
			net.KillAndRespawn ();
		}
	}

	private void NewGenerationDuration () {
		currentGenerationDuration = generationDuration + Utility.randomPlusOrMinusOne * maxGenerationVariation;
	}
	public void RegisterEnemy (ControlCharacterML characterML) {
		enemies [intialEnemyIndex] = characterML;
		intialEnemyIndex++;
		// InitializeNetIfReady ();
	}
	public void RegisterPlayer (Character player) {
		this.player = player;
		// InitializeNetIfReady ();
	}


	[SerializeField]
	private GameObject enemyMLPrefab;
	[ContextMenu ("Generate Enemies")]
	private void GenerateEnemies () {
		for (int x = 0; x < numberOfEnemies; x++) {
			Instantiate (enemyMLPrefab).transform.parent = transform;
		}
	}
}
