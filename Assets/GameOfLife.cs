using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    public int resolutionX = 50;
    public int resolutionY = 50;
    public float density = .2f;

    bool[,] cells;
    // Start is called before the first frame update
    void Start()
    {
        cells = new bool[resolutionX, resolutionY];

        for (int x = 0; x < resolutionX; x++)
        {
            for (int y = 0; y < resolutionY; y++)
            {
                float rand = Random.Range(0.0f, 1.0f);
                cells[x, y] = rand <= density;
            }
        }

        PopulateGrid(cells);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearGrid();
            cells = GetNextGen();
            PopulateGrid(cells);
        }
    }

    void PopulateGrid(bool[,] cells)
    {
        for (int x = 0; x < resolutionX; x++)
            for (int y = 0; y < resolutionY; y++)
                if (cells[x, y]) SpawnCellAtPosition(x, y);
    }

    void SpawnCellAtPosition(int xCoord, int yCoord)
    {
        GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cell.transform.position = new Vector3(xCoord, yCoord, 0);
        cell.transform.SetParent(this.transform, true);
        cell.tag = "Cell";
    }

    bool[,] GetNextGen()
    {
        bool[,] nextGen = new bool[resolutionX, resolutionY];

        // Nested loop to check each cell for number of alive neighbors
        // Outer loop iterates x coordinate
        for (int x = 0; x < resolutionX; x++)
        {
            // Inner iterates y coordinate
            for (int y = 0; y < resolutionY; y++)
            {
                int aliveNeighbors = 0;

                // Mini loop to check neighbors
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    for (int yOffset = -1; yOffset <= 1; yOffset++)
                    {
                        // We don't want to count the current cell
                        if (xOffset != 0 || yOffset != 0)
                        {
                            if (GetMooreNeighbor(x + xOffset, y + yOffset)) aliveNeighbors += 1;
                        }
                    }
                }
                // This cell is alive
                if (cells[x, y])
                {
                    if (aliveNeighbors == 2 || aliveNeighbors == 3) nextGen[x, y] = true;
                    else nextGen[x, y] = false;
                }
                // This cell is dead
                else if (!cells[x, y])
                {
                    if (aliveNeighbors == 3) nextGen[x, y] = true;
                    else nextGen[x, y] = false;
                }
            }
        }
        return nextGen;
    }

    // Make our neighbors wrap at the edges
    bool GetMooreNeighbor(int xCoord, int yCoord)
    {
        int wrappedX = xCoord % resolutionX;
        if (wrappedX < 0) wrappedX += resolutionX;

        int wrappedY = yCoord % resolutionY;
        if (wrappedY < 0) wrappedY += resolutionY;

        return cells[wrappedX, wrappedY];
    }

    void ClearGrid()
    {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cell");
        foreach (GameObject cube in cubes)
            Destroy(cube);
    }
}
