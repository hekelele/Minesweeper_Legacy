using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MineMaker : MonoBehaviour
{
    private Tilemap _BackMap;
    public Tile _MineTile;
    public Tile[] _MarkingTiles;

    [SerializeField] private int _XCount = 9, _YCount = 9;
    [SerializeField] private int _MineCount = 10;
    

    private void Awake()
    {
        _BackMap = GetComponent<Tilemap>();
    }


    // 通过索引获取坐标
    private Vector2Int getTilePosByIndex(int i)
    {
        if (i >= _XCount * _YCount || i < 0)
        {
            return new Vector2Int(-1, -1);
        }
        else
        {
            int xx, yy;
            xx = i % _XCount;
            yy = i / _XCount;
            return new Vector2Int(xx, yy);
        }
    }


    // 填充指定索引的Tile
    private void setTileByIndex(int i, Tile t)
    {
        Vector2Int pos = getTilePosByIndex(i);
        if(pos.x>=0 && pos.y >= 0)
        {
            _BackMap.SetTile(new Vector3Int(pos.x, pos.y, 0), t);
        }
        else
        {
            Debug.LogError("index out range");
        }
    }

    // 判断指定位置的Tile是否等于这个Tile
    private bool isTileEqualByIndex(int i, Tile t)
    {
        Vector2Int pos = getTilePosByIndex(i);
        if (pos.x >= 0 && pos.y >= 0)
        {
            TileBase bt = _BackMap.GetTile(new Vector3Int(pos.x, pos.y, 0));
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

    // 判断指定位置的Tile是否等于这个Tile
    private bool isTileEqualByPos(int xx, int yy, Tile t)
    {
        if (xx>= 0 && yy >= 0)
        {
            TileBase bt = _BackMap.GetTile(new Vector3Int(xx,yy, 0));
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

    // 随机布雷
    private void makeMineField()
    {
        int mineNumber = _MineCount;
        if (mineNumber > _XCount * _YCount)
        {
            Debug.LogError("too many mine!");
            mineNumber = _XCount * _YCount;
        }

        int[] flags = new int[_XCount * _YCount];
        for(int i=0; i<flags.Length; i++)
        {
            flags[i] = i;
        }

        for(int i=0; i<flags.Length-1; i++)
        {
            int ri = Random.Range(i, flags.Length - 1);
            (flags[i], flags[ri]) = (flags[ri], flags[i]);
        }

        for(int i=0; i<mineNumber; i++)
        {
            setTileByIndex(flags[i],_MineTile);
        }
    }

    // 计算标记
    private void makeMarkingNumbers()
    {
        for(int i=0; i<_XCount*_YCount; i++)
        {
            if (!isTileEqualByIndex(i, _MineTile))
            {
                int mine_number = 0;
                Vector2Int vp = getTilePosByIndex(i);
                mine_number += isTileEqualByPos(vp.x - 1, vp.y, _MineTile) ? 1 : 0;
                mine_number += isTileEqualByPos(vp.x + 1, vp.y, _MineTile) ? 1 : 0;
                mine_number += isTileEqualByPos(vp.x, vp.y - 1, _MineTile) ? 1 : 0;
                mine_number += isTileEqualByPos(vp.x, vp.y + 1, _MineTile) ? 1 : 0;
                mine_number += isTileEqualByPos(vp.x - 1, vp.y - 1, _MineTile) ? 1 : 0;
                mine_number += isTileEqualByPos(vp.x - 1, vp.y + 1, _MineTile) ? 1 : 0;
                mine_number += isTileEqualByPos(vp.x + 1, vp.y - 1, _MineTile) ? 1 : 0;
                mine_number += isTileEqualByPos(vp.x + 1, vp.y + 1, _MineTile) ? 1 : 0;
                setTileByIndex(i, _MarkingTiles[mine_number]);
            }
        }
    }

    private bool isPosValid(Vector3Int vp)
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



    #region Public Area

    public bool isMineByPos(Vector3Int vp)
    {
        return isPosValid(vp) && isTileEqualByPos(vp.x, vp.y, _MineTile);
    }

    public bool isEmptyByPos(Vector3Int vp)
    {
        return isPosValid(vp) && isTileEqualByPos(vp.x, vp.y, _MarkingTiles[0]);
    }

    public void makeMines(int x, int y, int mineNum)
    {
        _XCount = x;
        _YCount = y;
        _MineCount = mineNum;
        _BackMap.ClearAllTiles();
        makeMineField();
        makeMarkingNumbers();
    }

    public void makeMines()
    {
        _BackMap.ClearAllTiles();
        makeMineField();
        makeMarkingNumbers();
    }

    public int getMineNumber()
    {
        return _MineCount;
    }

    #endregion
}
