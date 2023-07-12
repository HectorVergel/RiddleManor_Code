using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shape : MonoBehaviour
{
    LayerMask layer;
    public Collider shapeCollider;
    [NonSerialized] public ShapeType type;
    List<GameObject> childs = new List<GameObject>();
    public Renderer runeRenderer;
    private void Start() {
        layer = gameObject.layer;
        childs.Add(gameObject);
        for (int i = 0; i < transform.childCount; i++)
        {
            childs.Add(transform.GetChild(i).gameObject);
        }
        if(shapeCollider==null) shapeCollider = GetComponent<Collider>();
        switch (shapeCollider)
        {
            case BoxCollider:
            type = ShapeType.Box;
            break;

            case SphereCollider:
            type = ShapeType.Sphere;
            break;

            case CapsuleCollider:
            type = ShapeType.Capsule;
            break;
        }
        Unselect();
    }
    public void SetRune(Material mat)
    {
        runeRenderer.material = mat;
    }
    public void SetSelected()
    {

    }
    public void Unselect()
    {

    }
}
public enum ShapeType
{
    Box,
    Sphere,
    Capsule
}
