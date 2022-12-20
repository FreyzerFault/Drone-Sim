using UnityEngine;
using System.Collections;

/// <summary>
/// GPS sensor class
/// </summary>
public class GPS : MonoBehaviour
{
    /// <summary>
    /// Gets the coordinates of the drone in a Vector2 object
    /// </summary>
    public Vector2 getCoords() { return new Vector2(noiseX.getNoise(transform.position.x), noiseY.getNoise(transform.position.z)); }

    NoiseAdder noiseX;
    NoiseAdder noiseY;

    /// <summary>
    /// Function Called when the object is activated for the first time
    /// </summary>
    void Awake() { noiseX = new NoiseAdder(); noiseY = new NoiseAdder(); }
     
}
