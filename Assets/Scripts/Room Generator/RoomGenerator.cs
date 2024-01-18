using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;


public class Rectangle
{
    public int Width { get; }
    public int Heigth { get; }
    public Vector2 OriginCorner { get; }
    public List<Line> Lines { get; private set; } = new List<Line>();

    public Rectangle(int width, int heigth, Vector2 originCorner)
    {
        Width = width;
        Heigth = heigth;
        OriginCorner = originCorner;

        DetermineLines();
    }


    private void DetermineLines()
    {
        Vector2 point2 = new Vector2(OriginCorner.x, OriginCorner.y + Heigth);
        Vector2 point3 = new Vector2(OriginCorner.x + Width, OriginCorner.y + Heigth);
        Vector2 point4 = new Vector2(OriginCorner.x + Width, OriginCorner.y);

        Lines.Add(new Line(OriginCorner, point2));
        Lines.Add(new Line(point2, point3));
        Lines.Add(new Line(point3, point4));
        Lines.Add(new Line(point4, OriginCorner));
    }
}


public class Line
{
    public Vector2 PointA { get; }
    public Vector2 PointB { get; }


    public bool IsParallelTo(Line other)
    {
        var vector = PointB - PointA;
        var otherVector = other.PointB - other.PointA;


        return vector.x * otherVector.y - vector.y * otherVector.x == 0;

    }

    public bool Contains(Vector2 point)
    {
        return point == PointA || point == PointB;
    }

    public Line(Vector2 pointA, Vector2 pointB)
    {
        PointA = pointA;
        PointB = pointB;
    }

    public bool Equals(Line other)
    {
        return Contains(other.PointA) && Contains(other.PointB);
    }

    public bool IsHorizonal
    {
        get
        {
            return PointA.y == PointB.y;
        }
    }

    public bool IsVertical
    {
        get
        {
            return PointA.x == PointB.x;
        }
    }

    public bool IsIntersecting(Line line)
    {

        if (line == this)
        {
            return false;
        }

        if (line.PointA.y == PointA.y && line.PointB.y == PointB.y && line.PointA.y == line.PointB.y)
        {

            float minX = Mathf.Min(line.PointA.x, line.PointB.x);
            float lineTableEntryMinX = Mathf.Min(PointA.x, PointB.x);

            float maxX = Mathf.Max(line.PointA.x, line.PointB.x);
            float lineTableEntryMaxX = Mathf.Max(PointA.x, PointB.x);

            if (minX >= lineTableEntryMinX && maxX <= lineTableEntryMaxX)
            {
                return true;
            }
        }


        if (line.PointA.x == PointA.x && line.PointB.x == PointB.x && line.PointA.x == line.PointB.x)
        {

            float minY = Mathf.Min(line.PointA.y, line.PointB.y);
            float lineTableEntryMinY = Mathf.Min(PointA.y, PointB.y);

            float maxY = Mathf.Max(line.PointA.y, line.PointB.y);
            float lineTableEntryMaxY = Mathf.Max(PointA.y, PointB.y);

            if (minY >= lineTableEntryMinY && maxY <= lineTableEntryMaxY)
            {
                return true;
            }
        }

        return false;
    }

    public int CompareTo(Line other)
    {
        if (other.PointA == PointA && other.PointB == PointB)
        {
            return 0;
        }
        else return 1;
    }
}

public class OverlappingLinePair
{
    public Line line1 { get; set; }
    public Line line2 { get; set; }
}

public class RoomGenerator : MonoBehaviour
{

    public int dimensions;

    public GameObject block;
    public GameObject redBlock;
    public GameObject blueBlock;

    GameObject[] grid;

    void Start()
    {

        grid = new GameObject[dimensions * dimensions];

        for (int y = 0; y < dimensions; y++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                grid[x + y * dimensions] = Instantiate(block, new Vector2(x, y), Quaternion.identity);
            }
        }

        List<Line> lineTable = new();

        var roomPart1 = new Rectangle(15, 6, new Vector2(4, 4));
        var roomPart3 = new Rectangle(7, 7, new Vector2(10, 6));
        var roomPart2 = new Rectangle(4, 6, new Vector2(10, 10));

        lineTable.AddRange(roomPart1.Lines);
        lineTable.AddRange(roomPart3.Lines);
        lineTable.AddRange(roomPart2.Lines);

        List<Vector2> crossPoints;
        lineTable = GetCrossingLines(lineTable, out crossPoints);

        EliminateInnerPoints(lineTable);

        SimplifyForm(lineTable);

        RemoveDuplicates(lineTable);

