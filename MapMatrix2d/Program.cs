using MapMatrix2d.Generator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

class Program
{
    static int width = 256;
    static int height = 256;
    static BiomeModel[,] biomeMap;
    static float[,] heightMap;

    static readonly int MAX_HEIGHT = 255;

    static void Main(string[] args)
    {
        biomeMap = new BiomeModel[width, height];
        heightMap = new float[width, height];

        string seed = "";
        for (int i = 0; i < 8; i++) seed += new Random().Next(0, 10);
        int seedValue = Convert.ToInt32(seed);
        
        Bitmap noiseBitmap = PerlinNoise.GetNoiseMap(width, height, 0.1f, 1.0f, 0.1f, 16, 18, 0.9f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color pixelColor = noiseBitmap.GetPixel(x, y);
                heightMap[x, y] = pixelColor.R;
                biomeMap[x, y] = GetBiome((int)heightMap[x, y]);
            }
        }

        DrawMap();
        SaveMapToFile("generated_island.png");
    }

    static List<BiomeModel> biomes = new List<BiomeModel>
    {
        new BiomeModel("Deep Water", new RangeModel(0, 24), Color.DarkBlue),
        new BiomeModel("Water", new RangeModel(25, 49), Color.Blue),
        new BiomeModel("Sand", new RangeModel(50, 79), Color.Goldenrod),
        new BiomeModel("Valley", new RangeModel(80, 119), Color.DarkGreen),
        new BiomeModel("Isle", new RangeModel(120, 149), Color.Green),
        new BiomeModel("Forest", new RangeModel(150, 199), Color.LimeGreen),
        new BiomeModel("Mountain", new RangeModel(200, 243), Color.Gray),
        new BiomeModel("High Mountain", new RangeModel(244, MAX_HEIGHT), Color.WhiteSmoke)
    };

    static BiomeModel GetBiome(int height) => biomes.FirstOrDefault(x => x.HeightRange.Fits(height));

    static void DrawMap()
    {
        using (Bitmap bitmap = new Bitmap(width * 2, height * 2))
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color col = biomeMap[x, y].Col;
                    for (int dx = 0; dx < 2; dx++)
                    {
                        for (int dy = 0; dy < 2; dy++)
                        {
                            bitmap.SetPixel(x * 2 + dx, y * 2 + dy, col);
                        }
                    }
                }
            }
            bitmap.Save("generated_island.png", ImageFormat.Png); 
        }
    }

    static void SaveMapToFile(string filename)
    {
        using (Bitmap bitmap = new Bitmap(width, height))
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color col = biomeMap[x, y].Col;
                    bitmap.SetPixel(x, y, col);
                }
            }
            bitmap.Save(filename, ImageFormat.Png);
        }
    }
}

