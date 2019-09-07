using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileFliper : TileMapPlus
{
    public Tile[] _MarkType;

    // 重写的初始化
    public override void reset(int xx, int yy)
    {
        base.reset(xx, yy);
        fillUp(_MarkType[0]);
    }
    
    // 点击Tile
    public bool clickTile(Vector3 pos)
    {
        Vector3Int vp = _BaseTilemap.WorldToCell(pos);
        TileBase t = _BaseTilemap.GetTile(vp);
        // 只有是空地的时候可以点击
        if (t==_MarkType[0])
        {
            _BaseTilemap.SetTile(vp, null);
            return true;
        }
        return false;
    }

    // 改变Tile标记
    public void changeMarking(Vector3 pos)
    {
        Vector3Int vp = _BaseTilemap.WorldToCell(pos);
        TileBase t = _BaseTilemap.GetTile(vp);
        if (t != null)
        {
            int type = -1;
            // 几种标记循环改变
            for (int i = 0; i < _MarkType.Length; i++)
            {
                if (_MarkType[i] == t)
                {
                    type = i;
                    break;
                }
            }
            if (type >= 0)
            {
                type = (type + 1) % _MarkType.Length;
                _BaseTilemap.SetTile(vp, _MarkType[type]);
            }
            else
            {
                Debug.LogError("type unknow");
            }
        }
    }

    
    // 得到地雷标记数量
    public int getMineMarkNum()
    {
        int c = 0;
        for(int i=0; i<_XCount; i++)
        {
            for(int j=0; j<_YCount; j++)
            {
                c += isTileEqualByPos(new Vector3Int(i, j, 0), _MarkType[1]) ? 1 : 0;
            }
        }

        return c;
    }
    
    // 得到所有标记位置
    public Vector3Int[] getAllMarkedPos()
    {
        List<Vector3Int> vpis = new List<Vector3Int>();
        for(int i=0; i<_XCount; i++)
        {
            for(int j=0; j<_YCount; j++)
            {
                TileBase t = _BaseTilemap.GetTile(new Vector3Int(i, j, 0));
                if (t == _MarkType[1])
                {
                    vpis.Add(new Vector3Int(i,j,0));
                }
            }
        }
        return vpis.ToArray();
    }
}
