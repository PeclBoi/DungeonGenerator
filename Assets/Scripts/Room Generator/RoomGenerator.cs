using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;


public class Rectangle
{
    public int Width { get; }
    public int Heigth { get; }
    public Point OriginCorner { get; }
    public List<Line> Lines { get; private set; } = new List<Line>();

    public Rectangle(int width, int heigth, Point originCorner)
    {
        Width = width;
        Heigth = heigth;
        OriginCorner = originCorner;

        DetermineLines();
    }


    private void DetermineLines()
    {
        Mathf.Clamp(OriginCorner.y + Heigth, 0, 25);

        var point2 = new Point(OriginCorner.x, Mathf.Clamp(OriginCorner.y + Heigth, 0, 25));
        var point3 = new Point(Mathf.Clamp(OriginCorner.x + Width, 0, 25), Mathf.Clamp(OriginCorner.y + Heigth, 0, 25));
        var point4 = new Point(Mathf.Clamp(OriginCorner.x + Width, 0, 25), OriginCorner.y);

        Lines.Add(new Line(OriginCorner, point2));
        Lines.Add(new Line(point2, point3));
        Lines.Add(new Line(point3, point4));
        Lines.Add(new Line(point4, OriginCorner));
    }
}


public class Line
{
    public Point PointA { get; }
    public Point PointB { get; }


    //public bool Contains(Vector2 point)
    //{
    //    return point == PointA || point == PointB;
    //}

    public Line(Point pointA, Point pointB)
    {
        PointA = pointA;
        PointB = pointB;
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
}

public class Point
{

    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Point(float x, float y)
    {
        this.x = (int)x;
        this.y = (int)y;
    }

    public Point(Vector2 vector)
    {
        x = (int)vector.x;
        y = (int)vector.y;
    }

    public bool TryGetNextPointUp(List<Point> points, out Point nextPoint)
    {
        nextPoint = points.Where(p => p.x == x && p.y > y).OrderBy(p => p.y).FirstOrDefault();

        return nextPoint != null;
    }

    public bool TryGetNextPointDown(List<Point> points, out Point nextPoint)
    {
        nextPoint = points.Where(p => p.x == x && p.y < y).OrderByDescending(p => p.y).FirstOrDefault();
        return nextPoint != null;
    }

    public bool TryGetNextPointRight(List<Point> points, out Point nextPoint)
    {
        nextPoint = points.Where(p => p.y == y && p.x > x).OrderBy(p => p.x).FirstOrDefault();
        return nextPoint != null;
    }

    public bool TryGetNextPointLeft(List<Point> points, out Point nextPoint)
    {
        nextPoint = points.Where(p => p.y == y && p.x < x).OrderByDescending(p => p.x).FirstOrDefault();
        return nextPoint != null;
    }

}


public class RoomGenerator : MonoBehaviour
{

    public int dimensions;
    public int numberOfSegments;

    public GameObject block;
    public GameObject redBlock;
    public GameObject blueBlock;

    GameObject[] grid;

    int layer = 0;
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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            List<Point> points = new();
            List<Line> lineTable = new();
            int roomMinSize = 4;

            for (int i = 0; i < numberOfSegments; i++)
            {
                var x = UnityEngine.Random.Range(0, dimensions - roomMinSize - 1);
                var y = UnityEngine.Random.Range(0, dimensions - roomMinSize - 1);

                var origin = new Point(x, y);
                int width = UnityEngine.Random.Range(roomMinSize, dimensions - x);
                int height = UnityEngine.Random.Range(roomMinSize, dimensions - y);

                Debug.Log("centre: " + origin.x + "|" + origin.y);
                Debug.Log("heigt: " + height + " width: " + width);
                var roomPart = new Rectangle(width, height, origin);

                //var roomPart = new Rectangle(7, 10, new Point(15, 11));
                //var roomPart1 = new Rectangle(6, 4, new Point(4, 13));
                //var roomPart2 = new Rectangle(18, 6, new Point(6, 9));

                foreach (var line in roomPart.Lines)
                {
                    points.Add(line.PointA);
                    points.Add(line.PointB);
                }
                //foreach (var line in roomPart1.Lines)
                //{
                //    points.Add(line.PointA);
                //    points.Add(line.PointB);
                //}
                //foreach (var line in roomPart2.Lines)
                //{
                //    points.Add(line.PointA);
                //    points.Add(line.PointB);
                //}

                lineTable.AddRange(roomPart.Lines);
                //lineTable.AddRange(roomPart1.Lines);
                //lineTable.AddRange(roomPart2.Lines);
            }


