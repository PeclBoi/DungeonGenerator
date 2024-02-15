using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RoomContentPlacer : MonoBehaviour
{

    public List<DecorationAsset> decorationAssets;

    public RoomRasterizer[] rasterizers;

    public List<Cell> cells = new List<Cell>();

    public bool DrawGizmos;

    void OnDrawGizmos()
    {

        if(!DrawGizmos) { return; }

        float maxDistance = 10f;
        RaycastHit hit;

        foreach (var cell in cells)
        {
            bool isHit = Physics.BoxCast(cell.position, new Vector3(rasterizers[0].SpaceX, 0, rasterizers[0].SpaceY) / 2, Vector3.up, out hit,
            transform.rotation, maxDistance);
            if (isHit)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(cell.position, transform.up * hit.distance);
                Gizmos.DrawWireCube(cell.position + transform.up * hit.distance, new Vector3(rasterizers[0].SpaceX, 0, rasterizers[0].SpaceY));
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(cell.position, transform.up * maxDistance);
            }
        }
    }

    private void Start()
    {
        foreach (var rasterizer in rasterizers)
        {
            cells.AddRange(rasterizer.Rasterize());
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(FillCellsAsync(cells.ToArray()));
        }
    }


    IEnumerator FillCellsAsync(Cell[] cells)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            var cell = cells[i];

            if (cell.zone == CellTag.Taken || !cell.IsFree()) { continue; }



            var zoneC = ZoneChances(cell.zone);
            var rand = Random.Range(0, 1f);

            if (rand <= zoneC)
            {
                var possibleElements = decorationAssets.Where(x => x.zone == cell.zone).ToList();
                if (possibleElements.Count > 0)
                {
                    var decoration = PickOneAsset(possibleElements);

                    if(decoration == null) { continue; }

                    var pos = cell.position;
                    var rot = GetRotation(cell.side);

                    var decorationAsset = Instantiate(decoration.prefab, pos + new Vector3(0, 0.1f, 0), rot);

                    //yield return new WaitForSeconds(0.1f);

                    if (IsInsideRoom(decoration) && !IsOverlap(decorationAsset, cells))
                    {
                        cells = RemoveArea(cells);
                    }
                    else
                    {
                        Destroy(decorationAsset);
                    }

                    yield return new WaitForSeconds(0.1f);
                }

            }
        }
    }




    private void CheckCells(Cell[] cells)
    {
        foreach (var cell in cells)
        {
            if (!Physics.BoxCast(cell.position, new Vector3(cell.Size.x, 0, cell.Size.y) / 2, Vector3.up, out RaycastHit hit, transform.rotation, 5f))
            {
                cell.zone = CellTag.Inside;
            }
        }
    }

    private Cell[] RemoveArea(Cell[] cells)
    {
        foreach (var cell in cells.Where(cell => cell.zone != CellTag.Taken))
        {
            if (Physics.BoxCast(cell.position, new Vector3(cell.Size.x, 0, cell.Size.y) / 2, Vector3.up, out RaycastHit hit, transform.rotation, 5f))
            {
                Debug.Log(hit.transform.name);
                cell.zone = CellTag.Taken;
            }
        }

        return cells;
    }

    private bool IsOverlap(GameObject decorationObject, Cell[] cells)
    {
        foreach (var cell in cells.Where(cell => cell.zone == CellTag.Taken))
        {
            var hits = Physics.BoxCastAll(cell.position, new Vector3(cell.Size.x, 0, cell.Size.y) / 2, Vector3.up, transform.rotation, 5f);
            if (hits.Any())
            {
                if (hits.Select(hit => hit.transform.gameObject).Contains(decorationObject))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsInsideRoom(DecorationAsset decoration)
    {
        // TODO
        return true;

    }

    private Quaternion GetRotation(CellSideTag side)
    {
        //TODO
        return Quaternion.identity;
    }


    private DecorationAsset PickOneAsset(List<DecorationAsset> possibleElements)
    {
        float random = Random.Range(0, 1f);
        var viableElements = possibleElements.Where(element => element.chances >= random).OrderBy(e => e.chances).ToList();
        
        if(!viableElements.Any()) { return null; }

        var lowestChance = viableElements.Min(e => e.chances);

        var elementsWithLowestChance = viableElements.Where(e => e.chances == lowestChance).ToList();

        var randomIndex = Random.Range(0, elementsWithLowestChance.Count);

        return elementsWithLowestChance[randomIndex];

    }

    private float ZoneChances(CellTag zone)
    {
        float chance = 0;
        switch (zone)
        {
            case CellTag.Inside:
                chance = 0.4f;
                break;
            case CellTag.Wall:
                chance = 0.2f;
                break;
        }

        return chance;
    }
}