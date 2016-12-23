﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRenderer : MonoBehaviour
{

    [Header("Width of beam"), Range(.01f, 1f)]
    public float width = .05f;

    // Start and end points of the laser line
    private Vector2 start;
    private Vector2 end;
    private LineRenderer lr;
    // This transform
    private Transform tr;
    // The two laser sources' transform
    private Transform source1tr;
    private Transform source2tr;
    // Cooldown
    private float lastUse = 0f;
    public float cooldown = 0.1f;
    // Armonic on/off
    [Header("Interval on/off, set to 0 for always on")]
    public float m_sec_offset = 0f;
    private float switchT;

    // Use this for initialization
    void Start()
    {
        lr = gameObject.GetComponent<LineRenderer>() as LineRenderer;
        lr.SetColors(Color.red, Color.red);
        lr.startWidth = width;
        lr.endWidth = width;
        tr = gameObject.GetComponent<Transform>() as Transform;
        Transform parenttr = tr.parent;
        source1tr = parenttr.GetChild(0);
        source2tr = parenttr.GetChild(1);
        start = new Vector2(source1tr.position.x, source1tr.position.y);
        end = new Vector2(source2tr.position.x, source2tr.position.y);
        lr.SetPositions(new Vector3[] { start, end });
        lr.enabled = true;
        if(m_sec_offset != 0) switchT = Time.time + m_sec_offset;
    }

    void Update()
    {
        LaserInteraction();
    }

    void FixedUpdate()
    {
        start = new Vector2(source1tr.position.x, source1tr.position.y);
        end = new Vector2(source2tr.position.x, source2tr.position.y);
        lr.SetPositions(new Vector3[] { start, end });
        if(m_sec_offset != 0 && Time.time > switchT)
        {
            lr.enabled = !lr.enabled;
            switchT = Time.time + m_sec_offset;
        }
    }

    // Laser kills particles here
    void LaserInteraction()
    {
        RaycastHit2D[] hits = Physics2D.LinecastAll(start, end);

        foreach (RaycastHit2D hit in hits)
        {
            GameObject elem = hit.collider.gameObject;
            if (elem.tag == "Player")
            {
                if(Time.time - lastUse > cooldown)
                {
                    GameManager.Instance.m_Player.GetComponent<PlayerAvatar_02>().Deactivate_Particle(elem);
                    lastUse = Time.time;
                }
            }
        }
    }
}
