using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MineMaker : MonoBehaviour
{
    private Tilemap _BackMap;
    [SerializeField] private Tile _MineTile;
    [SerializeField] private Tile[] _MarkingTiles;

    [SerializeField] private int _XCount = 9, _YCount = 9;
    [SerializeField] private int _MineCount = 10;
    

    private void Awake()
    {
        _BackMap = GetComponent<Tilemap>();
        makeMineField(_MineCount);
        makeMarkingNumbers();

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
    private void makeMineField(int mineNumber)
    {
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
}
