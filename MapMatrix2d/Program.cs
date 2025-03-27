using MapMatrix2d.Generator;
using MapMatrix2d.Generator.Tests;
using NUnit.Framework;  
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

class Program
{
    static int width = 256; // Width of the generated map  
    static int height = 256; // Height of the generated map  
    static BiomeModel[,] biomeMap; // 2D array for storing biome information  
    static float[,] heightMap; // 2D array for storing height information  

    static readonly int MAX_HEIGHT = 255; // Maximum height value for noise generation  

    static void Main(string[] args)
    {
        RunAllTests(); // Run unit tests before map generation  
        biomeMap = new BiomeModel[width, height]; // Initialize biome map  
        heightMap = new float[width, height]; // Initialize height map  

        // Generate a random seed value for noise generation  
        string seed = "";
        for (int i = 0; i < 8; i++) seed += new Random().Next(0, 10);
        int seedValue = Convert.ToInt32(seed);

        // Generate a Perlin noise bitmap  
        Bitmap noiseBitmap = PerlinNoise.GetNoiseMap(width, height, 0.1f, 1.0f, 0.1f, 16, seedValue, 0.9f);

        // Populate height and biome maps based on the noise bitmap  
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color pixelColor = noiseBitmap.GetPixel(x, y); // Get the color of the current pixel  
                heightMap[x, y] = pixelColor.R; // Store the red channel as height value  
                biomeMap[x, y] = GetBiome((int)heightMap[x, y]); // Determine biome based on height  
            }
        }

        DrawMap(); // Draw the map with biomes  
        SaveMapToFile("generated_island.png"); // Save the generated map to a file  
    }

    static void RunAllTests()
    {
        // Run all unit tests in the PerlinNoiseTests class  
        var assembly = typeof(PerlinNoiseTests).Assembly;
        foreach (var test in assembly.GetTypes()
            .Where(t => t.GetCustomAttributes(typeof(TestFixtureAttribute), false).Length > 0))
        {
            // Create an instance of the test class  
            var instance = Activator.CreateInstance(test);
            foreach (var method in test.GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Length > 0))
            {
                // Invoke the test method  
                method.Invoke(instance, null);
            }
        }
    }

    // List of available biomes with their respective height ranges and colors  
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

    // Determine the biome based on height  
    static BiomeModel GetBiome(int height) => biomes.FirstOrDefault(x => x.HeightRange.Fits(height));

    // Method to draw the map based on the biomeMap and save it  
    static void DrawMap()
    {
        using (Bitmap bitmap = new Bitmap(width * 2, height * 2)) // Create a bitmap with double the dimensions for scaling  
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color col = biomeMap[x, y].Col; // Get color for the current biome  
                    for (int dx = 0; dx < 2; dx++) // Scale up pixels  
                    {
                        for (int dy = 0; dy < 2; dy++)
                        {
                            bitmap.SetPixel(x * 2 + dx, y * 2 + dy, col); // Set pixel color  
                        }
                    }
                }
            }
            bitmap.Save("generated_island.png", ImageFormat.Png); // Save the generated map as PNG  
        }
    }

    // Method to save the biome map to a file  
    static void SaveMapToFile(string filename)
    {
        using (Bitmap bitmap = new Bitmap(width, height)) // Create a new bitmap  
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color col = biomeMap[x, y].Col; // Get color for the current biome  
                    bitmap.SetPixel(x, y, col); // Set pixel color  
                }
            }
            bitmap.Save(filename, ImageFormat.Png); // Save the bitmap to the specified file  
        }
    }
}