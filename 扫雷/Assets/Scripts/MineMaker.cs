using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MineMaker : MonoBehaviour
{
    private Tilemap     _BackMap;
    public Tile         _MineTile;
    public Tile[]       _MarkingTiles;

    public enum MineSpreadType
    {
        NONE,
        SAFE,
        CLEAR
    }

    public MineSpreadType _MineType = MineSpreadType.NONE;

    private int _XCount, _YCount;


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

    // 通过坐标获取索引
    private int getTileIndexByPos(Vector3Int vp)
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

    public void emptyMineField(int xx, int yy)
    {
        _BackMap.ClearAllTiles();
        _XCount = xx;
        _YCount = yy;
        for(int i=0; i<xx; i++)
        {
            for(int j=0; j<yy; j++)
            {
                _BackMap.SetTile(new Vector3Int(i, j, 0), _MarkingTiles[0]);
            }
        }
    }

    // 随机布雷
    private void makeMineField(Vector3Int clickTilePos, int xx, int yy, int mm)
    {
        _XCount = xx;
        _YCount = yy;
        int mineNumber = mm;
        int maxMineNumber;
        List<int> safePos = new List<int>();
        switch (_MineType)
        {
            case MineSpreadType.NONE:
                maxMineNumber = _XCount * _YCount;
                break;
            case MineSpreadType.SAFE:
                maxMineNumber = _XCount * _YCount - 1;
                int id =getTileIndexByPos(clickTilePos);
                if (id > 0)
                {
                    safePos.Add(id);
                }
                break;
            case MineSpreadType.CLEAR:
            default:
                maxMineNumber = _XCount * _YCount - 9;
                for(int m=-1; m<2; m++)
                {
                    for(int n=-1; n<2; n++)
                    {
                        int id2 = getTileIndexByPos(new Vector3Int(clickTilePos.x + m, clickTilePos.y + n, 0));
                        if (id2 > 0)
                        {
                            safePos.Add(id2);
                        }
                    }
                }
                break;
        }

        if (mineNumber > maxMineNumber)
        {
            Debug.LogError("too many mine!");
            mineNumber = maxMineNumber;
        }

        int[] flags = new int[_XCount * _YCount];
        
        for(int i=0; i<flags.Length; i++)
        {
            if (!safePos.Contains(i))
            {
                flags[i] = i;
            }
            else
            {
                flags[i] = -1;
            }
        }

        for(int i=0; i<flags.Length-1; i++)
        {
            int ri = Random.Range(i, flags.Length - 1);
            (flags[i], flags[ri]) = (flags[ri], flags[i]);
        }

        int mines = 0;
        for (int i = 0; i < mineNumber + (_XCount * _YCount - maxMineNumber); i++)
        {
            int pos = flags[i];
            if (pos >= 0)
            {
                setTileByIndex(pos, _MineTile);
                mines++;
            }
            if (mines >= mineNumber)
            {
                break;
            }
        }
    }

    // 计算标记
    private void makeMarkingNumbers()
    {
        for (int i=0; i<_XCount*_YCount; i++)
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
    

    public void makeMines(Vector3 vp, int xx, int yy, int mm)
    {
        _BackMap.ClearAllTiles();
        makeMineField(_BackMap.WorldToCell(vp), xx, yy, mm);
        makeMarkingNumbers();
    }

    public bool isPosInCells(Vector3 pos)
    {
        Vector3Int vp = _BackMap.WorldToCell(pos);
        if (_BackMap.GetTile(vp) != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    

    #endregion
}
