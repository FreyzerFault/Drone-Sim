using UnityEngine;
using System.Collections;

public class noiseAddVector3 {

    NoiseAdder xN;
    NoiseAdder yN;
    NoiseAdder zN;
     
    /// <summary>
    /// Constructor of the class
    /// <para>Just create a noiseAdder class for each element of the vector</para>
    /// </summary>
    public noiseAddVector3()
    {
        xN = new NoiseAdder();
        yN = new NoiseAdder();
        zN = new NoiseAdder();      
    }

    /// <summary>
    /// Obtain the noise using the value passed as parameter and the past values
    /// </summary>
    /// <param name="val">Actual Vector3 which we need to apply the noise to</param>
    /// <returns></returns>
    public Vector3 getNoise(Vector3 val) { return new Vector3(xN.getNoise(val.x), yN.getNoise(val.y), zN.getNoise(val.z)); }
}
