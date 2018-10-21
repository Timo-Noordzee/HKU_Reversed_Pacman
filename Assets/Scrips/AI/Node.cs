using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {
	public int gCost;
	public int hCost;
	public int fCost {
		get { return gCost + hCost; }
	}

	public int gridX;
	public int gridY;

	public bool walkable;
	public Vector2 position;
	public Node parent;

	public int heapIndex;
	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}

	public Node(bool _walkable, Vector2 _position, int _gridX, int _gridY) {
		walkable = _walkable;
		position = _position;
		gridX = _gridX;
		gridY = _gridY;
	}
}