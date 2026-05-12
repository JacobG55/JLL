using JLL.Components.Elevator;
using PathfindingLib.API.SmartPathfinding;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JLL.API.Compatability
{
    /*
    public static class PathfindingHelper
    {
        private readonly static Dictionary<JElevatorBody, JElevator> Elevators = [];

        public static void RegisterElevator(JElevatorBody body)
        {
            if (body.Controller == null)
            {
                JLogHelper.LogWarning($"{body.name} tried to register pathfinding with a null elevator controller!");
                return;
            }
            if (!Elevators.TryGetValue(body, out JElevator elevator))
            {
                elevator = new(body);
                Elevators.Add(body, elevator);
            }
            elevator.Register(true);
        }

        public static void UnregisterElevator(JElevatorBody body, bool destroy = false)
        {
            if (Elevators.TryGetValue(body, out JElevator elevator))
            {
                elevator.Register(false);
                if (destroy) Elevators.Remove(body);
            }
        }
        /*
        private class JElevator : IElevator
        {
            public JElevatorBody Body;
            public JElevatorController Controller;
            public readonly ElevatorFloor[] Floors;
            private bool registered = false;

            public JElevator(JElevatorBody body)
            {
                Controller = body.Controller;
                Floors = body.Controller.elevatorFloors.Select((floor) => new ElevatorFloor(this, floor.exitPos)).ToArray();
            }

            public void Register(bool active)
            {
                if (registered == active) return;
                registered = active;
                if (active) foreach (ElevatorFloor floor in Floors)
                {
                    SmartPathfinding.RegisterElevatorFloor(floor);
                }
                else foreach (ElevatorFloor floor in Floors)
                {
                    SmartPathfinding.UnregisterElevatorFloor(floor);
                }
            }

            public Transform InsideButtonNavMeshNode => Controller.interiorNode ?? Controller.elevatorParent;
            public ElevatorFloor ClosestFloor => Floors[Mathf.Clamp(Mathf.RoundToInt(Controller.progress), 0, Floors.Length - 1)];
            public bool DoorsAreOpen => !Controller.inMotion;
            public ElevatorFloor TargetFloor => Floors[Controller.targetFloor];

            public void GoToFloor(ElevatorFloor floor)
            {
                for (int i = 0; i < Floors.Length; i++)
                {
                    if (Floors[i] == floor)
                    {
                        Body.RequestFloorServerRpc(i);
                        return;
                    }
                }
            }

            public float TimeFromFloorToFloor(ElevatorFloor a, ElevatorFloor b)
            {
                if (a == b) return 0f;
                if (Controller.normalizeTravelTime) return Controller.elevatorSpeed;
                float aPos = -1;
                float bPos = -1;
                for (int i = 0; i < Floors.Length; i++)
                {
                    ElevatorFloor floor = Floors[i];
                    if (floor == a) aPos = i;
                    else if (floor == b) bPos = i;
                }
                if (aPos < 0 || bPos < 0) return Controller.elevatorSpeed;
                return Mathf.Abs(aPos - bPos) / Controller.elevatorSpeed;
            }

            public float TimeToCompleteCurrentMovement()
            {
                float spd = Controller.elevatorSpeed;
                if (Controller.normalizeTravelTime) spd *= Mathf.Abs(Controller.prevFloor - Controller.targetFloor);
                return Mathf.Abs(Controller.progress - Controller.targetFloor) / spd;
            }
        }
        /
    }
    */
}