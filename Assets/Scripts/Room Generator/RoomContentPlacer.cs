using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RoomContentPlacer : MonoBehaviour
{

    private Room room;

    private bool IsCellsLoaded;

    [Header("Room Decoration Assets")]
    public RoomContent roomContent;

    //public List<DecorationAsset> decorationAssets;
    public List<Cell> cells = new List<Cell>();


    [Header("Rasterization")]
    public RoomRasterizer[] rasterizers;
    // TODO Make Grid Resolution Uniform for all FloorTiles
    //public int gridResolution;

    [Header("Debug Info")]
    public bool DrawGizmos;

    //public LayerMask layerMask;

    void OnDrawGizmos()
    {

        if (!DrawGizmos) { return; }

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
        room = GetComponentInParent<Room>();
    }

    public void LoadCellsOnce()
    {
        if(IsCellsLoaded) { return; }

        foreach (var rasterizer in rasterizers)
        {
            cells.AddRange(rasterizer.Rasterize());
        }
        IsCellsLoaded = true;
    }

    public void PlaceAssets()
    {
        LoadCellsOnce();
        StartCoroutine(FillCellsAsync(cells.ToArray()));
    }


    public void PlaceNPCs()
    {
        LoadCellsOnce();
        StartCoroutine(PlaceNPCsAsync(cells.ToArray()));
    }

    private IEnumerator PlaceNPCsAsync(Cell[] cells)
    {
        if (room.NPC == null) { yield break; }
        int randomNumberOfNPCs = Random.Range(0, 3);

        for (int i = 0; i < randomNumberOfNPCs; i++)
        {
            var availableCells = cells.Where(c => c.zone != CellTag.Taken).ToArray();
            int randomCell = Random.Range(0, availableCells.Length);
            var cell = availableCells[randomCell];

            PlaceNPC(cell.position);
            yield return null;
        }
    }

    private void PlaceNPC(Vector3 position)
    {
        var npc = Instantiate(room.NPC);
        npc.transform.position = position;
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
                var possibleElements = roomContent.Decoration.Where(x => x.DecorationAsset.zone == cell.zone).ToList();
                if (possibleElements.Count > 0)
                {
                    var decoration = PickOneAsset(possibleElements);

                    if (decoration == null) { continue; }

                    var pos = cell.position;
                    //var rot = GetRotation(cell.side);

                    var decorationAsset = Instantiate(decoration.prefab, pos + new Vector3(0, 0.1f, 0), Quaternion.identity);

                    yield return new WaitForSeconds(0.2f);

                    if (IsInsideRoom(decoration) && !IsOverlap(decorationAsset, cells))
                    {
                        cells = RemoveArea(cells);
                    }
                    else
                    {
                        Destroy(decorationAsset);
                    }

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
                cell.zone = CellTag.Taken;
            }
        }

        return cells;
    }

    private bool IsOverlap(GameObject decorationObject, Cell[] cells)
    {
        foreach (var cell in cells.Where(cell => cell.zone == CellTag.Taken))
        {
            var hits = Physics.BoxCastAll(cell.position, new Vector3(cell.Size.x, 0, cell.Size.y) / 2, Vector3.up, transform.rotation, 5f/*, layerMask*/);
            if (hits.Any())
            {
                // Items with Childelements do not work, because the transform is the parent.
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


    private DecorationAsset PickOneAsset(List<Decoration> possibleElements)
    {
        float random = Random.Range(0, 1f);
        var viableElements = possibleElements.Where(element => element.probability >= random).OrderBy(e => e.probability).ToList();

        if (!viableElements.Any()) { return null; }

        var lowestChance = viableElements.Min(e => e.probability);

        var elementsWithLowestChance = viableElements.Where(e => e.probability == lowestChance).ToList();

        var randomIndex = Random.Range(0, elementsWithLowestChance.Count);

        return elementsWithLowestChance[randomIndex].DecorationAsset;

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