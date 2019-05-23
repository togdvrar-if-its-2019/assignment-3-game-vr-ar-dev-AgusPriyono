using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    private int[,] map;
    public GameObject[,] blocks;

    void Start()
    {
        map = new int[7, 11];

        blocks = new GameObject[7, 11];

        for (int i = 1; i < map.GetLength(0) - 1; i += 1)
        {
            for (int j = 1; j < map.GetLength(1) - 1; j += 1)
            {
                if ((i % 2 == 0 && j % 2 == 1) || (i % 2 == 1 && j % 2 == 0) || (i % 2 == 0 && j % 2 == 0))
                {
                    blocks[i, j] = GameObject.Find("tale_" + i + "_" + j);

                }
            }
        }

        for (int i = 0; i < map.GetLength(0); i += 1)
        {
            for (int j = 0; j < map.GetLength(1); j += 1)
            {
                if (i == 0 || i == map.GetLength(0) - 1 || j == 0 || j == map.GetLength(1) - 1 || (i % 2 == 0 && j % 2 == 0))
                {
                    map[i, j] = 1;
                }
            }
        }

        divide3(map, true, 0, 6, 0, 10);

        for (int i = 1; i < map.GetLength(0) - 1; i += 1)
        {
            for (int j = 1; j < map.GetLength(1) - 1; j += 1)
            {
                if (i % 2 == 0 && j % 2 == 0)
                {
                    if (map[i - 1, j] == 0 && map[i, j - 1] == 0 && map[i + 1, j] == 0 && map[i, j + 1] == 0)
                    {
                        map[i, j] = 0;
                    }
                }
            }
        }

        for (int i = 1; i < map.GetLength(0) - 1; i += 1)
        {
            for (int j = 1; j < map.GetLength(1) - 1; j += 1)
            {
                if ((i % 2 == 0 && j % 2 == 1) || (i % 2 == 1 && j % 2 == 0) || (i % 2 == 0 && j % 2 == 0))
                {
                    if (map[i, j] == 0)
                    {
                        Destroy(blocks[i, j]);
                    }
                }
            }
        }


        Solving solving = new Solving(map);
    }

    void Update()
    {
       // Debug.Log(Random.Range(0, 2)); // 0 or 1 only
        if (Input.GetKeyDown(KeyCode.M))
        {
            Application.LoadLevel(Application.loadedLevelName);
        }
    }

    void show(int[,] map)
    {
        for (int i = 0; i < 7; i += 1)
        {
            string a = "";
            for (int j = 0; j < 11; j += 1)
            {
                a = a + " " + map[i, j];
            }
            Debug.Log(a);
        }
    }

    void divide3(int[,] map, bool vertical, int top, int bottom, int left, int right)
    {
        if (vertical)
        {
            if (right - left >= 4)
            {
                int middle_column = left + ((right - left) / 2);
                if (middle_column % 2 == 1)
                {
                    middle_column++;
                }
                int destroy = (Random.Range(0, ((bottom - top) / 2)) * 2) + top + 1;

                for (int i = (top + 1); i <= (bottom - 1); i++)
                {
                    if (i != destroy)
                    {
                        map[i, middle_column] = 1;
                    }
                }

                //Debug.Log("create line vertical at column " + middle_column + " at row from " + (top + 1) + " to " + (bottom - 1) + " destroy " + destroy);

                divide3(map, !vertical, top, bottom, left, middle_column);
                divide3(map, !vertical, top, bottom, middle_column, right);
            }
        }
        else
        {
            if (bottom - top >= 4)
            {
                int middle_row = top + ((bottom - top) / 2);
                if (middle_row % 2 == 1)
                {
                    middle_row++;
                }
                int destroy = (Random.Range(0, ((right - left) / 2)) * 2) + left + 1;
                int destroy2 = (Random.Range(0, ((right - left) / 2)) * 2) + left + 1;

                for (int i = (left + 1); i <= (right - 1); i++)
                {
                    if (i != destroy && i != destroy2)
                    {
                        map[middle_row, i] = 1;
                    }
                }

               // Debug.Log("create line horizontal at row " + middle_row + " at column from " + (left + 1) + " to " + (right - 1) + " destroy " + destroy);

                divide3(map, !vertical, top, middle_row, left, right);
                divide3(map, !vertical, middle_row, bottom, left, right);
            }
        }
    }

}