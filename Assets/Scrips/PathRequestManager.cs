using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinding))]
public class PathRequestManager : MonoBehaviour {

	public Pathfinding pathfinding;

	private static PathRequestManager instance;
	private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	private PathRequest currentPathRequest;
	private bool isProcessingPath;

	void Awake() {
		instance = this;
		pathfinding = GetComponent<Pathfinding>();
	}

	public static void requestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
		PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
		instance.pathRequestQueue.Enqueue(newRequest);
		instance.tryProcessNext();
	}

	private void tryProcessNext() {
		if (!isProcessingPath && pathRequestQueue.Count > 0) {
			currentPathRequest = pathRequestQueue.Dequeue();
			isProcessingPath = true;
			pathfinding.startFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}

	public void finishedProcessingPath(Vector3[] path, bool success) {
		currentPathRequest.callback(path, success);
		isProcessingPath = false;
		tryProcessNext();
	}

	struct PathRequest {
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool> callback;

		public PathRequest(Vector3 _pathStart, Vector3 _pathEnd, Action<Vector3[], bool> _callback) {
			pathStart = _pathStart;
			pathEnd = _pathEnd;
			callback = _callback;
		}

	}
}