using UnityEngine;
using System.Collections;
using System;
using IsoUnity;

public class WaitForEventFinished : IEnumerator, IEventManager
{
    private IGameEvent waiting;
    private IGameEvent eventFinished;
    private bool finished = false;
    public WaitForEventFinished(IGameEvent gameEvent)
    {
        if(Game.main)
            Game.main.RegisterEventManager(this);

        waiting = gameEvent;
    }

    public WaitForEventFinished(IGameEvent gameEvent, out IGameEvent eventFinished) : this(gameEvent)
    {
        this.eventFinished = eventFinished = new GameEvent();
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

            // eventfinished clone
            if(eventFinished != null)
            {
                eventFinished.Name = ev.Name;
                foreach (var param in ev.Params)
                    eventFinished.setParameter(param, ev.getParameter(param));
            }
        }
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public void Tick(){}
}
