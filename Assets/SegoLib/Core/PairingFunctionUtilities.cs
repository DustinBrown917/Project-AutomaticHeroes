using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    public static class PairingFunctions
    {
        public static ulong Pair(uint a, uint b)
        {
            if(a < b) {
                return ((ulong)a << 32) + b;
            } else {
                return ((ulong)b << 32) + a;
            }
        }

        public static uint Pair(ushort a, ushort b)
        {
            if (a < b) {
                return ((uint)a << 16) + b;
            } else {
                return ((uint)b << 16) + a;
            }
        }

        public static long Pair(int a, int b)
        {
            if (a < b) {
                return ((long)a << 32) + b;
            } else {
                return ((long)b << 32) + a;
            }
        }

        public static int Pair(short a, short b)
        {
            if (a < b) {
                return ((int)a << 16) + b;
            } else {
                return ((int)b << 16) + a;
            }
        }
    } 
}
