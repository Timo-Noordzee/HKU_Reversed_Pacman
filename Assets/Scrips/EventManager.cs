using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType {
	OnPacmanDeath,
	OnDotsEaten,
	OnPowerPelletEaten
}

public static class EventManager {

	private static Dictionary<EventType, Action> events = new Dictionary<EventType, Action>();

	private static Dictionary<EventType, List<object>> eventsWithArg = new Dictionary<EventType, List<object>>();

	public static void registerEventListener(EventType eventType, Action action) {
		if (!events.ContainsKey(eventType)) {
			events.Add(eventType, action);
		} else {
			events[eventType] += action;
		}
	}

	public static void unregisterEventListener(EventType eventType, Action action) {
		if (events.ContainsKey(eventType)) {
			events[eventType] -= action;
		}
	}

	public static void triggerEvent(EventType eventType) {
		if (events.ContainsKey(eventType)) {
			events[eventType]();
		}
	}

}