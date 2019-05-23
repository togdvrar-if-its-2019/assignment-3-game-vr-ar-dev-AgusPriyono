using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solving : MonoBehaviour
{
    public GameObject donut;
    Queue<MyVector2> before = new Queue<MyVector2>();
    Queue<MyVector2> after = new Queue<MyVector2>();
    int[,] map;
    int[,] track;
    int[,] parent;
    int[,] web;

    public static int UP = 55;
    public static int DOWN = 56;
    public static int LEFT = 57;
    public static int RIGHT = 58;
    public static int FIRST = 59;
    public static int UNDEFINED = 60;

    int web_count = 0;
    int web_count_target = 8;

    List<MyVector2> solved_path;
    List<MyVector2> all_path;
    List<MyVector2> expansion;
    List<MyVector2> temp;
    List<List<MyVector2>> special_food_position;
    List<MyVector2> special_food_temp;

    public Solving(int[,] map)
    {
        donut = GameObject.Find("makanan_2");
        this.map = map;
        expansion = new List<MyVector2>();
        solved_path = new List<MyVector2>();
        all_path = new List<MyVector2>();
        temp = new List<MyVector2>();
        special_food_temp = new List<MyVector2>();
        special_food_position = new List<List<MyVector2>>();
        this.track = new int[map.GetLength(0), map.GetLength(1)];
        this.parent = new int[map.GetLength(0), map.GetLength(1)];
        this.web = new int[map.GetLength(0), map.GetLength(1)];
        for (int i = 0; i < map.GetLength(0); i += 1)
        {
            for (int j = 0; j < map.GetLength(1); j += 1)
            {
                track[i, j] = map[i, j];
                parent[i, j] = map[i, j];
                web[i, j] = map[i, j];
            }
        }

        track[1, 1] = 1;
        parent[1, 1] = FIRST;
        before.Enqueue(new MyVector2(1, 1));

        // show(track);

        do
        {
            MyVector2 process = before.Dequeue();
            //Debug.Log("remove " + process.x + " - " + process.y + " from before");

            track[(int)process.x, (int)process.y] = 1;

            foreach (MyVector2 child in getChildren((int)process.x, (int)process.y))
            {
                //Debug.Log("add " + child.x + " - " + child.y + " to before");
                parent[child.x, child.y] = getParentDirection(child.x, child.y, process.x, process.y);
                before.Enqueue(child);
            }

            after.Enqueue(process);
            //Debug.Log("add " + process.x + " - " + process.y + " to after");

        } while (before.Count > 0);

        detectWeb();

        show(web);

        Debug.Log("WEB COUNT : " + web_count);

        searchExpansion();

        foodPlacement();
    }

    public void foodPlacement()
    {
        int food_count = 0;
        foreach (List<MyVector2> food in special_food_position) {
            if (food.Count>0)
            {
                int index = Random.Range(0, food.Count);
               // Debug.Log("size " + food.Count + " index is " + index);
                Debug.Log("makanan ditaruh " + food[index].x + " " + food[index].y);
                //1,76 col, 1,72 row
                food_count++;
                Instantiate(donut, new Vector3((food[index].y*1.76f)-8.94f, (1.72f*(6 - food[index].x)) - 5.03f, 0), Quaternion.identity);
            }
        }


        for (int i = 0; i < (5-food_count); i++) {
            int index = Random.Range(0, solved_path.Count);
            Instantiate(donut, new Vector3((solved_path[index].y * 1.76f) - 8.94f, (1.72f * (6 - solved_path[index].x)) - 5.03f, 0), Quaternion.identity);
        }
    }

    public void searchExpansion()
    {
        int expand_count = web_count_target - web_count;
        int size_all = 0;

        while (expand_count > 0 && expansion.Count > 0)
        {
            size_all = all_path.Count;
            int index = Random.Range(0, expansion.Count);
            Debug.Log("web now : " + web_count + " target : " + web_count_target + " expansion list " + expansion.Count);
            MyVector2 target = expansion[index];
            expansion.RemoveAt(index);
            if (canCreateVertical(target.x, target.y))
            {
                if (createWeb(target.x, target.y, UP))
                {
                    special_food_position.Add(special_food_temp);
                    web_count++;
                }
                Debug.Log(target.x + " " + target.y + " ver");
            }
            else
            {
                if (createWeb(target.x, target.y, LEFT))
                {
                    special_food_position.Add(special_food_temp);
                    web_count++;
                }
                Debug.Log(target.x + " " + target.y + " hor");
            }

            foreach (MyVector2 pos in temp)
            {
                if (pos.x != web.GetLength(0) - 2 && pos.y != web.GetLength(1) - 2 && pos.x % 2 == 1 && pos.y % 2 == 1)
                {
                    if (web[pos.x - 1, pos.y] == 0 || web[pos.x + 1, pos.y] == 0 || web[pos.x, pos.y - 1] == 0 || web[pos.x, pos.y + 1] == 0)
                    {

                        expansion.Add(new MyVector2(pos.x, pos.y));
                        Debug.Log(pos.x + " - " + pos.y + " is eligible ");

                    }
                }
            }
            expand_count = web_count_target - web_count;
        }

    }

    public bool canCreateVertical(int x, int y)
    {
        if (web[x - 1, y] == 0 || web[x + 1, y] == 0)
        {
            return true;
        }
        return false;
    }

    public void detectWeb()
    {

        int row = map.GetLength(0) - 2;
        int col = map.GetLength(1) - 2;

        do
        {
            solved_path.Add(new MyVector2(row, col));
            if (parent[row, col] == UP)
            {
                if (createWeb(row, col, UP))
                {
                    web_count++;
                }
                row--;
                //Debug.Log("naik");
            }
            else if (parent[row, col] == DOWN)
            {
                if (createWeb(row, col, DOWN))
                {
                    web_count++;
                }
                row++;
                //Debug.Log("turun");
            }
            else if (parent[row, col] == LEFT)
            {
                if (createWeb(row, col, LEFT))
                {
                    web_count++;
                }
                col--;
                //Debug.Log("kiri");
            }
            else if (parent[row, col] == RIGHT)
            {
                if (createWeb(row, col, RIGHT))
                {
                    web_count++;
                }
                col++;
                //Debug.Log("kanan");
            }

        } while (parent[row, col] != FIRST);

        solved_path.Add(new MyVector2(1, 1));
        solved_path.Reverse();

        foreach (MyVector2 pos in all_path)
        {
            Debug.Log("all path " + pos.x + " " + pos.y);
        }

        foreach (MyVector2 pos in all_path)
        {
            if (pos.x != web.GetLength(0) - 2 && pos.y != web.GetLength(1) - 2 && pos.x % 2 == 1 && pos.y % 2 == 1)
            {
                if (web[pos.x - 1, pos.y] == 0 || web[pos.x + 1, pos.y] == 0 || web[pos.x, pos.y - 1] == 0 || web[pos.x, pos.y + 1] == 0)
                {
                    expansion.Add(new MyVector2(pos.x, pos.y));
                    Debug.Log(pos.x + " - " + pos.y + " is eligible ");
                }
            }
        }

    }

    public bool createWeb(int row, int col, int direction)
    {
        temp = new List<MyVector2>();
        special_food_temp = new List<MyVector2>();
        int size_all = all_path.Count;
        int created_count = 0;
        if (direction == UP || direction == DOWN)
        {
            bool up_arrow = false, down_arrow = false;
            // create up
            if (web[row - 1, col] != 1 && web[row - 1, col] != 2)
            {
                int row_temp = row;
                up_arrow = true;
                do
                {
                    web[row_temp, col] = 2;
                    if (!isExist(new MyVector2(row_temp, col), all_path))
                    {
                        all_path.Add(new MyVector2(row_temp, col));
                        if (row_temp!=row)
                        {
                            special_food_temp.Add(new MyVector2(row_temp, col));
                        }
                        created_count++;
                    }
                    row_temp--;
                } while (web[row_temp, col] != 1 && web[row_temp, col] != 2);
            }
            // create down
            if (web[row + 1, col] != 1 && web[row + 1, col] != 2)
            {
                int row_temp = row;
                down_arrow = true;
                do
                {
                    web[row_temp, col] = 2;
                    if (!isExist(new MyVector2(row_temp, col), all_path))
                    {
                        all_path.Add(new MyVector2(row_temp, col));
                        temp.Add(new MyVector2(row_temp, col));
                        if (row_temp != row)
                        {
                            special_food_temp.Add(new MyVector2(row_temp, col));
                        }
                        created_count++;
                    }
                    row_temp++;
                } while (web[row_temp, col] != 1 && web[row_temp, col] != 2);
            }

            return (up_arrow || down_arrow);
        }
        else if (direction == LEFT || direction == RIGHT)
        {
            bool left_arrow = false, right_arrow = false;
            // create left
            if (web[row, col - 1] != 1 && web[row, col - 1] != 2)
            {
                int col_temp = col;
                left_arrow = true;
                do
                {

                    web[row, col_temp] = 2;
                    if (!isExist(new MyVector2(row, col_temp), all_path))
                    {
                        all_path.Add(new MyVector2(row, col_temp));
                        temp.Add(new MyVector2(row, col_temp));
                        if (col_temp != col)
                        {
                            special_food_temp.Add(new MyVector2(row, col_temp));
                        }
                        created_count++;
                    }
                    col_temp--;

                } while (web[row, col_temp] != 1 && web[row, col_temp] != 2);
            }

            // create right
            if (web[row, col + 1] != 1 && web[row, col + 1] != 2)
            {
                int col_temp = col;
                right_arrow = true;
                do
                {

                    web[row, col_temp] = 2;
                    if (!isExist(new MyVector2(row, col_temp), all_path))
                    {
                        all_path.Add(new MyVector2(row, col_temp));
                        temp.Add(new MyVector2(row, col_temp));
                        if (col_temp != col)
                        {
                            special_food_temp.Add(new MyVector2(row, col_temp));
                        }
                        created_count++;
                    }
                    col_temp++;

                } while (web[row, col_temp] != 1 && web[row, col_temp] != 2);
            }

            return (left_arrow || right_arrow);
        }
        return false;
    }

    public bool isExist(MyVector2 position, List<MyVector2> collections)
    {
        foreach (MyVector2 data in collections)
        {
            if (data.x == position.x && data.y == position.y)
            {
                return true;
            }
        }
        return false;
    }

    public LinkedList<MyVector2> getChildren(int row, int col)
    {
        LinkedList<MyVector2> children = new LinkedList<MyVector2>();
        if (track[row, col - 1] == 0)
        {
            children.AddLast(new MyVector2(row, col - 1));
        }
        if (track[row, col + 1] == 0)
        {
            children.AddLast(new MyVector2(row, col + 1));
        }
        if (track[row - 1, col] == 0)
        {
            children.AddLast(new MyVector2(row - 1, col));
        }
        if (track[row + 1, col] == 0)
        {
            children.AddLast(new MyVector2(row + 1, col));
        }
        return children;
    }

    public int getParentDirection(int childX, int childY, int parentX, int parentY)
    {
        if (childX == parentX)
        {

            // same in row, options are left or right

            if (childY > parentY)
            {
                return LEFT;
            }
            else
            {
                return RIGHT;
            }
        }
        else if (childY == parentY)
        {

            // same in col, options are up or down

            if (childX > parentX)
            {
                return UP;
            }
            else
            {
                return DOWN;
            }
        }
        return UNDEFINED;
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

}

public class MyVector2
{

    public int x, y;

    public MyVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

}