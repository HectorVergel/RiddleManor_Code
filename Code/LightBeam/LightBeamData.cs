using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeamData
{
    public LightBeamData(LightBeam _beam, Vector3 _pos, Vector3 _dir)
    {
        this.beam = _beam;
        this.pos = _pos;
        this.dir = _dir;
    }
    public LightBeam beam;
    public Vector3 pos;
    public Vector3 dir;
}
