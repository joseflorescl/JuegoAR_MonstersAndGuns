using UnityEngine;

public interface IVFXEntity
{
    public Color CurrentColor { get; }
    public Vector3 ExplosionPosition { get; }
    public Material DamageMaterial => null; // Notar el uso de implementaci�n default de una property en la interface (C# 8.0)
    public Material AttackMaterial => null;
    public Material NormalMaterial => null;
    public Renderer[] Renderers => null;
}
