using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class Constants
    {
        public static readonly Dictionary<ColorTypeEnum, Color> Colors = new Dictionary<ColorTypeEnum, Color>()
        {
                { ColorTypeEnum.Blue, Color.blue },
                { ColorTypeEnum.Red, Color.red },
                { ColorTypeEnum.Green, Color.green },
                { ColorTypeEnum.Yellow, Color.yellow },
                { ColorTypeEnum.Grey, Color.grey },
                { ColorTypeEnum.Magenta, Color.magenta }
        };
    }
}
