using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVFXEntity
{
    public VFXColor CurrentColor { get; }
    public Vector3 ExplosionPosition { get; }

}
