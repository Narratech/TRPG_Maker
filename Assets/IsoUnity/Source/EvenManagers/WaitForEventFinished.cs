using UnityEngine;
using System.Collections;
using System;
using IsoUnity;
using System.Collections.Generic;

public class WaitForEventFinished : IEnumerator, IEventManager
{
    private IGameEvent waiting;
    private Dictionary<string, object> outParams;
    private bool finished = false;
    public WaitForEventFinished(IGameEvent gameEvent)
    {
        if (Game.main)
            Game.main.RegisterEventManager(this);

        waiting = gameEvent;
    }

    public WaitForEventFinished(IGameEvent gameEvent, out Dictionary<string, object> outParams) : this(gameEvent)
    {
        this.outParams = outParams = new Dictionary<string, object>();
    }

    public object Current { get { return null; } }

    public bool MoveNext()
    {
        return !finished;
    }

    public void ReceiveEvent(IGameEvent ev)
    {
        if (ev.Name == "event finished" && ev.getParameter("event") == waiting)
        {
            finished = true;
            if (Game.main)
                Game.main.DeRegisterEventManager(this);

            if (outParams != null)
            {
                foreach (var param in ev.Params)
                    outParams.Add(param, ev.getParameter(param));
            }
        }
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public void Tick() { }
}