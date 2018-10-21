using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour {

	public Grid grid;
	public Pathfinding pathfinding;

	public float speed = 30f;

	private Vector2 nextDirection;
	private Vector2 direction;
	private Node currentNode;
	private Node destinationNode;

	private Rigidbody2D rigid;
	private Animator animator;

	// Use this for initialization
	void Start() {
		rigid = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		destinationNode = grid.getNodeFromPosition(transform.position);
	}

	void OnEnable() {
		destinationNode = grid.getNodeFromPosition(transform.position);
		nextDirection = direction = Vector2.zero;
	}

	// Update is called once per frame
	void Update() {
		Vector2 p = Vector2.MoveTowards(transform.position, destinationNode.position, speed * Time.deltaTime);
		rigid.MovePosition(p);

		if (Input.GetAxis("HorizontalGhost") > 0) nextDirection = Vector2.right;
		if (Input.GetAxis("HorizontalGhost") < 0) nextDirection = -Vector2.right;
		if (Input.GetAxis("VerticalGhost") > 0) nextDirection = Vector2.up;
		if (Input.GetAxis("VerticalGhost") < 0) nextDirection = -Vector2.up;

		currentNode = grid.getNodeFromPosition(transform.position);

		if ((Vector2) transform.position == destinationNode.position) {
			if (pathfinding.isValid(currentNode, nextDirection)) {
				destinationNode = pathfinding.nextDestination(currentNode, nextDirection);
				direction = nextDirection;
			} else {
				if (pathfinding.isValid(currentNode, direction)) destinationNode = pathfinding.nextDestination(currentNode, direction);
			}
		}

		animate();

	}

	private void animate() {
		Vector2 dir = destinationNode.position - (Vector2) transform.position;
		animator.SetFloat("dirX", dir.x);
		animator.SetFloat("dirY", dir.y);
	}
}