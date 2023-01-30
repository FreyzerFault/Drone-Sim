using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Race : Challenge
{
    public GameObject startObject;
    public TriggerEvent endTrigger;

    public Ring[] rings;

    public bool orderRingsByHierarchy = true;
    public bool appearRingsInOrder = true;

    protected override void Awake()
    {
        base.Awake();
        endTrigger.OnTrigger += EndChallenge;

        rings = GetComponentsInChildren<Ring>();
        
        if (orderRingsByHierarchy)
        {
            for (int i = 0; i < rings.Length; i++)
                rings[i].id = i;
        }
        else // los ordena por ID
        {
            IEnumerable<Ring> ringsOrdered = rings.OrderBy<Ring, int>((ring) => ring.id);
            rings = ringsOrdered.ToArray();
        }
        
        if (appearRingsInOrder)
            for (int i = 1; i < rings.Length; i++)
            {
                rings[i].gameObject.SetActive(false);
            }


        foreach (var ring in rings)
        {
            ring.SuscribeToTrigger(id => Debug.Log("Ring " + id + " traspasao"));
            ring.SuscribeToTrigger(id =>
            {
                if (id < rings.Length - 1)
                    rings[id + 1].gameObject.SetActive(true);
                else
                    EndChallenge();
            });
        }
    }

    private void Start() => StartChallenge();

    public override void StartChallenge()
    {
        base.StartChallenge();

        PlaceDroneInStart();
        StartTimer();
        ShowTimer();
    }

    public override void EndChallenge()
    {
        if (!completed) PauseTimer();

        base.EndChallenge();
    }

    private void PlaceDroneInStart() =>
        DroneManager.Instance.currentDroneController.transform.SetPositionAndRotation(
            startObject.transform.position,
            startObject.transform.rotation
        );
}