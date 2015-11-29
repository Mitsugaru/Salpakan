using UnityEngine;
using System;
using System.Collections;
using strange.extensions.mediation.impl;

/// <summary>
/// http://gamedev.stackexchange.com/questions/74393/how-to-edit-key-value-pairs-like-a-dictionary-in-unitys-inspector
/// </summary>
public class UnitMaterialManager : View, IUnitMaterialManager
{

    [Serializable]
    public class UnitMaterial
    {
        public UnitRank rank;
        public Material material;
    }

    public UnitMaterial[] materials;

    public Material GetRankMaterial(UnitRank rank)
    {
        Material material = materials[0].material;

        foreach(UnitMaterial um in materials)
        {
            if(um.rank.Equals(rank))
            {
                material = um.material;
                break;
            }
        }

        return material;
    }
}
