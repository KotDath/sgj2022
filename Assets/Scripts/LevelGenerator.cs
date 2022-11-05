using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using sys = System;

public enum TileType
{
    Empty = 0,    // 000000
    Wall = 1,    // 000001
    Entry = 2,    // 000010
    Exit = 4,    // 000100
    Key = 8,     // 001000
    Corridor = 16     // 001000
}

public class Pair<T, U>
{
    public Pair()
    {
    }

    public Pair(T first, U second)
    {
        this.First = first;
        this.Second = second;
    }

    public T First { get; set; }
    public U Second { get; set; }
};


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LevelGenerator : MonoBehaviour
{
    [Header("Тайлы")]
    [Label("стенка"), SerializeField] GameObject wall;
    [Label("вход"), SerializeField] GameObject entry;
    [Label("выход"), SerializeField] GameObject exit;
    [Label("комната с ключом"), SerializeField] GameObject keyRoom;
    [Label("коридор"), SerializeField] GameObject corridor;

    [Header("Характеристики лабиринта")]
    [Label("ширина в тайлах"), SerializeField] int width;
    [Label("высота в тайлах"), SerializeField] int height;

    [Label("ширина тайла"), SerializeField] int tileWidth;
    [Label("высота тайла"), SerializeField] int tileHeight;

    [Label("Число итераций"), SerializeField] int iterCount;

    [Label("Игрок"), SerializeField] GameObject player;

    TileType[,] tiles;

    void Awake()
    {

        var Floor = Instantiate(corridor, new Vector3(width * tileWidth / 2, 0, height * tileHeight / 2), Quaternion.identity);
        Floor.transform.localScale = Vector3.Scale(Floor.transform.localScale, new Vector3(width * tileWidth, 1, height * tileHeight));
        tiles = new TileType[width, height];
        
        for (int i = 1; i < width - 1; ++i)
        {
            for (int j = 1; j < height - 1; ++j)
            {
                var value = TileType.Empty;
                if (Random.Range(0f, 1f) > 0.8f)
                {
                    value = TileType.Corridor;
                } else
                {
                    value = TileType.Wall;
                }

                tiles[i, j] = value;

            }
        }

        for (int i = 1; i < width - 1; ++i)
        {
            for (int j = 1; j < height - 1; ++j)
            {
                var count = 0;
                if (tiles[i + 1, j] == TileType.Corridor)
                {
                    ++count;
                }
                if (tiles[i - 1, j] == TileType.Corridor)
                {
                    ++count;
                }
                if (tiles[i, j + 1] == TileType.Corridor)
                {
                    ++count;
                }
                if (tiles[i, j - 1] == TileType.Corridor)
                {
                    ++count;
                }

                if (count >= 2)
                {
                    continue;
                }

                int way = Random.Range(0, 2);
                if (way == 0)
                {
                    tiles[i + 1, j] = TileType.Corridor;
                } else
                {
                    tiles[i, j + 1] = TileType.Corridor;
                }
            }
        }

        for (int i = 0; i < width; ++i)
        {
            tiles[i, 0] = TileType.Wall;
            tiles[i, height - 1] = TileType.Wall;
        }
        for (int i = 0; i < height; ++i)
        {
            tiles[0, i] = TileType.Wall;
            tiles[width - 1, i] = TileType.Wall;
        }

        for (int i = 1; i < width - 1; ++i)
        {
            for (int j = 1; j < height - 1; ++j)
            {
                if (tiles[i, j] == TileType.Empty)
                {
                    tiles[i, j] = TileType.Wall;
                }

            }
        }

        for (int i = 1; i < (width - 1) / 5; ++i)
        {
            for (int j = 1; j < (height - 1) / 5; ++j)
            {
                if (Random.Range(0f, 1f) > 0.7f)
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        for (int l = 0; l < 4; ++l)
                        {
                            tiles[i * 5 + k, j * 5 + l] = TileType.Corridor;
                        }
                    }
                }

            }
        }

        for (int i = 1; i < width - 1; ++i)
        {
            for (int j = 1; j < height - 1; ++j)
            {
                var count = 0;
                for (int k = 0; k < 9; ++k)
                {
                    if (tiles[i + k / 3 - 1, j + k % 3 - 1] != TileType.Wall && k != 4)
                    {
                        ++count;
                    }
                }

                if (count >= 7)
                {
                    tiles[i, j] = TileType.Corridor;
                }

            }
        }


        tiles[1, 1] = TileType.Entry;

        int[,] alreadyVisit = new int[width, height];
        Queue<Pair<int, int>> cells = new Queue<Pair<int, int>>();
        if (tiles[2, 1] == TileType.Corridor)
        {
            cells.Enqueue(new Pair<int, int>(2, 1));
            alreadyVisit[2, 1] = 1;
        }

        if (tiles[1, 2] == TileType.Corridor)
        {
            cells.Enqueue(new Pair<int, int>(1, 2));
            alreadyVisit[1, 2] = 1;
        }


        for (int i = 1; i < iterCount && cells.Count >= 1; ++i)
        {
            var elem = cells.Dequeue();
            alreadyVisit[elem.First, elem.Second] = 2;
            if (tiles[elem.First + 1, elem.Second] == TileType.Corridor && alreadyVisit[elem.First + 1, elem.Second] == 0)
            {
                cells.Enqueue(new Pair<int, int>(elem.First + 1, elem.Second));
                alreadyVisit[elem.First + 1, elem.Second] = 1;
            }

            if (tiles[elem.First, elem.Second + 1] == TileType.Corridor && alreadyVisit[elem.First, elem.Second + 1] == 0)
            {
                cells.Enqueue(new Pair<int, int>(elem.First, elem.Second + 1));
                alreadyVisit[elem.First, elem.Second + 1] = 1;
            }

            if (tiles[elem.First - 1, elem.Second] == TileType.Corridor && alreadyVisit[elem.First - 1, elem.Second] == 0)
            {
                cells.Enqueue(new Pair<int, int>(elem.First - 1, elem.Second));
                alreadyVisit[elem.First - 1, elem.Second] = 1;
            }

            if (tiles[elem.First, elem.Second - 1] == TileType.Corridor && alreadyVisit[elem.First, elem.Second - 1] == 0)
            {
                cells.Enqueue(new Pair<int, int>(elem.First, elem.Second - 1));
                alreadyVisit[elem.First, elem.Second - 1] = 1;
            }
        }

        var keyPosition = cells.Dequeue();
        tiles[keyPosition.First, keyPosition.Second] = TileType.Key;

        var queueCount = cells.Count;
        while(cells.Count > queueCount / 2)
        {
            cells.Dequeue();
        }

        var exitPosition = cells.Dequeue();
        tiles[exitPosition.First, exitPosition.Second] = TileType.Exit;

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                
                var tile = tiles[i, j];
                if (tile == TileType.Entry)
                {
                    var position = entry.transform.position + new Vector3(i * tileWidth, 0, j * tileHeight);
                    Instantiate(entry, position, Quaternion.identity);
                }

                if (tile == TileType.Exit)
                {
                    var position = exit.transform.position + new Vector3(i * tileWidth, 0, j * tileHeight);
                    Instantiate(exit, position, Quaternion.identity);
                }

                if (tile == TileType.Corridor)
                {
                    var position = corridor.transform.position + new Vector3(i * tileWidth, 0, j * tileHeight);
                    Instantiate(corridor, position, Quaternion.identity);
                }

                if (tile == TileType.Wall)
                {
                    var position = wall.transform.position + new Vector3(i * tileWidth, 0, j * tileHeight);
                    Instantiate(wall, position, Quaternion.identity);
                }

                if (tile == TileType.Key)
                {
                    var position = keyRoom.transform.position + new Vector3(i * tileWidth + (float)tileWidth, 0, j * tileHeight + (float)tileHeight);
                    Instantiate(keyRoom, position, Quaternion.identity);
                }


            }
        }

        player.transform.position = new Vector3((float)tileWidth * 1.5f, 1, (float)tileHeight * 1.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