            List<Point> crossPoints;



            GetCrossingLines(lineTable, out crossPoints);
            DrawLine(lineTable, redBlock);
            //points.AddRange(lineTable.DistinctBy(l => l.PointA).Select(l => l.PointA).ToList());
            points = crossPoints.Distinct().ToList();
            var orderedPoints = CreateOutline(points);
            DrawPoints(orderedPoints);
            // StartCoroutine(DrawPoints(orderedPoints));
            lineTable = ConnectLines(orderedPoints);


            //StartCoroutine(DrawPoints(orderedPoints));
            DrawLine(lineTable, blueBlock);
            Debug.Log("Done.");
        }
    }

    private List<Line> ConnectLines(List<Point> orderedPoints)
    {

        var line = new List<Line>();

        for (int i = 0; i < orderedPoints.Count - 1; i++)
        {
            line.Add(new Line(orderedPoints[i], orderedPoints[i + 1]));
        }

        return line;

    }

    private List<Point> CreateOutline(List<Point> points)
    {
        List<Point> orderedPoints = new List<Point>();


        //Point leftUpperCorner = points.FirstOrDefault(p => p.x == points.Min(p => p.x) && p.y == points.Where(p => p.x == points.Min(p => p.x)).Max(p => p.y));
        //Point rightUpperCorner = points.FirstOrDefault(p => p.x == points.Max(p => p.x) && p.y == points.Where(p => p.x == points.Max(p => p.x)).Max(p => p.y));
        //Point leftLowerCorner = points.FirstOrDefault(p => p.y == points.Min(p => p.y) && p.x == points.Where(p => p.y == points.Min(p => p.y)).Min(p => p.x));
        //Point rightLowerCorner = points.FirstOrDefault(p => p.x == points.Max(p => p.x) && p.y == points.Where(p => p.x == points.Max(p => p.x)).Min(p => p.y));

        int xThreshHold = (points.Max(p => p.x) + points.Min(p => p.x)) / 2;
        int yThreshHold = (points.Max(p => p.y) + points.Min(p => p.y)) / 2;


        var point = points.FirstOrDefault(p => p.x == points.Min(p => p.x) && p.y == points.Where(p => p.x == points.Min(p => p.x)).Max(p => p.y));
        var startPoint = point;
        orderedPoints.Add(point);
        Point nextPoint;

        int i = 0;
        do
        {
            i++;
            Debug.Log(i);
            if (point.x <= xThreshHold && point.y >= yThreshHold)
            {
                Point tempPoint = new(0, 0);
                if (point.TryGetNextPointLeft(points, out nextPoint))
                {
                    if (!orderedPoints.Contains(nextPoint))
                    {
                        tempPoint = nextPoint;
                    }
                }
                if (point.TryGetNextPointDown(points, out nextPoint))
                {
                    if (!orderedPoints.Contains(nextPoint))
                    {
                        tempPoint = nextPoint;
                    }
                }
                if (point.TryGetNextPointRight(points, out nextPoint))
                {
                    if (!orderedPoints.Contains(nextPoint))
                    {
                        tempPoint = nextPoint;
                    }
                }
                if (point.TryGetNextPointUp(points, out nextPoint))
                {
                    if (!orderedPoints.Contains(nextPoint))
                    {
                        tempPoint = nextPoint;
                    }
                }
                point = tempPoint;
                orderedPoints.Add(point);
            }
            else if (point.x >= xThreshHold && point.y >= yThreshHold)
            {
                Point tempPoint = new(0, 0);
                if (point.TryGetNextPointLeft(points, out nextPoint))
                {
                    if (!orderedPoints.Contains(nextPoint))
                    {
                        tempPoint = nextPoint;
                    }
                }
                if (point.TryGetNextPointDown(points, out nextPoint))
                {
                    if (!orderedPoints.Contains(nextPoint))
                    {
                        tempPoint = nextPoint;
                    }
                }
                if (point.TryGetNextPointRight(points, out nextPoint))
                {
                    if (!orderedPoints.Contains(nextPoint))
                    {
                        tempPoint = nextPoint;
                    }
                }
                if (point.TryGetNextPointUp(points, out nextPoint))
                {
                    if (!orderedPoints.Contains(nextPoint))
                    {
                        tempPoint = nextPoint;
                    }
                }
                point = tempPoint;
                orderedPoints.Add(point);
            }
            else if (point.x >= xThreshHold && point.y <= yThreshHold)
            {
                Point tempPoint = new(0, 0);

                if (point.TryGetNextPointUp(points, out nextPoint))
                {
                    if (!orderedPoints.Contains(nextPoint))
                    {
                        tempPoint = nextPoint;
                    }
                }
                if (point.TryGetNextPointLeft(points, out nextPoint))
                {
                    if (!orderedPoints.Contains(nextPoint))
                    {
                        tempPoint = nextPoint;
                    }
                }
                if (point.TryGetNextPointDown(points, out nextPoint))
                {
                    if (!orderedPoints.Contains(nextPoint))
                    {
                        tempPoint = nextPoint;
                    }
                }
                if (point.TryGetNextPointRight(points, out nextPoint))
                {
                    if (!orderedPoints.Contains(nextPoint))
                    {
                        tempPoint = nextPoint;
                    }
                }

                point = tempPoint;
                orderedPoints.Add(point);

            }
            else if (point.x <= xThreshHold && point.y <= yThreshHold)
            {
                var tempPoint = new Point(0,0);
                if (point.TryGetNextPointRight(points, out nextPoint))
                {
                    if (!orderedPoints.Contains(nextPoint) || nextPoint == startPoint)
                    {
                        tempPoint = nextPoint;
                    }
                }
                if (point.TryGetNextPointUp(points, out nextPoint))
                {
                    if (!orderedPoints.Contains(nextPoint) || nextPoint == startPoint)
                    {
                        tempPoint = nextPoint;
                    }
                }
                if (point.TryGetNextPointLeft(points, out nextPoint))
                {
                    if (!orderedPoints.Contains(nextPoint) || nextPoint == startPoint)
                    {
                        tempPoint = nextPoint;
                    }
                }
                if (point.TryGetNextPointDown(points, out nextPoint))
                {
                    if (!orderedPoints.Contains(nextPoint) || nextPoint == startPoint)
                    {
                        tempPoint = nextPoint;
                    }
                }

                point = tempPoint;
                orderedPoints.Add(point);

            }
            else
            {
                Debug.Log("Error amana");
            }

        } while (point != startPoint && i < 200);

        orderedPoints.Add(startPoint);

        #region old_try
        //// ----

        //i = 0;
        //do
        //{
        //    i++;
        //    if (point.TryGetNextPointRight(points, out nextPoint))
        //    {
        //        if (!orderedPoints.Contains(nextPoint))
        //        {
        //            point = nextPoint;
        //            orderedPoints.Add(point);
        //        }
        //    }
        //    else if (point.TryGetNextPointDown(points, out nextPoint))
        //    {
        //        if (!orderedPoints.Contains(nextPoint))
        //        {
        //            point = nextPoint;
        //            orderedPoints.Add(point);
        //        }
        //    }
        //    else if (point.TryGetNextPointLeft(points, out nextPoint))
        //    {
        //        if (!orderedPoints.Contains(nextPoint))
        //        {
        //            point = nextPoint;
        //            orderedPoints.Add(point);
        //        }
        //    }
        //    else if (point.TryGetNextPointUp(points, out nextPoint))
        //    {
        //        if (!orderedPoints.Contains(nextPoint))
        //        {
        //            point = nextPoint;
        //            orderedPoints.Add(point);
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("Error amana");
        //    }

        //} while (point != rightLowerCorner && i < 200);

        //// ----

        //i = 0;
        //do
        //{
        //    i++;
        //    if (point.TryGetNextPointDown(points, out nextPoint))
        //    {
        //        if (!orderedPoints.Contains(nextPoint))
        //        {
        //            point = nextPoint;
        //            orderedPoints.Add(point);
        //        }
        //    }
        //    else if (point.TryGetNextPointLeft(points, out nextPoint))
        //    {
        //        if (!orderedPoints.Contains(nextPoint))
        //        {
        //            point = nextPoint;
        //            orderedPoints.Add(point);
        //        }
        //    }
        //    else if (point.TryGetNextPointUp(points, out nextPoint))
        //    {
        //        if (!orderedPoints.Contains(nextPoint))
        //        {
        //            point = nextPoint;
        //            orderedPoints.Add(point);
        //        }
        //    }
        //    else if (point.TryGetNextPointRight(points, out nextPoint))
        //    {
        //        if (!orderedPoints.Contains(nextPoint))
        //        {
        //            point = nextPoint;
        //            orderedPoints.Add(point);
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("Error amana");
        //    }

        //} while (point != leftLowerCorner && i < 200);

        //// ----

        //i = 0;
        //while (point != leftUpperCorner && i < 200)
        //{
        //    i++;
        //    if (point.TryGetNextPointLeft(points, out nextPoint))
        //    {
        //        if (!orderedPoints.Contains(nextPoint))
        //        {
        //            point = nextPoint;
        //            orderedPoints.Add(point);
        //        }
        //    }
        //    else if (point.TryGetNextPointUp(points, out nextPoint))
        //    {
        //        if (!orderedPoints.Contains(nextPoint))
        //        {
        //            point = nextPoint;
        //            orderedPoints.Add(point);
        //        }
        //    }
        //    else if (point.TryGetNextPointRight(points, out nextPoint))
        //    {
        //        if (!orderedPoints.Contains(nextPoint))
        //        {
        //            point = nextPoint;
        //            orderedPoints.Add(point);
        //        }
        //    }
        //    else if (point.TryGetNextPointDown(points, out nextPoint))
        //    {
        //        if (!orderedPoints.Contains(nextPoint))
        //        {
        //            point = nextPoint;
        //            orderedPoints.Add(point);
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("Error amana");
        //    }
        //}
        //orderedPoints.Insert(0, leftUpperCorner);
        #endregion

        return orderedPoints;
    }


    private List<Line> GetCrossingLines(List<Line> lineTable, out List<Point> crossPoints)
    {
        var horizontalLines = lineTable.Where(l => l.IsHorizonal).ToList();
        var verticalLines = lineTable.Where(l => l.IsVertical).ToList();

        var newLineTable = lineTable;
        var crossingPoints = new List<Point>();

        foreach (var line in verticalLines)
        {
            foreach (var horizontalLine in horizontalLines)
            {
                var lowerLimit = Mathf.Min(horizontalLine.PointA.x, horizontalLine.PointB.x);
                var upperLimit = Mathf.Max(horizontalLine.PointA.x, horizontalLine.PointB.x);

                var lowerYPos = Mathf.Min(line.PointA.y, line.PointB.y);
                var upperYPos = Mathf.Max(line.PointA.y, line.PointB.y);

                if (line.PointA.x >= lowerLimit && line.PointA.x <= upperLimit && lowerYPos <= horizontalLine.PointA.y && upperYPos >= horizontalLine.PointA.y)
                {
                    var crossingPoint = new Point(line.PointA.x, horizontalLine.PointA.y);
                    crossingPoints.Add(crossingPoint);
                }
            }
        }

        crossPoints = crossingPoints;

        return newLineTable;
    }





    public void DrawLine(List<Line> lineTable, GameObject drawBlock)
    {
        layer += 2;
        foreach (var line in lineTable)
        {

            int horitonzalDiff = (int)MathF.Abs(line.PointA.x - line.PointB.x);
            int verticalDiff = (int)MathF.Abs(line.PointA.y - line.PointB.y);


            int startX = Mathf.Min(line.PointA.x, line.PointB.x);
            int startY = Mathf.Min(line.PointA.y, line.PointB.y);
            for (int y = startY; y <= startY + verticalDiff; y++)
            {
                for (int x = startX; x <= startX + horitonzalDiff; x++)
                {
                    int index = x + y * dimensions;
                    //Destroy(grid[index]);
                    grid[index] = Instantiate(drawBlock, new Vector3(x, y, -layer), Quaternion.identity);
                }
            }
        }
    }


    IEnumerator DrawPoints(List<Vector2> points)
    {
        int i = 0;
        foreach (var point in points)
        {
            Debug.Log(i++);
            int index = (int)point.x + (int)point.y * dimensions;
            grid[index] = Instantiate(redBlock, new Vector3(point.x, point.y, -i), Quaternion.identity);

            yield return new WaitForSeconds(1f);
        }
    }

    public void DrawPoints(List<Point> points)
    {
        foreach (var point in points)
        {
            int index = point.x + point.y * dimensions;
            grid[index] = Instantiate(blueBlock, new Vector2(point.x, point.y), Quaternion.identity);
        }
    }
}


