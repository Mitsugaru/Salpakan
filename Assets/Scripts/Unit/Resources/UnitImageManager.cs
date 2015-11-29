using UnityEngine;
using System;
using System.Collections;
using strange.extensions.mediation.impl;

public class UnitImageManager : View, IUnitImageManager
{
    [Serializable]
    public class UnitImage
    {
        public UnitRank rank;
        public Sprite sprite;
    }

    public UnitImage[] images;

    public Sprite GetRankImage(UnitRank rank)
    {
        Sprite sprite = images[0].sprite;

        foreach(UnitImage image in images)
        {
            if(image.rank.Equals(rank))
            {
                sprite = image.sprite;
                break;
            }
        }

        return sprite;
    }
}
