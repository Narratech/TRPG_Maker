using System.Collections;
using System.Collections.Generic;
using IsoUnity;
using UnityEngine;

public class CellSelectionArrow: IsoUnity.Entities.EntityScript
{



    public override void eventHappened(IsoUnity.IGameEvent ge)
    {
        // If we're waiting for the event finished
        if (movement != null)
            if (ge.Name.ToLower() == "event finished")
                // Let's check it's our event:
                if (ge.getParameter("event") == movement)
                {
                    // Ok it's done
                    movement = null;
                }
    }

    private IsoUnity.GameEvent movement;
    private IsoUnity.Cell actionCell;
    private IsoUnity.Entities.Entity actionEntity;
    private IsoUnity.IGameEvent toLaunch;
    private bool movingArrow;

    [SerializeField]
    private bool active = true;
    public bool Active
    {
        get
        {
            return active;
        }
        set
        {
            this.active = value;
        }
    }

    public void onControllerEvent(IsoUnity.ControllerEventArgs args)
    {
        // # Avoid responding controller event when inactive
        if (!active)
            return;

        // # Normal threatment
        // Multiple controller events only give one launch result per tick
        if (toLaunch == null)
        {

            if (args.cell != null)
            {
                var cell = args.cell;
                if (args.cell.VirtualNeighbors.Count > 0)
                {
                    cell = args.cell.VirtualNeighbors[0].Destination;
                }


                
                GameEvent ge = new GameEvent();
                ge.setParameter("mover", this.Entity.mover);
                ge.setParameter("cell", cell);
                ge.Name = "teleport";
                Game.main.enqueueEvent(ge);
               

            }
            // Otherwise, the controller event should contain keys pressed
            else
            {

                int to = -1;
                if (args.LEFT) { to = 0; }
                else if (args.UP) { to = 1; }
                else if (args.RIGHT) { to = 2; }
                else if (args.DOWN) { to = 3; }

                if (to > -1)
                {
                    if (movement == null || !movingArrow)
                    {
                        if (Entity == null)
                            Debug.Log("Null!");
                        IsoUnity.Cell destination = Entity.Position.Map.getNeightbours(Entity.Position)[to];
                        // Can move to checks if the entity can DIRECT move to this cells.
                        // This should solve bug #29
                        IsoUnity.Entities.Mover em = this.Entity.GetComponent<IsoUnity.Entities.Mover>();
                        if (em != null && em.CanMoveTo(destination))
                        {
                            IsoUnity.GameEvent ge = new IsoUnity.GameEvent();
                            ge.setParameter("mover", this.Entity.mover);
                            ge.setParameter("cell", destination);
                            ge.setParameter("synchronous", true);
                            ge.Name = "move";
                            IsoUnity.Game.main.enqueueEvent(ge);
                            movement = ge;
                        }
                        else
                        {
                            IsoUnity.GameEvent ge = new IsoUnity.GameEvent();
                            ge.setParameter("mover", this.Entity.mover);
                            ge.setParameter("direction", fromIndex(to));
                            ge.setParameter("synchronous", true);
                            ge.Name = "turn";
                            IsoUnity.Game.main.enqueueEvent(ge);
                            movement = ge;
                        }
                        movingArrow = true;
                    }
                }
            }
        }
    }

    private IsoUnity.Entities.Mover.Direction fromIndex(int i)
    {
        switch (i)
        {
            case 0: return IsoUnity.Entities.Mover.Direction.North;
            case 1: return IsoUnity.Entities.Mover.Direction.East;
            case 2: return IsoUnity.Entities.Mover.Direction.South;
            case 3: return IsoUnity.Entities.Mover.Direction.West;
        }

        return IsoUnity.Entities.Mover.Direction.North;
    }

    private bool registered = false;

    public override void tick()
    {

        if (toLaunch != null)
        {
            // If we're not waiting to receive event finished from Mover.
            if (movement == null)
            {
                // That means that, we're in the right position so...
                IsoUnity.Game.main.enqueueEvent(toLaunch);
                toLaunch = null;

                // Look to the action
                if (actionCell || actionEntity)
                {
                    IsoUnity.Entities.Mover.Direction toLook = IsoUnity.Entities.Mover.getDirectionFromTo(Entity.transform, actionEntity ? actionEntity.transform : actionCell.transform);
                    IsoUnity.GameEvent ge = new IsoUnity.GameEvent("turn");
                    ge.setParameter("mover", Entity.mover);
                    ge.setParameter("direction", toLook);
                    IsoUnity.Game.main.enqueueEvent(ge);
                }

                actionCell = null;
                actionEntity = null;
            }
        }

        if (!registered)
        {
            IsoUnity.ControllerManager.onControllerEvent += this.onControllerEvent;
            registered = true;
        }

    }

    public override IsoUnity.Option[] getOptions()
    {
        return new IsoUnity.Option[] { };
    }

    public override void Update()
    {

    }

    void OnDestroy()
    {
        IsoUnity.ControllerManager.onControllerEvent -= this.onControllerEvent;
    }


    }

