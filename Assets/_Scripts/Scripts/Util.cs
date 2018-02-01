using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// An event that is triggered when an AI should investigate a point.
// This event has a single Vector3 argument, which represents the point to investigate.
[System.Serializable]
public class InvestigatePointEvent : UnityEvent<Vector3> {}