        List<Line> linesToAdd = new();
        List<Line> linesToRemove = new();
        foreach (var line in lineTable)
        {
            List<Vector2> points = new();

            var parallelLines = lineTable.Where(l => l.IsParallelTo(line));

            if(linesToRemove.Any(l => l.Equals(line))) { continue; }

            if (line.IsHorizonal)
            {
                var overlappingLines = parallelLines.Where(l => l.PointA.y == line.PointA.y).ToList();
                if (overlappingLines.Count > 1)
                {
                    foreach (var overlappingLine in overlappingLines)
                    {
                        points.Add(overlappingLine.PointA);
                        points.Add(overlappingLine.PointB);
                    }

                    points = points.OrderBy(p => p.y).ToList();

                    linesToRemove.AddRange(overlappingLines);
                    linesToAdd.Add(new Line(points.First(), points.Last()));
                }
            }
            else
            {
                var overlappingLines = parallelLines.Where(l => l.PointA.x == line.PointA.x).ToList();
                if (overlappingLines.Count > 1)
                {
                    foreach (var overlappingLine in overlappingLines)
                    {
                        points.Add(overlappingLine.PointA);
                        points.Add(overlappingLine.PointB);
                    }

                    points = points.OrderBy(p => p.y).ToList();

                    linesToRemove.AddRange(overlappingLines);
                    linesToAdd.Add(new Line(points.First(), points.Last()));
                }
            }
        }

        foreach (var lineToRemove in linesToRemove)
        {
            lineTable.Remove(lineToRemove);
        }

        lineTable.AddRange(linesToAdd);


