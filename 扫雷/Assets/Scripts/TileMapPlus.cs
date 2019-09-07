using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapPlus : MonoBehaviour
{
    protected Tilemap _BaseTilemap;
    protected int _XCount, _YCount;
    public Tile _DefaultTile;

    protected virtual void Awake()
    {
        _BaseTilemap = GetComponent<Tilemap>();
    }

    public virtual void reset(int xx, int yy)
    {
        _XCount = xx;
        _YCount = yy;
        _BaseTilemap.ClearAllTiles();
    }

    // 通过索引获取坐标
    public Vector3Int getTilePosByIndex(int i)
    {
        if (i >= _XCount * _YCount || i < 0)
        {
            return new Vector3Int(-1, -1, -1);
        }
        else
        {
            int xx, yy;
            xx = i % _XCount;
            yy = i / _XCount;
            return new Vector3Int(xx, yy, 0);
        }
    }

    // 通过坐标获取索引
    public int getTileIndexByPos(Vector3Int vp)
    {
        if (isPosValid(vp))
        {
            return vp.y * _XCount + vp.x;
        }
        else
        {
            return -1;
        }
    }

   

    // 判断指定位置的Tile是否等于这个Tile
    public bool isTileEqualByPos(Vector3Int vp, Tile t)
    {
        if (vp.x >= 0 && vp.y >= 0)
        {
            TileBase bt = _BaseTilemap.GetTile(vp);
            if (bt == null)
            {
                return false;
            }
            if (bt == t)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    // 判断指定索引的Tile是否等于这个Tile
    public bool isTileEqualByIndex(int i, Tile t)
    {
        Vector3Int pos = getTilePosByIndex(i);
        return isTileEqualByPos(pos, t);
    }

    // 判读指定位置是否包含在Tilemap范围之中
    public bool isPosInCells(Vector3 pos)
    {
        Vector3Int vp = _BaseTilemap.WorldToCell(pos);
        if (_BaseTilemap.GetTile(vp) != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //判断是否出框
    public bool isPosValid(Vector3Int vp)
    {
        if (vp.x < 0 || vp.y < 0 || vp.x >= _XCount || vp.y >= _YCount)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // 得到所有在范围内的领格（坐标形式）
    public Vector3Int[] getAllValidNeighbour(Vector3Int basePos)
    {
        List<Vector3Int> neighbours = new List<Vector3Int>();
        if (isPosValid(basePos))
        {
            // 周围八个邻居
            Vector3Int[] temp = new Vector3Int[8] {
                        new Vector3Int(basePos.x+1,basePos.y,0), new Vector3Int(basePos.x-1, basePos.y, 0),
                        new Vector3Int(basePos.x, basePos.y+1, 0), new Vector3Int(basePos.x, basePos.y-1, 0),
                        new Vector3Int(basePos.x-1,basePos.y-1,0), new Vector3Int(basePos.x-1, basePos.y+1, 0),
                        new Vector3Int(basePos.x+1, basePos.y-1, 0), new Vector3Int(basePos.x+1, basePos.y+1, 0)};
            // 只保留范围内格点
            for (int i=0; i<temp.Length; i++)
            {
                if (isPosValid(temp[i]))
                {
                    neighbours.Add(temp[i]);
                }
            }
        }

        return neighbours.ToArray();
    }

    // 得到所有在范围内的领格（索引形式）
    public int[] getAllValidNeighbourIndex(Vector3Int basePos)
    {
        Vector3Int[] neighbours = getAllValidNeighbour(basePos);
        List<int> neighbourIndexes = new List<int>();

        for (int i = 0; i < neighbours.Length; i++)
        {
            if (isPosValid(neighbours[i]))
            {
                neighbourIndexes.Add(getTileIndexByPos(neighbours[i]));
            }
        }
        return neighbourIndexes.ToArray();
    }

    // 统计范围内有多少个Tile
    public int getTileNum()
    {
        int num = 0;
        for (int m = 0; m < _XCount; m++)
        {
            for (int n = 0; n < _YCount; n++)
            {
                if (_BaseTilemap.GetTile(new Vector3Int(m, n, 0)) != null)
                {
                    num++;
                }
            }
        }
        return num;
    }

    // 转换格点坐标到世界坐标
    public Vector3 CellToWorld(Vector3Int vpi)
    {
        return _BaseTilemap.CellToWorld(vpi);
    }

    // 转换世界坐标到格点
    public Vector3Int WorldToCell(Vector3 vp)
    {
        return _BaseTilemap.WorldToCell(vp);
    }

    // 与Cell网格对齐
    public Vector3 getPosAlignWithCell(Vector3 vp)
    {
        Vector3Int vpi = _BaseTilemap.WorldToCell(vp);
        if (isPosValid(vpi))
        {
            return CellToWorld(vpi);
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    // Tilemap全部清空
    public void clearAllTiles()
    {
        _BaseTilemap.ClearAllTiles();
    }

    // Tilemap全部填充
    public void fillUp(Tile tb)
    {
        _BaseTilemap.ClearAllTiles();
        for (int i = 0; i < _XCount; i++)
        {
            for (int j = 0; j < _YCount; j++)
            {
                _BaseTilemap.SetTile(new Vector3Int(i, j, 0), tb);
            }
        }
    }

    // 填充指定索引的Tile
    public void setTileByIndex(int i, Tile t)
    {
        Vector3Int pos = getTilePosByIndex(i);
        if (isPosValid(pos))
        {
            _BaseTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), t);
        }
        else
        {
            Debug.LogError("index out range");
        }
    }

    // 填充指定位置的Tile
    public void setTileByPos(Vector3Int vp, Tile t)
    {
        if (isPosValid(vp))
        {
            _BaseTilemap.SetTile(new Vector3Int(vp.x, vp.y, 0), t);
        }
        else
        {
            Debug.LogError("pos out range " + vp);
        }
    }

    // 填充指定位置的Tile 无检查版本
    public void setTileByPosUncheck(Vector3Int vp, Tile t)
    {
        if (isPosValid(vp))
        {
            _BaseTilemap.SetTile(new Vector3Int(vp.x, vp.y, 0), t);
        }
    }

}

