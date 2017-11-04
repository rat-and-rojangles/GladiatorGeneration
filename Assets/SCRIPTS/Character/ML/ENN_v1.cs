using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ENN_v1 {
	private float [,,] W_1;
	private float [,] B_1;

	private float [,,] W_2;
	private float [,] B_2;
	private float [] lr;

	private struct WinnerDistance {
		public WinnerDistance (float distance, int index) {
			this.distance = distance;
			this.index = index;
		}
		public float distance;
		public int index;
	}
	private WinnerDistance [] winners = new WinnerDistance [3];

	public UnityEngine.Color [] enemyColors;
	private ControlCharacterML [] enemies;
	private Character player;

	public ENN_v1 (Character player, ControlCharacterML [] enemies) {
		ResetWinners ();
		this.player = player;
		this.enemies = enemies;
		enemyColors = new Color [NeuralNetController.staticRef.numberOfEnemies];
		W_1 = new float [NeuralNetController.staticRef.numberOfEnemies, 6, 6]; //12 inputs, your momentum vector, their momentum vector, the x y z distances
		B_1 = new float [NeuralNetController.staticRef.numberOfEnemies, 6];
		W_2 = new float [NeuralNetController.staticRef.numberOfEnemies, 3, 6];
		B_2 = new float [NeuralNetController.staticRef.numberOfEnemies, 3];
		lr = new float [NeuralNetController.staticRef.numberOfEnemies];
		for (int i = 0; i < NeuralNetController.staticRef.numberOfEnemies; i++) {
			lr [i] = 0.01f;
			// enemyColors [i] = new Color (UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
			enemyColors [i] = new ColorHSV (i * 1f / NeuralNetController.staticRef.numberOfEnemies, 1f, 1f, 1f);
			enemies [i].character.color = enemyColors [i];

		}
		for (int i = 0; i < NeuralNetController.staticRef.numberOfEnemies; i++) {
			for (int j = 0; j < 6; j++) {
				for (int k = 0; k < 6; k++) {
					W_1 [i, j, k] = 2.0f * (UnityEngine.Random.value - .5f);
				}
				B_1 [i, j] = 2.0f * (UnityEngine.Random.value - .5f);
			}
		}

		for (int i = 0; i < NeuralNetController.staticRef.numberOfEnemies; i++) {
			for (int j = 0; j < 3; j++) {
				for (int k = 0; k < 6; k++) {
					W_2 [i, j, k] = 2.0f * (UnityEngine.Random.value - .5f);
				}
				B_2 [i, j] = 2.0f * (UnityEngine.Random.value - .5f);
			}
		}

	}

	private void ResetWinners () {
		for (int x = 0; x < winners.Length; x++) {
			winners [x] = new WinnerDistance (Mathf.Infinity, x);
		}
	}
	private void RippleWinners () {
		if (winners [2].distance < winners [1].distance) {
			Utility.Swap (ref winners [2], ref winners [1]);
		}
		if (winners [1].distance < winners [0].distance) {
			Utility.Swap (ref winners [1], ref winners [0]);
		}
	}

	// Calculates info from player
	public void Update () {
		float [,] data = new float [NeuralNetController.staticRef.numberOfEnemies, 6];
		for (int x = 0; x < NeuralNetController.staticRef.numberOfEnemies; x++) {
			data [x, 0] = enemies [x].character.transform.position.x - player.transform.position.x;
			data [x, 1] = enemies [x].character.transform.position.y - player.transform.position.y;
			float currentDistance = Vector2.Distance (enemies [x].character.transform.position, player.transform.position);
			if (currentDistance < winners [2].distance) {
				winners [2] = new WinnerDistance (currentDistance, x);
				RippleWinners ();
			}
			data [x, 2] = enemies [x].character.velocity.x;
			data [x, 3] = enemies [x].character.velocity.y;
			data [x, 4] = player.velocity.x;
			data [x, 5] = player.velocity.y;
		}
		UpdateControllers (GetMoves (data));
	}

	/// <summary>
	/// INPUT
	/// First dimension: for each entity
	/// Second dimension: xToPlayer, yToPlayer, myXVel, myYVel, playerXVel, playerYVel
	/// OUTPUT
	/// 0: jump?
	/// 1: move left
	/// 2: move right
	/// </summary>
	bool [,] GetMoves (float [,] inp) {
		bool [,] output = new bool [NeuralNetController.staticRef.numberOfEnemies, 3];
		for (int k = 0; k < NeuralNetController.staticRef.numberOfEnemies; k++) {
			float [] a = new float [6];
			for (int i = 0; i < 6; i++) {
				float temp = 0f;
				for (int j = 0; j < 6; j++) {
					temp += W_1 [k, i, j] * inp [k, j];
				}
				a [i] = temp + B_1 [k, i];
				if (a [i] < 0) a [i] = 0;//RELU
			}
			float b;
			for (int i = 0; i < 3; i++) {
				float temp = 0.0f;
				for (int j = 0; j < 6; j++) {
					temp += W_2 [k, i, j] * a [j];
				}
				b = temp + B_2 [k, i];
				if (b < 0) { output [k, i] = false; }
				else {
					output [k, i] = true;//SIGMOID}
				}
			}
		}

		//a = W_1 [k,,] * inp[k,] + B_1[k,]
		//b = ReLU(a)
		//c = W_2 [k,,] * b + B_2[k,]
		//out = sigmoid(c)

		return output;

	}

	/// 0: jump?
	/// 1: move left
	/// 2: move right
	void UpdateControllers (bool [,] data) {
		for (int x = 0; x < NeuralNetController.staticRef.numberOfEnemies; x++) {
			int moveDirection = 0;
			if (data [x, 1]) {
				moveDirection--;
			}
			if (data [x, 2]) {
				moveDirection++;
			}
			enemies [x].UpdateActions (new FrameAction (moveDirection, data [x, 0]));
		}
	}

	/// <summary>
	/// call then respawn
	/// </summary>
	void weight_update (int winner_1_idx, int winner_2_idx, int winner_3_idx) {//
		for (int i = 0; i < NeuralNetController.staticRef.numberOfEnemies / 2; i++) {
			if (i != winner_1_idx & i != winner_2_idx & i != winner_3_idx) {
				lr [i] = lr [winner_1_idx] + 2.0f * (UnityEngine.Random.value - .5f) / 5;

				ColorHSV c = enemyColors [winner_1_idx];
				c.h = (c.h + Utility.RandomOneMinusOne () * 0.1f).Normalized01 ();
				enemyColors [i] = c;
				for (int j = 0; j < 6; j++) {
					for (int k = 0; k < 6; k++) {

						W_1 [i, j, k] = W_1 [winner_1_idx, j, k] + lr [i] * 2.0f * (UnityEngine.Random.value - .5f);

					}
					B_1 [i, j] = B_1 [winner_1_idx, j] + lr [i] * 2.0f * (UnityEngine.Random.value - .5f);
				}

				for (int j = 0; j < 3; j++) {
					for (int k = 0; k < 6; k++) {
						W_2 [i, j, k] = W_2 [winner_1_idx, j, k] + lr [i] * 2.0f * (UnityEngine.Random.value - .5f);
					}
					B_2 [i, j] = B_2 [winner_1_idx, j] + lr [i] * 2.0f * (UnityEngine.Random.value - .5f);
				}
			}
		}


		for (int i = NeuralNetController.staticRef.numberOfEnemies / 2; i < 5 * NeuralNetController.staticRef.numberOfEnemies / 6; i++) {
			if (i != winner_1_idx & i != winner_2_idx & i != winner_3_idx) {
				lr [i] = lr [winner_2_idx] + 2.0f * (UnityEngine.Random.value - .5f) / 5;

				ColorHSV c = enemyColors [winner_2_idx];
				c.h = (c.h + Utility.RandomOneMinusOne () * 0.1f).Normalized01 ();
				enemyColors [i] = c;
				for (int j = 0; j < 6; j++) {
					for (int k = 0; k < 6; k++) {

						W_1 [i, j, k] = W_1 [winner_2_idx, j, k] + lr [i] * 2.0f * (UnityEngine.Random.value - .5f);

					}
					B_1 [i, j] = B_1 [winner_2_idx, j] + lr [i] * 2.0f * (UnityEngine.Random.value - .5f);
				}

				for (int j = 0; j < 3; j++) {
					for (int k = 0; k < 6; k++) {
						W_2 [i, j, k] = W_2 [winner_2_idx, j, k] + lr [i] * 2.0f * (UnityEngine.Random.value - .5f);
					}
					B_2 [i, j] = B_2 [winner_2_idx, j] + lr [i] * 2.0f * (UnityEngine.Random.value - .5f);
				}
			}
		}

		for (int i = 5 * NeuralNetController.staticRef.numberOfEnemies / 6; i < NeuralNetController.staticRef.numberOfEnemies; i++) {
			if (i != winner_1_idx & i != winner_2_idx & i != winner_3_idx) {
				lr [i] = lr [winner_3_idx] + 2.0f * (UnityEngine.Random.value - .5f) / 5;

				ColorHSV c = enemyColors [winner_3_idx];
				c.h = (c.h + Utility.RandomOneMinusOne () * 0.1f).Normalized01 ();
				enemyColors [i] = c;
				for (int j = 0; j < 6; j++) {
					for (int k = 0; k < 6; k++) {

						W_1 [i, j, k] = W_1 [winner_3_idx, j, k] + lr [i] * 2.0f * (UnityEngine.Random.value - .5f);

					}
					B_1 [i, j] = B_1 [winner_3_idx, j] + lr [i] * 2.0f * (UnityEngine.Random.value - .5f);
				}

				for (int j = 0; j < 3; j++) {
					for (int k = 0; k < 6; k++) {
						W_2 [i, j, k] = W_2 [winner_3_idx, j, k] + lr [i] * 2.0f * (UnityEngine.Random.value - .5f);
					}
					B_2 [i, j] = B_2 [winner_3_idx, j] + lr [i] * 2.0f * (UnityEngine.Random.value - .5f);
				}
			}
		}
	}

	public void KillAndRespawn () {
		weight_update (winners [0].index, winners [1].index, winners [2].index);
		for (int x = 0; x < NeuralNetController.staticRef.numberOfEnemies; x++) {
			enemies [x].character.transform.position = enemies [winners [0].index].character.transform.position;
			enemies [x].character.color = enemyColors [x];
		}
		ResetWinners ();
	}
}