        StartCoroutine(DrawLine(lineTable, redBlock));
        Debug.Log("Done.");
    }

    private void SimplifyForm(List<Line> lineTable)
    {

        List<Line> linesToRemove = new List<Line>();
        List<Line> linesToAdd = new List<Line>();

        var vertLines = lineTable.Where(l => l.IsVertical);
        foreach (var line in vertLines)
        {
            var linesWithSamePoint = vertLines.Where(l => l.Contains(line.PointA)).ToList();
            if (linesWithSamePoint.Count() > 1)
            {

                List<Vector2> points = new List<Vector2>();
                foreach (var lineWithSamePoint in linesWithSamePoint)
                {
                    points.Add(lineWithSamePoint.PointA);
                    points.Add(lineWithSamePoint.PointB);
                }

                points = points.OrderBy(p => p.y).ToList();
                linesToAdd.Add(new Line(points.First(), points.Last()));

                linesToRemove.AddRange(linesWithSamePoint);
            }

            linesWithSamePoint = vertLines.Where(l => l.Contains(line.PointB)).ToList();
            if (linesWithSamePoint.Count() > 1)
            {

                List<Vector2> points = new List<Vector2>();
                foreach (var lineWithSamePoint in linesWithSamePoint)
                {
                    points.Add(lineWithSamePoint.PointA);
                    points.Add(lineWithSamePoint.PointB);
                }

                points = points.OrderBy(p => p.y).ToList();
                linesToAdd.Add(new Line(points.First(), points.Last()));


                linesToRemove.AddRange(linesWithSamePoint);
            }
        }

        var horizontalLines = lineTable.Where(l => l.IsHorizonal);
        foreach (var line in horizontalLines)
        {
            var linesWithSamePoint = horizontalLines.Where(l => l.Contains(line.PointA)).ToList();
            if (linesWithSamePoint.Count() > 1)
            {

                List<Vector2> points = new List<Vector2>();
                foreach (var lineWithSamePoint in linesWithSamePoint)
                {
                    points.Add(lineWithSamePoint.PointA);
                    points.Add(lineWithSamePoint.PointB);
                }

                points = points.OrderBy(p => p.x).ToList();
                linesToAdd.Add(new Line(points.First(), points.Last()));

                linesToRemove.AddRange(linesWithSamePoint);
            }

            linesWithSamePoint = horizontalLines.Where(l => l.Contains(line.PointB)).ToList();
            if (linesWithSamePoint.Count() > 1)
            {

                List<Vector2> points = new List<Vector2>();
                foreach (var lineWithSamePoint in linesWithSamePoint)
                {
                    points.Add(lineWithSamePoint.PointA);
                    points.Add(lineWithSamePoint.PointB);
                }

                points = points.OrderBy(p => p.x).ToList();
                linesToAdd.Add(new Line(points.First(), points.Last()));


                linesToRemove.AddRange(linesWithSamePoint);
            }
        }

        foreach (var lineToRemove in linesToRemove)
        {
            lineTable.Remove(lineToRemove);
        }

        lineTable.AddRange(linesToAdd);

    }

    private void RemoveDuplicates(List<Line> lineTable)
    {
        for (int i = 0; i < lineTable.Count; i++)
        {
            if (lineTable.Count(l => l.Equals(lineTable[i])) > 1)
            {
                lineTable.RemoveAt(i);
            }
        }
    }

    private void EliminateInnerPoints(List<Line> lineTable)
    {

        List<Vector2> points = new List<Vector2>();

        foreach (var line in lineTable)
        {
            points.Add(line.PointA);
            points.Add(line.PointB);
        }

        foreach (var point in points)
        {

            bool lowerRightPoint = points.Where(p => p.x > point.x && p.y > point.y).Any();
            bool uppperRightPoint = points.Where(p => p.x > point.x && p.y < point.y).Any();
            bool lowerLeftPoint = points.Where(p => p.x < point.x && p.y > point.y).Any();
            bool upperLeftPoint = points.Where(p => p.x < point.x && p.y > point.y).Any();

            if (lowerLeftPoint && lowerRightPoint && upperLeftPoint && uppperRightPoint)
            {

                var linesWithPoint = lineTable.Where(p => p.PointA == point || p.PointB == point).ToList();
                foreach (var lwp in linesWithPoint)
                {
                    lineTable.Remove(lwp);
                }
            }
        }
    }

    private List<Line> GetCrossingLines(List<Line> lineTable, out List<Vector2> crossPoints)
    {
        var horizontalLines = lineTable.Where(l => l.IsHorizonal).ToList();
        var verticalLines = lineTable.Where(l => l.IsVertical).ToList();

        var newLineTable = lineTable;
        var crossingPoints = new List<Vector2>();

        foreach (var line in verticalLines)
        {
            foreach (var horizontalLine in horizontalLines)
            {
                var lowerLimit = Mathf.Min(horizontalLine.PointA.x, horizontalLine.PointB.x);
                var upperLimit = Mathf.Max(horizontalLine.PointA.x, horizontalLine.PointB.x);

                var lowerYPos = Mathf.Min(line.PointA.y, line.PointB.y);
                var upperYPos = Mathf.Max(line.PointA.y, line.PointB.y);

                if (line.PointA.x > lowerLimit && lowerYPos < horizontalLine.PointA.y && upperYPos > horizontalLine.PointA.y)
                {
                    var crossingPoint = new Vector2(line.PointA.x, horizontalLine.PointA.y);
                    crossingPoints.Add(crossingPoint);
                    List<Line> splitLines = new List<Line>()
                        {
                            new Line(line.PointA, crossingPoint),
                            new Line(crossingPoint, line.PointB),
                            new Line(horizontalLine.PointA, crossingPoint),
                            new Line(crossingPoint, horizontalLine.PointB)
                        };

                    newLineTable.Remove(line);
                    newLineTable.Remove(horizontalLine);

                    newLineTable.AddRange(splitLines);
                }
            }
        }

        crossPoints = crossingPoints;

        return newLineTable;
    }



    private List<Line> RemoveLine(Line lineA, Line lineB)
    {

        List<Vector2> points = new List<Vector2>();

        points.Add(lineA.PointA);
        points.Add(lineA.PointB);
        points.Add(lineB.PointA);
        points.Add(lineB.PointB);

        if (lineA.IsHorizonal)
        {
            points = points.OrderBy(p => p.x).ToList();
        }
        else if (lineA.IsVertical)
        {
            points = points.OrderBy(p => p.y).ToList();
        }

        List<Line> lines = new List<Line>
        {
            new Line(points[0], points[3]),
            //new Line(points[2], points[3])
        };
        return lines;

    }

    private void DrawRectangle(Vector2 pointA, Vector2 pointB, GameObject drawBlock)
    {

        // StartCoroutine(DrawLine(pointA, pointB, drawBlock));

    }

    IEnumerator DrawLine(List<Line> lineTable, GameObject drawBlock)
    {
        int i = 0;
        foreach (var line in lineTable)
        {

            int horitonzalDiff = (int)MathF.Abs(line.PointA.x - line.PointB.x);
            int verticalDiff = (int)MathF.Abs(line.PointA.y - line.PointB.y);


            int startX = (int)Mathf.Min(line.PointA.x, line.PointB.x);
            int startY = (int)Mathf.Min(line.PointA.y, line.PointB.y);
            yield return new WaitForSeconds(1f);
            Debug.Log(i++);
            for (int y = startY; y <= startY + verticalDiff; y++)
            {
                for (int x = startX; x <= startX + horitonzalDiff; x++)
                {
                    int index = x + y * dimensions;
                    //Destroy(grid[index]);
                    grid[index] = Instantiate(drawBlock, new Vector3(x, y, i), Quaternion.identity);
                }
            }
        }
    }

}
