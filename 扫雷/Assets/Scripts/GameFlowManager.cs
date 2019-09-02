using System.Collections;
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
            waitList.Add(clickPos);
            checkedList.Add(clickPos);
            int looptime = 0;
            while (waitList.Count > 0 && looptime < 10000)
            {
                looptime++;
                if (!_MM.isMineByPos(waitList[0]))
                {
                    if (_MM.isEmptyByPos(waitList[0]))
                    {
                        for(int i=0; i<9; i++)
                        {
                            if (i == 4)
                            {
                                continue;
                            }
                            else
                            {
                                int m = i / 3 - 1;
                                int n = i % 3 - 1;
                                Vector3Int temp;
                                temp = new Vector3Int(waitList[0].x + m, waitList[0].y + n, 0);
                                if (!checkedList.Contains(temp))
                                {
                                    waitList.Add(temp);
                                    checkedList.Add(temp);
                                }
                            }
                        }

                        if (!vps.Contains(waitList[0]))
                        {
                            vps.Add(waitList[0]);
                        }
                    }
                    else
                    {
                        if (!vps.Contains(waitList[0]))
                        {
                            vps.Add(waitList[0]);
                        }
                    }
                }

                waitList.RemoveAt(0);
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
