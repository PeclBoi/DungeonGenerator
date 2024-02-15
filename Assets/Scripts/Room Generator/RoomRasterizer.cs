using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomRasterizer : MonoBehaviour
{

    private Collider _collider;

    public int rasterSize;

    public GameObject point;

    public float SpaceX { get { return _spaceX; } private set { _spaceX = value; } }
    public float SpaceY { get { return _spaceX; } private set { _spaceY = value; } }

    private float _spaceX;
    private float _spaceY;
    private float _tileWidth;
    private float _tileHeight;

    private Vector2 startPos;
    float xOffset = 0;
    float yOffset = 0;

    List<Cell> cells = new();

    public LayerMask layerMask;


    void Awake()
    {
        _collider = GetComponent<Collider>();

        startPos = new Vector2(_collider.bounds.min.x, _collider.bounds.min.z);

        _tileWidth = _collider.bounds.max.x - _collider.bounds.min.x;
        _tileHeight = _collider.bounds.max.x - _collider.bounds.min.x;

        _spaceX = _tileWidth / rasterSize;
        _spaceY = _tileWidth / rasterSize;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            RemoveOverlappedCells();
        }
    }

    public List<Cell> Rasterize()
    {
        yOffset = _spaceY / 2;
        while (yOffset <= _tileHeight)
        {
            xOffset = _spaceX / 2; ;
            while (xOffset <= _tileWidth)
            {
                var pos = new Vector3(startPos.x + xOffset, transform.position.y, startPos.y + yOffset);

                var marker = Instantiate(point, pos, Quaternion.identity);
                var cell = marker.AddComponent<Cell>();
                /*new Cell(marker.transform.position, CellTag.Inside, CellSideTag.None);*/
                cell.position = pos;
                cell.zone = CellTag.Inside;
                cell.side = CellSideTag.None;
                cell.Size = new Vector2(_spaceX, _spaceY);

                cells.Add(cell);
                xOffset += _spaceX;
            }
            yOffset += _spaceY;
        }

        return cells;

    }

    void RemoveOverlappedCells()
    {

        foreach (var cell in cells)
        {
            if (Physics.BoxCast(cell.position + new Vector3(0, 0.1f, 0), new Vector3(_spaceX, 0, _spaceY) / 2, Vector3.up, Quaternion.identity, 3, layerMask))
            {
                cell.position += new Vector3(0, 1, 0);
            }
        }
    }

}
