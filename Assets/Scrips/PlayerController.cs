using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[SerializeField] private float _speed = 30.0f;
	public float speed {
		get { return _speed; }
		set { _speed = value; }
	}

	public Pathfinding pathfinding;
	public Grid grid;

	private Vector2 direction = Vector2.zero;
	private Vector2 nextDirection = Vector2.zero;
	private Node targetNode;
	private Node currentNode;
	private bool alive;

	private Rigidbody2D rigid;
	private Animator animator;

	// Use this for initialization
	private void Start() {
		alive = true;
		rigid = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		targetNode = grid.getNodeFromPosition(transform.position);
	}

	private void OnEnable() {
		EventManager.registerEventListener(EventType.OnPacmanDeath, onPacmanDeath);
	}

	private void OnDisable() {
		EventManager.unregisterEventListener(EventType.OnPacmanDeath, onPacmanDeath);
	}

	private void animate() {
		Vector2 dir = targetNode.position - (Vector2) transform.position;
		animator.SetFloat("dirX", dir.x);
		animator.SetFloat("dirY", dir.y);
	}

	private void FixedUpdate() {
		if (alive) {
			Vector2 p = Vector2.MoveTowards(transform.position, targetNode.position, speed * Time.deltaTime);
			rigid.MovePosition(p);

			if (Input.GetAxis("Horizontal") > 0) nextDirection = Vector2.right;
			if (Input.GetAxis("Horizontal") < 0) nextDirection = -Vector2.right;
			if (Input.GetAxis("Vertical") > 0) nextDirection = Vector2.up;
			if (Input.GetAxis("Vertical") < 0) nextDirection = -Vector2.up;

			currentNode = grid.getNodeFromPosition(transform.position);

			if ((Vector2) transform.position == targetNode.position) {
				if (pathfinding.isValid(currentNode, nextDirection)) {
					targetNode = pathfinding.nextDestination(currentNode, nextDirection);
					direction = nextDirection;
				} else {
					if (pathfinding.isValid(currentNode, direction)) targetNode = pathfinding.nextDestination(currentNode, direction);
				}
			}
			animate();
		}
	}

	private void onPacmanDeath() {
		targetNode = grid.getNodeFromPosition(GameManager.Instance.pacmanSpawnPoint);
		direction = Vector2.zero;
		nextDirection = Vector2.zero;
		animator.SetBool("alive", true);
		alive = true;
		transform.position = GameManager.Instance.pacmanSpawnPoint;
	}

	private IEnumerator deathAnimation() {
		animator.SetBool("alive", false);
		animator.SetTrigger("Death");
		yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length * 1.5f);
		EventManager.triggerEvent(EventType.OnPacmanDeath);
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag("Pacdot")) {
			Destroy(other.gameObject);
			GameManager.DotCount--;
			GameManager.Score += 10;
		} else if (other.gameObject.CompareTag("PowerPellet")) {
			Destroy(other.gameObject);
			GameManager.DotCount--;
			EventManager.triggerEvent(EventType.OnPowerPelletEaten);
		}
	}

	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag("Ghost")) {
			if (other.gameObject.GetComponent<Ghost>().frightened) {

			} else {
				alive = false;
				StartCoroutine(deathAnimation());
			}
		}
	}
}