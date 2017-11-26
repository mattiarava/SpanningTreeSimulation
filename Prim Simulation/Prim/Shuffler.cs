using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prim
{
    public class Shuffler
    {

        public Shuffler()
        {
            //_rng = new Random();
        }

        public void Shuffle<T>(IList<T> array,Random _rng)
        {
            for (int n = array.Count; n > 1;)
            {
                int k = (_rng.Next(10000))%n;
                --n;
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        //private System.Random _rng;
    }
}
