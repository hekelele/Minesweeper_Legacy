﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public TileFliper _TF = null;
    public MineMaker _MM = null;
    public AudioSource _SE;

    public AudioClip _ExplodeSE;
    public AudioClip _WinSE;

    public static GameFlowManager _Main = null;
    public enum GameState
    {
        WAIT,
        RUNNING,
        END,
        WIN
    }

    private GameState _State = GameState.WAIT;

    private void Awake()
    {
        if (_Main != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _Main = this;
        }
    }

    private void Start()
    {
        resetGame();
    }

    private void Update()
    {
        if (_State == GameState.RUNNING)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _TF.clickTile(pos);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _TF.changeMarking(pos);
            }
            checkWinState();
        }
    }

    private void checkWinState()
    {
        if (_TF.getTileNum() == _MM.getMineNumber())
        {
            winGame();
        }
    }

    
    // 获取翻开区域
    public Vector3Int[] getFlipArea(Vector3Int clickPos)
    {
        List<Vector3Int> vps = new List<Vector3Int>();
        List<Vector3Int> waitList = new List<Vector3Int>();
        List<Vector3Int> checkedList = new List<Vector3Int>();
        
        if (!_MM.isMineByPos(clickPos))
        {
            if (_MM.isEmptyByPos(clickPos))
            {
                waitList.Add(clickPos);
                int loopNum = 0;
                while (waitList.Count > 0 || loopNum > 5000)
                {
                    loopNum++;
                    vps.Add(waitList[0]);
                    Vector3Int[] neighbours = new Vector3Int[4] {
                        new Vector3Int(waitList[0].x+1,waitList[0].y,0), new Vector3Int(waitList[0].x-1, waitList[0].y, 0),
                        new Vector3Int(waitList[0].x, waitList[0].y+1, 0), new Vector3Int(waitList[0].x, waitList[0].y-1, 0) };
                    for(int i=0; i<neighbours.Length; i++)
                    {
                        if (!checkedList.Contains(neighbours[i]))
                        {
                            checkedList.Add(neighbours[i]);
                            if (_MM.isEmptyByPos(neighbours[i]))
                            {
                                waitList.Add(neighbours[i]);
                                vps.Add(neighbours[i]);
                            }
                            else if (!_MM.isMineByPos(neighbours[i]))
                            {
                                vps.Add(neighbours[i]);
                            }
                        }
                    }
                    waitList.RemoveAt(0);
                }
            }
        }
        else
        {
            endGame();
        }

        return vps.ToArray();
    }

    // 游戏胜利
    public void winGame()
    {
        _State = GameState.WIN;
        _TF.clearAllTilesWhenReady();
        _SE.clip = _WinSE;
        _SE.Play();
    }


    // 结束游戏
    public void endGame()
    {
        _State = GameState.END;
        _TF.clearAllTilesWhenReady();
        _SE.clip = _ExplodeSE;
        _SE.Play();
    }

    // 重新开始游戏
    public void resetGame()
    {
        _MM.makeMines();
        _TF.makeField();
        _State = GameState.RUNNING;
    }
}
