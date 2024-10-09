using SegoLib.Random;
using System.Collections.Generic;
using UnityEngine;


namespace SegoLib.Random
{
    [System.Serializable]
    public class PerlinNoise
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        [System.Serializable]
        public struct GenerationParameters
        {
            public Vector3Int repeat;
            public Vector3 scale;
            public int octaves;
            public float persistence;
            public bool useStep;
            public FloatRange step;

            public GenerationParameters(Vector3Int repeat, Vector3 scale, int octaves, float persistence, bool useStep, FloatRange step)
            {
                this.repeat = repeat;
                this.scale = scale;
                this.octaves = octaves;
                this.persistence = persistence;
                this.useStep = useStep;
                this.step = step;
            }
        }

        public class PermutationTable
        {
            public int this[int i] => m_table[i];
            private int[] m_table;

            public int Length => m_table.Length;

            public int PermutationRepeats { get; }
            public int PermutationLength { get; }

            public PermutationTable(IRandomNumberGenerator rng) : this(2, 256, rng)
            {

            }

            public PermutationTable(int permutationRepeats, int permutationLength, IRandomNumberGenerator rng)
            {
                PermutationRepeats = Mathf.Clamp(permutationRepeats, 1, 16);
                PermutationLength = Mathf.Clamp(permutationLength, 1, 1024);
                BuildPermutationTable(rng);
            }


            private void BuildPermutationTable(IRandomNumberGenerator rng)
            {
                m_table = new int[PermutationLength * PermutationRepeats];
                List<int> orderedNums = new List<int>();

                for (int i = 0; i < PermutationLength; i++) {
                    orderedNums.Add(i);
                }

                List<int> copiedNums;

                int baseIndex;

                for (int repeats = 0; repeats < PermutationRepeats; repeats++) {
                    baseIndex = repeats * PermutationLength;
                    if (baseIndex >= m_table.Length) { baseIndex = m_table.Length - 1; }

                    copiedNums = new List<int>(orderedNums);

                    int targetIndex;
                    for (int i = 0; i < PermutationLength; i++) {
                        targetIndex = rng.Range(0, copiedNums.Count);
                        m_table[baseIndex + i] = copiedNums[targetIndex];
                        copiedNums.RemoveAt(targetIndex);
                    }
                }
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/



        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Statics ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/


        public static GenerationParameters DefaultGenerationParameters { get; } =
            new GenerationParameters(
            repeat: Vector3Int.one * -1,
            scale: Vector3.one,
            octaves: 1,
            persistence: 1.0f,
            useStep: false,
            step: new FloatRange(0, 1)
            );




        private static float PerlinFade(float t)
        {
            //Ken Perlin's fade function. (6t^5 - 15t^4 + 10t^3)
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static float GetGradient(int hash, float x, float y, float z)
        {
            switch (hash & 0xF) {
                case 0x0: return x + y;
                case 0x1: return -x + y;
                case 0x2: return x - y;
                case 0x3: return -x - y;
                case 0x4: return x + z;
                case 0x5: return -x + z;
                case 0x6: return x - z;
                case 0x7: return -x - z;
                case 0x8: return y + z;
                case 0x9: return -y + z;
                case 0xA: return y - z;
                case 0xB: return -y - z;
                case 0xC: return y + x;
                case 0xD: return -y + z;
                case 0xE: return y - x;
                case 0xF: return -y - z;
                default: return 0; // never happens
            }
        }

        public static float GetPerlin(float x, float y, float z, GenerationParameters genParams, PermutationTable permutationTable)
        {
            float totalNoise = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxPossibleNoise = 0;

            for (int i = 0; i < genParams.octaves; i++) {
                totalNoise += GetSimplePerlin(x * frequency, y * frequency, z * frequency, genParams, permutationTable) * amplitude;

                maxPossibleNoise += amplitude;

                amplitude *= genParams.persistence;
                frequency *= 2;
            }

            float finalNoise = totalNoise / maxPossibleNoise;

            if (genParams.useStep) {
                if (genParams.step.Contains(finalNoise) )
                    finalNoise = 1;
                else 
                    finalNoise = 0; 
            }

            return finalNoise;
        }

        public static float GetSimplePerlin(float x, float y, float z, GenerationParameters genParams, PermutationTable permutationTable)
        {
            //Handle scale
            x *= genParams.scale.x;
            y *= genParams.scale.y;
            z *= genParams.scale.z;

            //Handle repeat overflows in the coordinates.
            if (genParams.repeat.x > 0) x = x % genParams.repeat.x;
            if (genParams.repeat.y > 0) y = y % genParams.repeat.y;
            if (genParams.repeat.z > 0) z = z % genParams.repeat.z;

            //Establish unit cube coords. Bind to 0-255 to avoid overflow errors
            int cubeX = (int)x & 255;
            int cubeY = (int)y & 255;
            int cubeZ = (int)z & 255;

            //Get location within cube unit.
            float localX = x - (int)x;
            float localY = y - (int)y;
            float localZ = z - (int)z;

            float u = PerlinFade(localX);
            float v = PerlinFade(localY);
            float w = PerlinFade(localZ);

            //Hash the eight coordinates of the Unit Cube found above.
            int p1, p2, p3, p4, p5, p6, p7, p8;

            p1 = permutationTable[permutationTable[permutationTable[cubeX] + cubeY] + cubeZ];
            p2 = permutationTable[permutationTable[permutationTable[cubeX] + Increment(cubeY, genParams.repeat.y)] + cubeZ];
            p3 = permutationTable[permutationTable[permutationTable[cubeX] + cubeY] + Increment(cubeZ, genParams.repeat.z)];
            p4 = permutationTable[permutationTable[permutationTable[cubeX] + Increment(cubeY, genParams.repeat.y)] + Increment(cubeZ, genParams.repeat.z)];
            p5 = permutationTable[permutationTable[permutationTable[Increment(cubeX, genParams.repeat.x)] + cubeY] + cubeZ];
            p6 = permutationTable[permutationTable[permutationTable[Increment(cubeX, genParams.repeat.x)] + Increment(cubeY, genParams.repeat.y)] + cubeZ];
            p7 = permutationTable[permutationTable[permutationTable[Increment(cubeX, genParams.repeat.x)] + cubeY] + Increment(cubeZ, genParams.repeat.z)];
            p8 = permutationTable[permutationTable[permutationTable[Increment(cubeX, genParams.repeat.x)] + Increment(cubeY, genParams.repeat.y)] + Increment(cubeZ, genParams.repeat.z)];

            float x1, y1, x2, y2;

            x1 = Mathf.Lerp(GetGradient(p1, localX, localY, localZ), GetGradient(p5, localX - 1, localY, localZ), u);
            x2 = Mathf.Lerp(GetGradient(p2, localX, localY - 1, localZ), GetGradient(p6, localX - 1, localY - 1, localZ), u);

            y1 = Mathf.Lerp(x1, x2, v);

            x1 = Mathf.Lerp(GetGradient(p3, localX, localY, localZ - 1), GetGradient(p7, localX - 1, localY, localZ - 1), u);
            x2 = Mathf.Lerp(GetGradient(p4, localX, localY - 1, localZ - 1), GetGradient(p8, localX - 1, localY - 1, localZ - 1), u);

            y2 = Mathf.Lerp(x1, x2, v);

            return (Mathf.Lerp(y1, y2, w) + 1) * 0.5f;
        }

        private static int Increment(int num, int repeat)
        {
            num++;
            if (repeat > 0) { num %= repeat; }

            return num;
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Statics ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

    } 
}
