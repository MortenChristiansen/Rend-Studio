using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer.Mathematics
{
    class MersenneTwister
    {
        private const int mtRand = 624;
        private const int M = 397;
        private const ulong MATRIX_A = 0x9908b0df;
        private const ulong UPPER_MASK = 0x80000000;
        private const ulong LOWER_MASK = 0x7fffffff;
        private const ulong TEMPERING_MASK_B = 0x9d2c5680;
        private const ulong TEMPERING_MASK_C = 0xefc60000;

        //private readonly ushort[] mtRand_xsubi = new ushort[3] { 723, 32761, 44444 }; //Apparently not used

        private static ulong TEMPERING_SHIFT_U(ulong y) { return y >> 11; }
        private static ulong TEMPERING_SHIFT_S(ulong y) { return y << 7; }
        private static ulong TEMPERING_SHIFT_T(ulong y) { return y << 15; }
        private static ulong TEMPERING_SHIFT_L(ulong y) { return y >> 18; }

        private ulong[] mt;
        private int mti;

        public MersenneTwister(bool random)
        {
            mt = new ulong[mtRand];
            if (!random)
            {
                Seed(0xf2710812);
            }
            else
            {
                Random randomSeed = new Random();
                Seed((ulong)randomSeed.Next(int.MaxValue));
            }
        }

        public MersenneTwister(ulong seed)
        {
            mt = new ulong[mtRand];
            Seed(seed);
        }

        public void Seed(ulong seed)
        {
            mt[0] = seed;// &0xffffffff;
            for (mti = 1; mti < mtRand; mti++)
            {
                mt[mti] = (69069 * mt[mti - 1]);// &0xffffffff;
            }
            ulong s = 3737373;
            for (mti = 1; mti < mtRand; mti++)
            {
                mt[mti] ^= s;
                s = s * 5531 + 81547;
                s ^= (s >> 9) ^ (s << 19);
            }
        }

        public float Rand()
        {
            ulong y;
            ulong[] mag01 = new ulong[] { 0x0, MATRIX_A };
            if (mti >= mtRand)
            {
                int kk;
                for (kk = 0; kk < mtRand - M; kk++)
                {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1];
                }
                for (; kk < mtRand - 1; kk++)
                {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1];
                }
                y = (mt[mtRand - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
                mt[mtRand - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1];
                mti = 0;
            }
            y = mt[mti++];
            y ^= TEMPERING_SHIFT_U(y);
            y ^= TEMPERING_SHIFT_S(y) & TEMPERING_MASK_B;
            y ^= TEMPERING_SHIFT_T(y) & TEMPERING_MASK_C;
            y ^= TEMPERING_SHIFT_L(y);
            float g = (float)y * 2.3283064370807974e-10f;
            while (g > 1.0f) g *= 0.1f;
            return g;
            //return ((float)y * 2.3283064370807974e-10f);
        }

        public ulong RandL()
        {
            ulong y;
            ulong[] mag01 = new ulong[] { 0x0, MATRIX_A };
            if (mti >= mtRand) 
            {
                int kk;
                for (kk = 0; kk < mtRand - M; kk++) 
	            {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1];
                }
                for (; kk < mtRand - 1; kk++) 
	            {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + (M - mtRand)] ^ (y >> 1) ^ mag01[y & 0x1];
                }
                y = (mt[mtRand - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
                mt[mtRand - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1];
                mti = 0;
            }
            y = mt[mti++];
            y ^= TEMPERING_SHIFT_U(y);
            y ^= TEMPERING_SHIFT_S(y) & TEMPERING_MASK_B;
            y ^= TEMPERING_SHIFT_T(y) & TEMPERING_MASK_C;
            y ^= TEMPERING_SHIFT_L(y);
            return y;
        }
    }
}
