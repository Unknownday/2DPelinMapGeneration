using MapMatrix2d.Generator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

class Program
{
    static int width = 100;
    static int height = 100;
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

        Bitmap noiseBitmap = PerlinNoise.GetNoiseMap(width, height, 0.1f, 0.9f, 0.5f, 5, seedValue, 0.5f);

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

        noiseBitmap.Save("1.bmp", ImageFormat.Bmp);
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


// Условие 1: Горы находятся минимум на удалении 20% от берега
// Условие 2: Горы имеют соотношение 3:5
// Условие 3: Горы могут занимать от 20% до 25% общего площади острова
// Условие 4: Вокруг гор должен находится плавный переход.
// Условие 5: Горный массив окружен выжинами плавно стекающими к низинам
// Условие 6: Низины должны занимать от 40% до 60% общего площади острова
// Условие 7: Холмы должны занимать от 15% до 40% общего площади острова
// Условие 8: Начиная с самой удаленной вершины горы в сторону берега должна течь река, впадающая в море.
// Условие 9: Река начинается с ручейка и расширается ближе к морю.
// Условие 10: Перепады высот в горах не должны превышать 3, в выжинах - 2, а в низинах 1.
// Условие 11: Остров имеет площадь 90% от размера карты.
// Условие 12: Каждый горный массив должен иметь максимум одну вершину 10 высоты
// Условие 13: Каждый горный массив должен иметь миннимум одну вершину на 1 превышающую отльные

/*
 * Перепады высоты в зависимости от региона: 
 * Горы 10-7
 * Выжина 6-4
 * Низина 3-2
 * Берег 1
 * Уровень воды 0
 */

