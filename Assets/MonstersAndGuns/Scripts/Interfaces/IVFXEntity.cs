using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVFXEntity
{
    public Color CurrentColor { get; }
    public Vector3 ExplosionPosition { get; }

    public Material DamageMaterial { get; }

    public Material AttackMaterial { get; }
    public Material NormalMaterial { get; }

    public Renderer[] Renderers { get; }




}
