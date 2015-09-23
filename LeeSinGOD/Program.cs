using EloBuddy.SDK.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeSinGOD
{
    class Program
    {
        static void Main(string[] args)
        {
            Addon a = new Addon();
            Loading.OnLoadingComplete += a.start;
        }
    }
}
