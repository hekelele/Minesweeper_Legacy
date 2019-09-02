using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileFliper : MonoBehaviour
{
    private Tilemap _FrontMap;
    public Tile[] _MarkType;
    // 单次调用，清理全部Tile（用于延时标记）
    private bool _ClearAll = false;
    [SerializeField] private int _XCount = 9, _YCount = 9;

    private void Awake()
    {
        _FrontMap = GetComponent<Tilemap>();
    }

    private void Update()
    {
        if (_ClearAll)
        {
            _ClearAll = false;
            _FrontMap.ClearAllTiles();
        }
    }

    public void makeField()
    {
        _ClearAll = false;
        _FrontMap.ClearAllTiles();
        for(int m =0; m<_XCount; m++)
        {
            for(int n=0; n<_YCount; n++)
            {
                _FrontMap.SetTile(new Vector3Int(m, n, 0), _MarkType[0]);
            }
        }
    }

    public void makeField(int x, int y)
    {
        _ClearAll = false;
        _XCount = x;
        _YCount = y;
        _FrontMap.ClearAllTiles();
        for (int m = 0; m < _XCount; m++)
        {
            for (int n = 0; n < _YCount; n++)
            {
                _FrontMap.SetTile(new Vector3Int(m, n, 0), _MarkType[0]);
            }
        }
    }

    public void clickTile(Vector3 pos)
    {
        Vector3Int vp = _FrontMap.WorldToCell(pos);
        TileBase t = _FrontMap.GetTile(vp);
        if (t != null)
        {
            _FrontMap.SetTile(vp, null);
            Vector3Int[] vps = GameFlowManager._Main.getFlipArea(vp);
            for (int i = 0; i < vps.Length; i++)
            {
                _FrontMap.SetTile(vps[i], null);
            }
        }
    }

    public void changeMarking(Vector3 pos)
    {
        Vector3Int vp = _FrontMap.WorldToCell(pos);
        TileBase t = _FrontMap.GetTile(vp);
        if (t != null)
        {
            int type = -1;
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
                _FrontMap.SetTile(vp, _MarkType[type]);
            }
            else
            {
                Debug.LogError("type unknow");
            }
        }
    }

    public void clearAllTilesWhenReady()
    {
        _ClearAll = true;
    }

    public int getTileNum()
    {
        int num = 0;
        for (int m = 0; m < _XCount; m++)
        {
            for (int n = 0; n < _YCount; n++)
            {
                if(_FrontMap.GetTile(new Vector3Int(m, n, 0)) != null)
                {
                    num++;
                }
            }
        }
        return num;
    }
}
