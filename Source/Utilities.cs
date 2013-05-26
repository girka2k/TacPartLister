﻿using KSP.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tac
{
    public class Utilities
    {
        public static Rect EnsureVisible(Rect pos, float min = 16.0f)
        {
            if ((pos.x + pos.width) < min)
            {
                pos.x = min - pos.width;
            }
            if (pos.x > (Screen.width - min))
            {
                pos.x = Screen.width - min;
            }
            if ((pos.y + pos.height) < min)
            {
                pos.y = min - pos.height;
            }
            if (pos.y > (Screen.height - min))
            {
                pos.y = Screen.height - min;
            }

            return pos;
        }

        public static Rect ClampToScreenEdge(Rect pos)
        {
            float topSeparation = Math.Abs(pos.y);
            float bottomSeparation = Math.Abs(Screen.height - pos.y - pos.height);
            float leftSeparation = Math.Abs(pos.x);
            float rightSeparation = Math.Abs(Screen.width - pos.x - pos.width);

            if (topSeparation <= bottomSeparation && topSeparation <= leftSeparation && topSeparation <= rightSeparation)
            {
                pos.y = 0;
            }
            else if (leftSeparation <= topSeparation && leftSeparation <= bottomSeparation && leftSeparation <= rightSeparation)
            {
                pos.x = 0;
            }
            else if (bottomSeparation <= topSeparation && bottomSeparation <= leftSeparation && bottomSeparation <= rightSeparation)
            {
                pos.y = Screen.height - pos.height;
            }
            else if (rightSeparation <= topSeparation && rightSeparation <= bottomSeparation && rightSeparation <= leftSeparation)
            {
                pos.x = Screen.width - pos.width;
            }

            return pos;
        }

        public static Texture2D LoadImage(string filename)
        {
            if (File.Exists<TacPartLister>(filename))
            {
                var bytes = File.ReadAllBytes<TacPartLister>(filename);
                Texture2D texture = new Texture2D(16, 16, TextureFormat.ARGB32, false);
                texture.LoadImage(bytes);
                return texture;
            }
            else
            {
                return null;
            }
        }

        public static float GetValue(ConfigNode config, string name, float currentValue)
        {
            float newFloat;
            if (config.HasValue(name) && float.TryParse(config.GetValue(name), out newFloat))
            {
                return newFloat;
            }
            else
            {
                return currentValue;
            }
        }
    }
}
