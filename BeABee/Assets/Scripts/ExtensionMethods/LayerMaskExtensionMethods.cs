using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtensionMethods
{
    public static bool Contains(this LayerMask layerMask, int layer)
    {
        int layerbit = 1 << layer;
        return (layerMask & layerbit) != 0;
    }
}
