using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MineMaker : TileMapPlus
{
    // Tile图形
    public Tile         _MineTile;
    public Tile[]       _MarkingTiles;

    // reset重写
    public override void reset(int xx, int yy)
    {
        base.reset(xx, yy);
        fillUp(_MarkingTiles[0]);
    }


    // 随机布雷
    private void makeMineField(Vector3Int clickTilePos, int mineNumber)
    {
        int maxMineNumber;
        maxMineNumber = _XCount * _YCount - 9;
        
        List<int> safePos = new List<int>(getAllValidNeighbourIndex(clickTilePos));
        safePos.Add(getTileIndexByPos(clickTilePos));

        if (mineNumber > maxMineNumber)
        {
            Debug.LogError("too many mine!");
            mineNumber = maxMineNumber;
        }

        // 算法解释见文档
        int[] flags = new int[_XCount * _YCount];

        for (int i = 0; i < flags.Length; i++)
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

        for (int i = 0; i < flags.Length - 1; i++)
        {
            int ri = Random.Range(i, flags.Length - 1);
            (flags[i], flags[ri]) = (flags[ri], flags[i]);
        }

        int mines = 0;
        for (int i = 0; i < _XCount * _YCount; i++)
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
        // 对所有tile遍历
        for (int i=0; i<_XCount*_YCount; i++)
        {
            //如果不是地雷，那么计算提示数字
            if (!isTileEqualByIndex(i, _MineTile))
            {
                //获得邻居坐标
                Vector3Int vp = getTilePosByIndex(i);
                Vector3Int[] nbs = getAllValidNeighbour(vp);
                int mine_number = 0;
                for (int k = 0; k < nbs.Length; k++)
                {
                    mine_number += isTileEqualByPos(nbs[k], _MineTile) ? 1 : 0;     //计算地雷个数
                }
                setTileByIndex(i, _MarkingTiles[mine_number]);                      //更加个数改变提示标记图案
            }
        }
    }
    

    // 查看对应坐标是否有雷
    public bool isMineByPos(Vector3Int vp)
    {
        return isPosValid(vp) && isTileEqualByPos(vp, _MineTile);
    }

    // 查看对应坐标是否为空
    public bool isEmptyByPos(Vector3Int vp)
    {
        return isPosValid(vp) && isTileEqualByPos(vp, _MarkingTiles[0]);
    }
    
    // 布雷
    public void makeMines(Vector3 vp, int xx, int yy, int mm)
    {
        reset(xx, yy);
        makeMineField(_BaseTilemap.WorldToCell(vp),mm);     // 布雷
        makeMarkingNumbers();                               // 计算提示数字
    }

    // 获得所有地雷的世界坐标
    public Vector3[] getAllMineWorldPos()
    {
        List<Vector3> vps = new List<Vector3>();
        for (int m = 0; m < _XCount; m++)
        {
            for (int n = 0; n < _YCount; n++)
            {
                if(isMineByPos(new Vector3Int(m, n, 0)))
                {
                    vps.Add(_BaseTilemap.CellToWorld(new Vector3Int(m, n, 0)));
                }
            }
        }
        return vps.ToArray();
    }
}
