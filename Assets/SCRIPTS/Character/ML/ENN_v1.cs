using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ENN_v1 {
	private float [,,] W_1;
	private float [,] B_1;

	private float [,,] W_2;
	private float [,] B_2;
	private float [] lr;

	private float minDistance = Mathf.Infinity;
	private int currentWinnerIndex = 0;

	private ControlCharacterML [] enemies;
	private Character player;

	public ENN_v1 (Character player, ControlCharacterML [] enemies) {
		this.player = player;
		this.enemies = enemies;

		W_1 = new float [NeuralNetController.staticRef.numberOfEnemies, 6, 6]; //12 inputs, your momentum vector, their momentum vector, the x y z distances
		B_1 = new float [NeuralNetController.staticRef.numberOfEnemies, 6];
		W_2 = new float [NeuralNetController.staticRef.numberOfEnemies, 3, 6];
		B_2 = new float [NeuralNetController.staticRef.numberOfEnemies, 3];
		lr = new float [NeuralNetController.staticRef.numberOfEnemies];
		for (int i = 0; i < NeuralNetController.staticRef.numberOfEnemies; i++) lr [i] = 0.01f;

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

	// Calculates info from player
	public void Update () {
		float [,] data = new float [NeuralNetController.staticRef.numberOfEnemies, 6];
		for (int x = 0; x < NeuralNetController.staticRef.numberOfEnemies; x++) {
			data [x, 0] = enemies [x].character.transform.position.x - player.transform.position.x;
			data [x, 1] = enemies [x].character.transform.position.y - player.transform.position.y;
			float currentDistance = Vector2.Distance (enemies [x].character.transform.position, player.transform.position);
			if (currentDistance < minDistance) {
				minDistance = currentDistance;
				currentWinnerIndex = x;
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
	void weight_update (int winner_i) {
		for (int i = 0; i < NeuralNetController.staticRef.numberOfEnemies; i++) {
			if (i != winner_i) {
				lr [i] = lr [winner_i] + 2.0f * (UnityEngine.Random.value - .5f) / 5;
				for (int j = 0; j < 6; j++) {
					for (int k = 0; k < 6; k++) {

						W_1 [i, j, k] = W_1 [winner_i, j, k] + lr [i] * 2.0f * (UnityEngine.Random.value - .5f);

					}
					B_1 [i, j] = B_1 [winner_i, j] + lr [i] * 2.0f * (UnityEngine.Random.value - .5f);
				}

				for (int j = 0; j < 3; j++) {
					for (int k = 0; k < 6; k++) {
						W_2 [i, j, k] = W_2 [winner_i, j, k] + lr [i] * 2.0f * (UnityEngine.Random.value - .5f);
					}
					B_2 [i, j] = B_2 [winner_i, j] + lr [i] * 2.0f * (UnityEngine.Random.value - .5f);
				}
			}
		}
	}

	public void KillAndRespawn () {
		weight_update (currentWinnerIndex);
		for (int x = 0; x < NeuralNetController.staticRef.numberOfEnemies; x++) {
			enemies [x].character.transform.position = enemies [currentWinnerIndex].character.transform.position;
		}
		minDistance = Mathf.Infinity;
	}
}
