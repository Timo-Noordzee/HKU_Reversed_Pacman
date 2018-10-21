using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T> {

	T[] items;
	int itemCount;

	public int count {
		get { return itemCount; }
	}

	public Heap(int maxHeapSize) {
		items = new T[maxHeapSize];
	}

	public void add(T item) {
		item.HeapIndex = itemCount;
		items[itemCount] = item;
		sortUp(item);
		itemCount++;
	}

	public T removeFirst() {
		T item = items[0];
		itemCount--;
		items[0] = items[itemCount];
		items[0].HeapIndex = 0;
		sortDown(items[0]);
		return item;
	}

	public bool contains(T item) {
		return Equals(items[item.HeapIndex], item);
	}

	public void updateItem(T item) {
		sortUp(item);
	}

	private void sortDown(T item) {
		while (true) {
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;

			if (childIndexLeft < itemCount) {
				swapIndex = childIndexLeft;

				if (childIndexRight < itemCount) {
					if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) {
						swapIndex = childIndexRight;
					}
				}

				if (item.CompareTo(items[swapIndex]) < 0) {
					swap(item, items[swapIndex]);
				} else {
					return;
				}
			} else {
				return;
			}
		}
	}

	private void sortUp(T item) {
		int parentIndex = (item.HeapIndex - 1) / 2;
		while (true) {
			T parentItem = items[parentIndex];
			if (item.CompareTo(parentItem) > 0) {
				swap(item, parentItem);
			} else {
				break;
			}

			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	private void swap(T itemA, T itemB) {
		items[itemA.HeapIndex] = itemB;
		items[itemB.HeapIndex] = itemA;

		int index = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = index;
	}
}

public interface IHeapItem<T> : IComparable<T> {
	int HeapIndex {
		get;
		set;
	}
}