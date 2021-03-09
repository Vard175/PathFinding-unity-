using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridGround : MonoBehaviour
{

	public LayerMask unwalkable;
	public Vector2 gridSize;
	public float nodeRadius;
	Node[,] grid;

	float nodeDiameter;
	int rows, cols;

	void Awake()
	{
		nodeDiameter = nodeRadius * 2;
		rows = Mathf.RoundToInt(gridSize.x / nodeDiameter);
		cols = Mathf.RoundToInt(gridSize.y / nodeDiameter);
		CreateGrid();
	}

	void CreateGrid()
	{
		grid = new Node[rows, cols];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.y / 2;

		for (int x = 0; x < rows; x++)
		{
			for (int y = 0; y < cols; y++)
			{
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkable));
				grid[x, y] = new Node(walkable, worldPoint, x, y);
			}
		}
	}

	public List<Node> GetNeighbours(Node node)
	{
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < rows && checkY >= 0 && checkY < cols)
				{
					neighbours.Add(grid[checkX, checkY]);
				}
			}
		}

		return neighbours;
	}


	public Node NodeFromWorldToPoint(Vector3 worldPosition)
	{
		float percentX = (worldPosition.x + gridSize.x / 2) / gridSize.x;
		float percentY = (worldPosition.z + gridSize.y / 2) / gridSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((rows - 1) * percentX);
		int y = Mathf.RoundToInt((cols - 1) * percentY);
		return grid[x, y];
	}

	public List<Node> path;
	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));

		if (grid != null)
		{
			foreach (Node n in grid)
			{
				Gizmos.color = (n.walkable) 
					? Color.white : 
					Color.red;
				if (path != null)
					if (path.Contains(n))
						Gizmos.color = Color.black;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
			}
		}
	}
}