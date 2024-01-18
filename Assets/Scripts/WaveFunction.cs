using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class WaveFunction : MonoBehaviour
{
    public GameObject Selector;

    public int dimensions;
    public Block[] tileObjects;
    public List<Cell> gridComponents;
    public Cell cellObj;

    int iterations = 0;

    void Awake()
    {
        gridComponents = new List<Cell>();
        InitializeGrid();
    }

    void InitializeGrid()
    {
        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                Cell newCell = Instantiate(cellObj, new Vector2(x, y), Quaternion.identity);
                newCell.CreateCell(false, tileObjects);
                gridComponents.Add(newCell);
            }
        }

        List<Cell> tempGrid = new List<Cell>(gridComponents);
        StartCoroutine(CollapseCell(tempGrid));
        //StartCoroutine(CheckEntropy());
    }


    IEnumerator CheckEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(gridComponents);

        tempGrid.RemoveAll(c => c.collapsed);

        //tempGrid.Sort((a, b) => { return tileOptions.Length - b.tileOptions.Length; });

        int arrLength = tempGrid[0].tileOptions.Length;
        int stopIndex = default;

        for (int i = 1; i < tempGrid.Count; i++)
        {
            if (tempGrid[i].tileOptions.Length > arrLength)
            {
                stopIndex = i;
                break;
            }
        }

        if (stopIndex > 0)
        {
            tempGrid.RemoveRange(stopIndex, tempGrid.Count - stopIndex);
        }


        yield return new WaitForSeconds(0.01f);

        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        //CollapseCell(tempGrid, randIndex);
    }


    IEnumerator CollapseCell(List<Cell> tempGrid)
    {

        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                Cell cellToCollapse = tempGrid[x + y * dimensions];
                cellToCollapse.collapsed = true;
                Block selectedTile = cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
                cellToCollapse.tileOptions = new Block[] { selectedTile };

                Block foundTile = cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
                Instantiate(foundTile, cellToCollapse.transform.position, Quaternion.identity);
                UpdateGeneration(x, y);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    void UpdateGeneration(int x, int y)
    {
        List<Cell> newGenerationCell = new List<Cell>(gridComponents);


        var index = x + y * dimensions;
        index++;
        
        //if (gridComponents[index].collapsed)
        //{
        //    Debug.Log("called");
        //    newGenerationCell[index] = gridComponents[index];
        //}
        //else
        //{

        List<Block> options = new List<Block>();
        foreach (Block t in tileObjects)
        {
            options.Add(t);
        }

        //update above
        if (y > 0)
        {
            Cell up = gridComponents[x + (y - 1) * dimensions];
            List<Block> validOptions = new List<Block>();

            foreach (Block possibleOptions in up.tileOptions)
            {
                var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                var valid = tileObjects[valOption].upNeighbours;

                validOptions = validOptions.Concat(valid).ToList();
            }

            CheckValidity(options, validOptions);
        }

        //update right
        if (x < dimensions - 1)
        {
            Cell right = gridComponents[x + 1 + y * dimensions];
            List<Block> validOptions = new List<Block>();

            foreach (Block possibleOptions in right.tileOptions)
            {
                var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                var valid = tileObjects[valOption].leftNeighbours;

                validOptions = validOptions.Concat(valid).ToList();
            }

            CheckValidity(options, validOptions);
        }

        //look down
        if (y < dimensions - 1)
        {
            Cell down = gridComponents[x + (y + 1) * dimensions];
            List<Block> validOptions = new List<Block>();

            foreach (Block possibleOptions in down.tileOptions)
            {
                var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                var valid = tileObjects[valOption].downNeighbours;

                validOptions = validOptions.Concat(valid).ToList();
            }

            CheckValidity(options, validOptions);
        }

        //look left
        if (x > 0)
        {
            Cell left = gridComponents[x - 1 + y * dimensions];
            List<Block> validOptions = new List<Block>();

            foreach (Block possibleOptions in left.tileOptions)
            {
                var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                var valid = tileObjects[valOption].rightNeighbours;

                validOptions = validOptions.Concat(valid).ToList();
            }

            CheckValidity(options, validOptions);
        }

        Block[] newTileList = new Block[options.Count];

        for (int i = 0; i < options.Count; i++)
        {
            newTileList[i] = options[i];
        }

        newGenerationCell[index].RecreateCell(newTileList);
        //}

        gridComponents = newGenerationCell;
        iterations++;

        if (iterations < dimensions * dimensions)
        {
            StartCoroutine(CheckEntropy());
        }
    }

    void CheckValidity(List<Block> optionList, List<Block> validOption)
    {
        for (int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if (!validOption.Contains(element))
            {
                optionList.RemoveAt(x);
            }
        }
    }
}
