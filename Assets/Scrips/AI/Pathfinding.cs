using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class Pathfinding : MonoBehaviour {

	private Grid grid;
	private PathRequestManager requestManager;

	void Awake() {
		grid = GetComponent<Grid>();
		requestManager = GetComponent<PathRequestManager>();
	}

	public void startFindPath(Vector3 start, Vector3 target) {
		StartCoroutine(findPath(start, target));
	}

	private IEnumerator findPath(Vector3 start, Vector3 target) {
		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		Node startNode = grid.getNodeFromPosition(start);
		Node targetNode = grid.getNodeFromPosition(target);

		if (targetNode.walkable && targetNode.walkable) {
			Heap<Node> open = new Heap<Node>(grid.maxHeapSize);
			HashSet<Node> closed = new HashSet<Node>();
			open.add(startNode);

			while (open.count > 0) {
				Node currentNode = open.removeFirst();
				closed.Add(currentNode);

				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}

				foreach (Node neighbour in grid.getNeighbours(currentNode)) {
					if (!neighbour.walkable || closed.Contains(neighbour)) continue;

					int costToNeighbour = currentNode.gCost + getDistance(currentNode, neighbour);
					if (costToNeighbour < neighbour.gCost || !open.contains(neighbour)) {
						neighbour.gCost = costToNeighbour;
						neighbour.hCost = getDistance(neighbour, targetNode);
						neighbour.parent = currentNode;

						if (!open.contains(neighbour)) {
							open.add(neighbour);
						} else {
							open.updateItem(neighbour);
						}
					}
				}
			}
		}
		yield return null;
		if (pathSuccess) {
			waypoints = tracePath(startNode, targetNode);
		}
		requestManager.finishedProcessingPath(waypoints, pathSuccess);
	}

	private Vector3[] tracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		Vector3[] waypoints = simplifyPath(path);
		Array.Reverse(waypoints);

		return waypoints;
	}

	private Vector3[] simplifyPath(List<Node> path) {
		List<Vector3> waypoints = new List<Vector3>();
		Vector3 beginPosition = path[path.Count - 1].position;
		Vector2 directionOld = Vector2.zero;
		for (int i = 1; i < path.Count; i++) {
			Vector2 directionNew = new Vector2(path[i].gridX - path[i - 1].gridX, path[i].gridY - path[i - 1].gridY);
			if (directionNew != directionOld) {
				waypoints.Add(path[i - 1].position);
			}
			directionOld = directionNew;
		}
		waypoints.Add(beginPosition);

		return waypoints.ToArray();
	}

	private int getDistance(Node nodeA, Node nodeB) {
		int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (distX > distY) {
			return 14 * distY + 10 * (distX - distY);
		} else {
			return 14 * distX + 10 * (distY - distX);
		}
	}

	public bool isValid(Node current, Vector2 direction) {
		int checkX = current.gridX + (int) direction.x;
		int checkY = current.gridY + (int) direction.y;

		if (checkX >= 0 && checkX <= grid.gridSize.x && checkY >= 0 && checkY <= grid.gridSize.y) {
			return grid.grid[checkX, checkY].walkable;
		}
		return false;
	}

	public Node nextDestination(Node current, Vector2 direction) {
		int checkX = current.gridX + (int) direction.x;
		int checkY = current.gridY + (int) direction.y;

		if (checkX >= 0 && checkX <= grid.gridSize.x && checkY >= 0 && checkY <= grid.gridSize.y) {
			return grid.grid[checkX, checkY];
		}

		return null;
	}

}