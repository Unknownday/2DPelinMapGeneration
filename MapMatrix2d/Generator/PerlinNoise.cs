using System;
using System.Drawing;

namespace MapMatrix2d.Generator
{
    public static class PerlinNoise
    {
        /// <summary>  
        /// Generates a map of Perlin noise. 
        /// </summary>  
        /// <param name="width">Width of the map to be generated.</param>  
        /// <param name="height">Height of the map to be generated.</param>  
        /// <param name="frequency">Controls the scale of the noise features.Higher values result in smaller, more detailed patterns, while lower values create larger, smoother features.</param>  
        /// <param name="amplitude">Controls the intensity or height of the noise. Higher values produce more pronounced variations, while lower values result in flatter noise.</param>  
        /// <param name="persistence">Controls how much each octave contributes to the final noise. Lower values reduce the influence of higher octaves, creating smoother noise.</param>  
        /// <param name="octaves">The number of layers of noise added together. Each octave adds finer details at a higher frequency, increasing the complexity of the noise.</param>  
        /// <param name="seed">The seed value for the random number generator. Ensures that the same seed produces the same noise map, allowing for reproducibility.</param>  
        /// <param name="power">A value applied to the noise to modify its distribution. Values greater than 1 reduce low values and emphasize high values, while values less than 1 do the opposite. Useful for adjusting the balance between low and high areas.</param>  
        public static Bitmap GetNoiseMap(int width, int height, float frequency, float amplitude, float persistence, int octaves, int seed, float power = 1.0f)
        {
            Bitmap result = new Bitmap(width, height);
            float[,] noise = GenerateNoise(seed, width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float value = GetValue(x, y, width, height, frequency, amplitude, persistence, octaves, noise);

                    value = (value * 0.5f) + 0.5f;
                    value = (float)Math.Pow(value, power);

                    int rgbValue = Clamp((int)(value * 255), 0, 255);

                    result.SetPixel(x, y, Color.FromArgb(rgbValue, rgbValue, rgbValue));
                }
            }

            return result;
        }

        private static float GetValue(int x, int y, int width, int height, float frequency, float amplitude, float persistence, int octaves, float[,] noise)
        {
            float finalValue = 0.0f;

            for (int i = 0; i < octaves; i++)
            {
                finalValue += CubicInterpolateNoise(x * frequency, y * frequency, width, height, noise) * amplitude;
                frequency *= 2.0f;

                amplitude *= persistence;
            }

            return Clamp(finalValue, -1.0f, 1.0f);
        }

        private static float CubicInterpolateNoise(float x, float y, int width, int height, float[,] noise)
        {
            int x0 = (int)x;
            int y0 = (int)y;

            float fracX = x - x0;
            float fracY = y - y0;

            float[,] values = new float[4, 4];

            for (int i = -1; i <= 2; i++)
            {
                for (int j = -1; j <= 2; j++)
                {
                    int xi = (x0 + i + width) % width;

                    int yi = (y0 + j + height) % height;

                    values[i + 1, j + 1] = noise[xi, yi];
                }
            }

            float[] xInterpolated = new float[4];

            for (int i = 0; i < 4; i++) 
                xInterpolated[i] = CubicInterpolate(values[i, 0], values[i, 1], values[i, 2], values[i, 3], fracY);
            
            return CubicInterpolate(xInterpolated[0], xInterpolated[1], xInterpolated[2], xInterpolated[3], fracX);
        }

        private static float CubicInterpolate(float a, float b, float c, float d, float t)
        {
            float p = (d - c) - (a - b);

            return p * t * t * t + ((a - b) - p) * t * t + (c - a) * t + b;
        }

        private static float[,] GenerateNoise(int seed, int width, int height)
        {
            float[,] noise = new float[width, height];
            Random random = new Random(seed);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    noise[x, y] = (float)random.NextDouble() * 2 - 1;
                }
            }

            return noise;
        }

        private static int Clamp(int value, int min, int max) => value < min ? min : value > max ? max : value;
        private static float Clamp(float value, float min, float max) => value < min ? min : value > max ? max : value;
    }
}