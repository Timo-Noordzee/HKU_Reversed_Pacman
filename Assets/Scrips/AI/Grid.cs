using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

	public static Grid instance;

	[Tooltip("Grid size, x=width, y=height")]
	public Vector2 gridSize;
	public int MaxHeapSize {
		get { return (int) (gridSize.x + 1 * gridSize.y + 1); }
	}
	public Node[, ] grid;
	public List<Node> walkableNodes = new List<Node>();

	[Header("Pacdots")]
	public GameObject pacdot;
	public GameObject powerPellet;
	public List<Vector2> powerPelletLocations = new List<Vector2>();

	[Header("Debug")]
	[SerializeField] private bool showGizmos = false;
	[SerializeField] private float nodeGizmoRadius = 0.5f;
	[SerializeField] private Color pathColor;
	[SerializeField] private Color walkableColor;
	[SerializeField] private Color obstacleColor;

	private void Awake() {
		instance = this;
		createGrid();
	}

	private void createGrid() {
		Vector2 bottomLeft = new Vector2(transform.position.x - gridSize.x / 2, transform.position.y - gridSize.y / 2);

		grid = new Node[(int) gridSize.x + 1, (int) gridSize.y + 1];
		for (int x = 0; x <= gridSize.x; x++) {
			for (int y = 0; y <= gridSize.y; y++) {
				Vector2 position = bottomLeft + (Vector2.right * x) + (Vector2.up * y);
				RaycastHit2D hit = Physics2D.Raycast(position, Vector2.up, 0.0f);
				bool walkable = (hit.collider == null);
				grid[x, y] = new Node(walkable, position, x, y);
				if (walkable) {
					walkableNodes.Add(grid[x, y]);
					Instantiate((powerPelletLocations.Contains(position)) ? powerPellet : pacdot, position, Quaternion.identity, transform);
					GameManager.DotCount++;
				}
			}
		}
	}

	public List<Node> getNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if ((x == 0 && y == 0) || (x != 0 && y != 0)) continue;
				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX <= gridSize.x && checkY > 0 && checkY <= gridSize.y) {
					neighbours.Add(grid[checkX, checkY]);
				}
			}
		}
		return neighbours;
	}

	public Node getNodeFromPosition(Vector3 position) {
		int x = Mathf.RoundToInt(position.x) + Mathf.FloorToInt(gridSize.x / 2);
		int y = Mathf.RoundToInt(position.y) + Mathf.FloorToInt(gridSize.y / 2);
		return grid[x, y];
	}

	public Node getRandomWalkableNode(Vector3 position, float minRange = 10) {
		Node node = walkableNodes[Random.Range(0, walkableNodes.Count - 1)];
		while (Vector3.Distance((Vector3) node.position, position) < minRange) {
			node = walkableNodes[Random.Range(0, walkableNodes.Count - 1)];
		}
		return node;
	}

	private void OnDrawGizmos() {
		if (showGizmos) {
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, 1));

			if (grid != null) {
				foreach (Node n in grid) {
					Gizmos.color = (n.walkable ? walkableColor : obstacleColor);
					Gizmos.DrawWireCube(n.position, Vector3.one * nodeGizmoRadius);
				}
			}
		}
	}
}