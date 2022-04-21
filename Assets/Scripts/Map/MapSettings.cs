using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapSettings
{
    static List<Color> colors = new List<Color>() { new Color(.3f, .5f, .7f), new Color(.7f, .3f, .3f), new Color(.4f, .7f, .3f), Color.white };

    static public Color RoomTypeToColor(Type roomType)
    {
        switch (roomType)
        {
            case Type.Start:
                return colors[0];
            case Type.End:
                return colors[1];
            case Type.Special:
                return colors[2];
            case Type.Default:
                return colors[3];
            default:
                return colors[3];
        }
    }
}
