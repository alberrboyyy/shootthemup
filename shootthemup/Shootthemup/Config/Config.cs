﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shootthemup //Shootthemup.Config.cs
{
    public enum ProjectileType
    {
        Enemy,
        Player
    }

    internal class Config
    {
        public static readonly int WIDTH = 1200;
        public static readonly int HEIGHT = 600;


        public static Random alea = new Random();

    }
}
