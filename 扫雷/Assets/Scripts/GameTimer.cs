using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public static GameTimer _Main;
    public Text _ClockText;

    private bool _Run;

    private float _TimeStart;

    private void Awake()
    {
        if (_Main == null)
        {
            _Main = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void startTimer()
    {
        _Run = true;
        _TimeStart = Time.time;
    }

    public void stopTimer()
    {
        _Run = false;
    }

    public void clear()
    {
        _ClockText.text = "" + 0;
    }

    private void Update()
    {
        if (_Run)
        {
            int time = (int)(Time.time - _TimeStart);
            _ClockText.text = "" + time;
        }
    }
}
