using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Placable", menuName = "Placable Object/Create a object")]
public class PlacableObjectSO : ScriptableObject
{
    public static Direction GetNextDirection(Direction currentDirection)
    {
        switch (currentDirection)
        {
            default:
            case Direction.Down: return Direction.Left;
            case Direction.Left: return Direction.Down;
            //case Direction.Up: return Direction.Right;
            //case Direction.Right: return Direction.Down;
        }
    }

    public enum Direction
    {
        Down,
        Left,
        //Up,
        //Right
    }

    public static int GetRotationAngle(Direction direction)
    {
        switch (direction)
        {
            default:
            case Direction.Down:
                return 0;
            case Direction.Left:
                return 90;
            //case Direction.Up:
            //    return 180;
            //case Direction.Right:
            //    return 270;
        }
    }


    public string nameString;
    public Transform prefab;
    public Transform visual;
    public int width;
    public int height;

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Direction direction)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();

        switch (direction)
        {
            default:
            case Direction.Down:
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        gridPositionList.Add(offset +
                            new Vector2Int(x, y));
                    }
                }
                break;
            case Direction.Left:
                for (int x = 0; x < height; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        gridPositionList.Add(offset +
                            new Vector2Int(x, y));
                    }
                }
                break;
        }


       

        return gridPositionList;
    }
}
