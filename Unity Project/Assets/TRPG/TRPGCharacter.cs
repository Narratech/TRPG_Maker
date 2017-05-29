using System;
using System.Collections;
using System.Collections.Generic;
using IsoUnity;
using IsoUnity.Entities;
using UnityEngine;

public class TRPGCharacter : EntityScript {


    public bool playable = true;
    private bool finishedTurn = false;

    public bool turnFinished()
    {
        return finishedTurn;
    }

    public void finishTurn(bool finished)
    {
        this.finishedTurn = finished;
    }

    public override void eventHappened(IGameEvent ge)
    {
    }

    public override Option[] getOptions()
    {
        return null;
    }


    public override void tick()
    {
    }

    public override void Update()
    {
    }

    // Use this for initialization
    void Start () {
		
	}

        
}
