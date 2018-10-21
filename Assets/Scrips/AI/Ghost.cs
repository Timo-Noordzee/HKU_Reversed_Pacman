﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ghost : MonoBehaviour {

	[Header("Settings")]
	public int ghostId;
	public float movementSpeed;
	public float turningDelay = 1.0f;
	public Color color;
	public bool frightened;

	[Header("Movement")]
	public bool playerControlled;
	public bool PlayerControlled {
		get { return playerControlled; }
		set {
			GetComponent<GhostController>().enabled = value;
			playerControlled = value;
			if (playerControlled == false) requestPath();
		}
	}
	public bool hasPath = false;
	public bool requestingPath = false;
	public bool waitingForNextMove = false;
	[Space(5)]
	public int waypointIndex;
	private Vector2 direction;
	[Space(5)]
	public Vector3 waitingPosition;
	public Vector3 target;
	public Vector3[] path;

	private Coroutine frightenedCoroutine;

	[HideInInspector] public Rigidbody2D rigid;
	[HideInInspector] public Animator animator;
	[HideInInspector] public GhostController ghostController;

	public void spawnGhost(bool _playerControlled = false) {
		transform.position = GameManager.Instance.ghostSpawnPoint;
		this.enabled = true;
		GetComponent<CircleCollider2D>().enabled = true;
		PlayerControlled = _playerControlled;
		requestPath();
	}

	public void resetGhost() {
		transform.position = waitingPosition;
		this.enabled = false;
		GetComponent<CircleCollider2D>().enabled = false;
		ghostController.enabled = false;
	}

	public void requestPath() {
		target = (Vector3) Grid.instance.getRandomWalkableNode(transform.position).position;

		requestingPath = true;
		hasPath = false;
		PathRequestManager.requestPath(transform.position, target, onReceivePath);
	}

	private void Awake() {
		rigid = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		ghostController = GetComponent<GhostController>();

		GetComponent<CircleCollider2D>().enabled = false;
		ghostController.enabled = false;
	}

	private void OnEnable() {
		EventManager.registerEventListener(EventType.OnPacmanDeath, onPacmanDeath);
		EventManager.registerEventListener(EventType.OnDotsEaten, onAllDotsEaten);
		EventManager.registerEventListener(EventType.OnPowerPelletEaten, onPowerPelletEaten);
		GameManager.selectGhostEvent += onSelectGhost;
	}

	private void OnDisable() {
		EventManager.unregisterEventListener(EventType.OnPacmanDeath, onPacmanDeath);
		EventManager.unregisterEventListener(EventType.OnDotsEaten, onAllDotsEaten);
		EventManager.unregisterEventListener(EventType.OnPowerPelletEaten, onPowerPelletEaten);
		GameManager.selectGhostEvent -= onSelectGhost;
	}

	private void OnDrawGizmos() {
		Gizmos.color = color;
		if (path != null) {
			foreach (Vector3 position in path) {
				Gizmos.DrawCube(position, Vector3.one * 0.5f);
			}
		}
	}

	private void onPacmanDeath() {
		resetGhost();
		path = null;
	}

	private void onAllDotsEaten() {

	}

	private void onPowerPelletEaten() {
		if (frightenedCoroutine != null) StopCoroutine(frightenedCoroutine);
		frightenedCoroutine = StartCoroutine(getFrightened());
	}

	private void onSelectGhost(int id) {
		if (playerControlled) {
			if (id != ghostId) {
				PlayerControlled = false;
			}
		} else {
			if (id == ghostId) {
				PlayerControlled = true;
			}
		}
	}

	private void onReceivePath(Vector3[] _path, bool _success) {
		if (_success) {
			path = _path;
			hasPath = true;
			requestingPath = false;
			waypointIndex = 0;
		} else {
			requestingPath = false;
		}
	}

	private void move() {
		if (!PlayerControlled) {
			if (!hasPath && !requestingPath) {
				requestPath();
				return;
			}

			if (!waitingForNextMove) {
				if (path != null && waypointIndex < path.Length) {
					Vector2 p = Vector2.MoveTowards(transform.position, path[waypointIndex], (frightened ? movementSpeed * 0.5f : movementSpeed) * Time.deltaTime);
					direction = ((Vector2) path[waypointIndex] - (Vector2) transform.position).normalized;
					rigid.MovePosition(p);
					if (transform.position == path[waypointIndex]) {
						waypointIndex++;
						StartCoroutine(waitForNextTurn());
					}
				} else {
					requestPath();
				}
			}
		}
	}

	private IEnumerator getFrightened() {
		frightened = true;
		yield return new WaitForSeconds(10.0f);
		for (int i = 0; i < 8; i++) {
			yield return new WaitForSeconds(0.5f);
			animator.SetInteger("frightenedState", i % 2);
		}
		frightened = false;
	}

	private IEnumerator waitForNextTurn() {
		waitingForNextMove = true;
		yield return new WaitForSeconds(turningDelay);
		waitingForNextMove = false;
	}

	private void updateAnimation() {
		animator.SetBool("frightened", frightened);
		animator.SetFloat("dirX", direction.x);
		animator.SetFloat("dirY", direction.y);
	}

	private void Update() {
		if (!PlayerControlled) move();
		updateAnimation();
	}
}