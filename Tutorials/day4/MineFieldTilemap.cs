using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum TileType
{
    Mine,
    Empty,
    Number,
    NONE
}

public class MineFieldTilemap : TilemapPlus
{
    public Tile[] _SafeTileTypes;
    public Tile _MineTile;
    

    // Start is called before the first frame update
    void Start()
    {
        clearMineFiled();
    }

    public void makeMineField(int mineNumber,Vector3 clickPoint)
    {
        Vector3Int vp_click = _BaseMap.WorldToCell(clickPoint);

        // 初始化
        int[] positions = new int[_Xcount * _YCount];
        for(int i=0; i<positions.Length; i++)
        {
            positions[i] = i;
        }

        Vector3Int vp = vp_click;
        Vector3Int[] safe_pos = getNeighbours(vp, true);

        // 洗牌Fisher–Yates shuffle
        for (int j=0; j<positions.Length-1; j++)
        {
            int tempi = Random.Range(j, positions.Length);
            (positions[j], positions[tempi]) = (positions[tempi],positions[j]);
        }


        // 放雷
        int temp_mine = 0;
        int current_index = 0;
        while (temp_mine < mineNumber)
        {
            int xp, yp;
            xp = positions[current_index] % _Xcount;
            yp = positions[current_index] / _Xcount;
            if (isPosValid(new Vector3Int(xp,yp,0)))
            {
                bool in_safe_point = false;
                for(int i=0; i< safe_pos.Length; i++)
                {
                    if(xp==safe_pos[i].x && yp == safe_pos[i].y)
                    {
                        in_safe_point = true;
                        break;
                    }
                }

                if (!in_safe_point)
                {
                    _BaseMap.SetTile(new Vector3Int(xp, yp, 0), _MineTile);
                    temp_mine++;
                }
            }

            current_index++;
            if (current_index >= positions.Length)
            {
                Debug.LogError("too long!");
                break;
            }
        }

        for(int k=0; k<mineNumber; k++)
        {
            
        }

        // 计算数字标记
        for (int m = 0; m < _Xcount; m++)
        {
            for (int n = 0; n < _YCount; n++)
            {
                vp = new Vector3Int(m, n, 0);
                TileBase tb = _BaseMap.GetTile(vp);
                if (tb != null && tb != _MineTile)
                {
                    Vector3Int[] vps = getNeighbours(vp);
                    int mn = 0;
                    for(int i=0; i<vps.Length; i++)
                    {
                        if (isPosValid(vps[i]))
                        {
                            if (_BaseMap.GetTile(vps[i]) == _MineTile)
                            {
                                mn++;
                            }
                        }
                    }
                    _BaseMap.SetTile(vp, _SafeTileTypes[mn]);
                }
            }
        }
    }

    public TileType getTileTypeByPos(Vector3Int pos)
    {
        TileBase tb = _BaseMap.GetTile(pos);
        if (tb != null)
        {
            if (tb == _MineTile)
            {
                return TileType.Mine;
            }
            else if (tb == _SafeTileTypes[0])
            {
                return TileType.Empty;
            }
            else
            {
                return TileType.Number;
            }
        }

        return TileType.NONE;
    }
}
