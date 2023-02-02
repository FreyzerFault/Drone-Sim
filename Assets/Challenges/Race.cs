using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Race : Challenge
{
    public GameObject startObject;

    public Ring[] rings;
    
    public int ringsTriggered = 0;
    public int NumRings => rings.Length;
    private Ring NextRing => rings[ringsTriggered + 1];

    public TMP_Text ringsText;
    public ArrowPointer arrowToRing;

    public bool orderRingsByHierarchy = true;

    protected override void Awake()
    {
        base.Awake();
        
        rings = GetComponentsInChildren<Ring>();
        
        GameObject arrow = GameObject.FindWithTag("Arrow Hint");
        if (arrow != null)
            arrowToRing = arrow.GetComponent<ArrowPointer>();

        if (levelHUD != null)
            ringsText = levelHUD.GetComponentsInChildren<TMP_Text>()[1];


        if (orderRingsByHierarchy)
        {
            // Los ordena por la jerarquia de Unity
            for (int i = 0; i < rings.Length; i++)
                rings[i].id = i;
        }
        else // los ordena por ID
        {
            IEnumerable<Ring> ringsOrdered = rings.OrderBy<Ring, int>((ring) => ring.id);
            rings = ringsOrdered.ToArray();
        }
    }

    private void Start() => StartChallenge();

    public override void StartChallenge()
    {
        base.StartChallenge();

        InitializeRings();
        PlaceDroneInStart();
        StartTimer();
        ShowTimer();
        UpdateRingsText();
    }

    public override void EndChallenge()
    {
        if (!completed)
        {
            PauseTimer();
            arrowToRing.Hide();

            foreach (Ring ring in rings) ring.gameObject.SetActive(false);
            Message.Instance.Text = "Race ended within " + timer.timeElapsed.ToString("F2") + " seconds!";
            Message.Instance.Open();
        }

        base.EndChallenge();
    }

    private void PlaceDroneInStart() =>
        DroneManager.Instance.currentDroneController.transform.SetPositionAndRotation(
            startObject.transform.position,
            startObject.transform.rotation
        );

    private void InitializeRings()
    {
        // Desactiva todos los anillos menos el PRIMERO
        for (int i = 0; i < rings.Length; i++)
        {
            if (i == 0)
                TurnOnRing(i);
            else
                TurnOffRing(i);
        }


        // Suscribe sus eventos de Trigger (cuando el dron pasa)
        foreach (var ring in rings) ring.SuscribeToTrigger(RingTriggered);
    }
    
    private void RingTriggered(int id)
    {
        ringsTriggered++;
        
        TurnOffRing(id);
        
        if (id == rings.Length - 1)
        {
            // LAST RING
            EndChallenge();
        }
        else
        {
            // Tiene NEXT
            Ring nextRing = rings[id + 1];
            TurnOnRing(id + 1);
        }
        
        UpdateRingsText();
    }

    private void UpdateRingsText()
    {
        if (ringsText != null)
        {
            ringsText.text = ringsTriggered + "/" + rings.Length;
            if (ringsTriggered == rings.Length)
                ringsText.color = Color.yellow;
        }
    }

    private void TurnOffRing(int id)
    {
        Ring ring = rings[id];
        ring.TurnOff();
            
        ring.OnVisible -= HideArrow;
        ring.OnInvisible -= ShowArrow;
    }
    
    private void TurnOnRing(int id)
    {
        Ring ring = rings[id];
        ring.TurnOn();
            
        ring.OnVisible += HideArrow;
        ring.OnInvisible += ShowArrow;
        arrowToRing.target = ring.transform;
    }

    private void ShowArrow()
    {
        if (arrowToRing != null)
            arrowToRing.Show();
    }

    private void HideArrow()
    {
        if (arrowToRing != null)
            arrowToRing.Hide();
    }
}