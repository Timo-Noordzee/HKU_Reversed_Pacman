using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState {
	GameOver,
	Win,
	Game
}

public class GameManager : MonoBehaviour {

	[Header("Settings")]
	public Vector3 pacmanSpawnPoint;
	public Vector3 ghostSpawnPoint;
	public float ghostSpawnDelay = 10.0f;
	public static GameState gameState = GameState.Game;

	[Header("Ghosts")]
	private int controlledGhostIndex = 0;
	public int ControlledGhostIndex {
		get { return controlledGhostIndex; }
		set {
			controlledGhostIndex = value;
			ghostImage.sprite = ghosts[controlledGhostIndex].sprite;
		}
	}
	public List<Ghost> ghosts;

	[Header("UI Elements")]
	public Text scoreText;
	public Text livesText;
	public Image ghostImage;

	private static GameManager instance;
	public static GameManager Instance {
		get { return instance; }
	}

	public delegate void selectGhostDelegate(int index);
	public static event selectGhostDelegate selectGhostEvent;

	private static int dotCount;
	public static int DotCount {
		get { return dotCount; }
		set {
			if (value < dotCount) Score += 10;
			dotCount = value;
			if (value <= 0) EventManager.triggerEvent(EventType.OnDotsEaten);
		}
	}

	private static int score;
	public static int Score {
		get { return score; }
		set {
			score = value;
			Instance.scoreText.text = "Score: " + value;
		}
	}

	private static int lives = 3;
	public static int Lives {
		get { return lives; }
		set {
			lives = value;
			Instance.livesText.text = "Lives: " + value;
			if (lives <= 0) {
				Application.Quit();
			}
		}
	}

	private Coroutine spawnGhostsCoroutine;

	private void Awake() {
		instance = this;
	}

	private void Start() {
		spawnGhostsCoroutine = StartCoroutine(spawnGhosts());
	}

	private void OnEnable() {
		EventManager.registerEventListener(EventType.OnDotsEaten, onAllDotsEaten);
		EventManager.registerEventListener(EventType.OnPacmanDeath, OnPacmanDeath);
	}

	private void onDisable() {
		EventManager.unregisterEventListener(EventType.OnDotsEaten, onAllDotsEaten);
		EventManager.unregisterEventListener(EventType.OnPacmanDeath, OnPacmanDeath);
	}

	private void OnPacmanDeath() {
		Lives--;
		controlledGhostIndex = 0;
		StopCoroutine(spawnGhostsCoroutine);
		spawnGhostsCoroutine = StartCoroutine(spawnGhosts());
	}

	private void onAllDotsEaten() {
		foreach (Ghost ghost in ghosts) {
			ghost.enabled = false;
		}
	}

	private IEnumerator spawnGhosts() {
		for (int i = 0; i < ghosts.Count; i++) {
			yield return new WaitForSeconds(ghostSpawnDelay * (i));
			ghosts[i].spawnGhost(i == 0);
		}
	}

	private void Update() {
		if (Input.GetButtonDown("Blinky")) {
			ControlledGhostIndex = 0;
			selectGhostEvent(ControlledGhostIndex);
		} else if (Input.GetButtonDown("Pinky")) {
			ControlledGhostIndex = 1;
			selectGhostEvent(ControlledGhostIndex);
		} else if (Input.GetButtonDown("Inky")) {
			ControlledGhostIndex = 2;
			selectGhostEvent(ControlledGhostIndex);
		} else if (Input.GetButtonDown("Clyde")) {
			ControlledGhostIndex = 3;
			selectGhostEvent(ControlledGhostIndex);
		}
	}
}