using NUnit.Framework; // NUnit framework for unit testing  
using System;
using System.Drawing; // For using Bitmap and Color classes  

namespace MapMatrix2d.Generator.Tests
{
    [TestFixture] // Attribute that marks this class as containing tests  
    public class PerlinNoiseTests
    {
        [Test]
        public void GetNoiseMap_ReturnsBitmapWithCorrectDimensions()
        {
            // Arrange  
            int width = 100; // Expected width of the noise map  
            int height = 100; // Expected height of the noise map  
            float frequency = 0.1f; // Frequency setting for noise generation  
            float amplitude = 1.0f; // Amplitude setting for noise generation  
            float persistence = 0.5f; // Persistence affecting the contribution of each octave  
            int octaves = 4; // Number of noise layers  
            int seed = 12345; // Random seed for reproducibility  
            float power = 0.9f; // Power applied to the noise value  

            // Act  
            Bitmap result = PerlinNoise.GetNoiseMap(width, height, frequency, amplitude, persistence, octaves, seed, power);

            // Assert  
            Assert.That(width, Is.EqualTo(result.Width)); // Check if the width is correct  
            Assert.That(height, Is.EqualTo(result.Height)); // Check if the height is correct  
        }

        [Test]
        public void GetNoiseMatrix_ReturnsMatrixWithCorrectDimensions()
        {
            // Arrange  
            int width = 100; // Expected width of the noise matrix  
            int height = 100; // Expected height of the noise matrix  
            float frequency = 0.1f; // Frequency setting for noise generation  
            float amplitude = 1.0f; // Amplitude setting for noise generation  
            float persistence = 0.5f; // Persistence affecting the contribution of each octave  
            int octaves = 4; // Number of noise layers  
            int seed = 12345; // Random seed for reproducibility  
            float power = 0.9f; // Power applied to the noise value  

            // Act  
            float[,] result = PerlinNoise.GetNoiseMatrix(width, height, frequency, amplitude, persistence, octaves, seed, power);

            // Assert  
            Assert.That(width, Is.EqualTo(result.GetLength(0))); // Check if the width is correct  
            Assert.That(height, Is.EqualTo(result.GetLength(1))); // Check if the height is correct  
        }

        [Test]
        public void GetNoiseMatrix_ValuesAreClampedBetweenZeroAndOne()
        {
            // Arrange  
            int width = 100; // Expected width of the noise matrix  
            int height = 100; // Expected height of the noise matrix  
            float frequency = 0.1f; // Frequency setting for noise generation  
            float amplitude = 1.0f; // Amplitude setting for noise generation  
            float persistence = 0.5f; // Persistence affecting the contribution of each octave  
            int octaves = 4; // Number of noise layers  
            int seed = 12345; // Random seed for reproducibility  
            float power = 0.9f; // Power applied to the noise value  

            // Act  
            float[,] result = PerlinNoise.GetNoiseMatrix(width, height, frequency, amplitude, persistence, octaves, seed, power);

            // Assert  
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Assert.That(result[x, y], Is.GreaterThanOrEqualTo(0.0f)); // Check for lower bound  
                    Assert.That(result[x, y], Is.LessThanOrEqualTo(1.0f)); // Check for upper bound  
                }
            }
        }

        [Test]
        public void GetNoiseMap_PixelValuesAreClampedBetweenZeroAnd255()
        {
            // Arrange  
            int width = 100; // Expected width of the noise map  
            int height = 100; // Expected height of the noise map  
            float frequency = 0.1f; // Frequency setting for noise generation  
            float amplitude = 1.0f; // Amplitude setting for noise generation  
            float persistence = 0.5f; // Persistence affecting the contribution of each octave  
            int octaves = 4; // Number of noise layers  
            int seed = 12345; // Random seed for reproducibility  
            float power = 0.9f; // Power applied to the noise value  

            // Act  
            Bitmap result = PerlinNoise.GetNoiseMap(width, height, frequency, amplitude, persistence, octaves, seed, power);

            // Assert  
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixel = result.GetPixel(x, y); // Get the pixel color at (x,y)  
                    Assert.That(pixel.R, Is.GreaterThanOrEqualTo(0)); // Check for red channel lower bound  
                    Assert.That(pixel.R, Is.LessThanOrEqualTo(255)); // Check for red channel upper bound  
                    Assert.That(pixel.G, Is.GreaterThanOrEqualTo(0)); // Check for green channel lower bound  
                    Assert.That(pixel.G, Is.LessThanOrEqualTo(255)); // Check for green channel upper bound  
                    Assert.That(pixel.B, Is.GreaterThanOrEqualTo(0)); // Check for blue channel lower bound  
                    Assert.That(pixel.B, Is.LessThanOrEqualTo(255)); // Check for blue channel upper bound  
                }
            }
        }

        [Test]
        public void GenerateNoise_ReturnsNoiseWithCorrectDimensions()
        {
            // Arrange  
            int width = 100; // Expected width of the noise array  
            int height = 100; // Expected height of the noise array  
            int seed = 12345; // Random seed for reproducibility  

            // Act  
            float[,] noise = PerlinNoise.GenerateNoise(seed, width, height);

            // Assert  
            Assert.That(width, Is.EqualTo(noise.GetLength(0))); // Check if the width is correct  
            Assert.That(height, Is.EqualTo(noise.GetLength(1))); // Check if the height is correct  
        }

        [Test]
        public void GenerateNoise_ValuesAreClampedBetweenMinusOneAndOne()
        {
            // Arrange  
            int width = 100; // Expected width of the noise array  
            int height = 100; // Expected height of the noise array  
            int seed = 12345; // Random seed for reproducibility  

            // Act  
            float[,] noise = PerlinNoise.GenerateNoise(seed, width, height);

            // Assert  
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Assert.That(noise[x, y], Is.GreaterThanOrEqualTo(-1.0f)); // Check for lower bound  
                    Assert.That(noise[x, y], Is.LessThanOrEqualTo(1.0f)); // Check for upper bound  
                }
            }
        }

        [Test]
        public void CubicInterpolate_ReturnsCorrectValue()
        {
            // Arrange  
            float a = 1.0f; // First control point  
            float b = 2.0f; // Second control point  
            float c = 3.0f; // Third control point  
            float d = 4.0f; // Fourth control point  
            float t = 0.5f; // Interpolation parameter  

            // Act  
            float result = PerlinNoise.CubicInterpolate(a, b, c, d, t); // Compute interpolation  

            // Assert  
            Assert.That(2.5f, Is.InRange(result - 0.001f, result + 0.001f)); // Check if result is close to expected  
        }

        [Test]
        public void Clamp_Int_ReturnsClampedValue()
        {
            // Arrange  
            int value = 150; // Value to clamp  
            int min = 0; // Minimum bound  
            int max = 100; // Maximum bound  

            // Act  
            int result = PerlinNoise.Clamp(value, min, max); // Perform clamping  

            // Assert  
            Assert.That(max, Is.EqualTo(result)); // Check if result equals the maximum value  
        }

        [Test]
        public void Clamp_Float_ReturnsClampedValue()
        {
            // Arrange  
            float value = 1.5f; // Value to clamp  
            float min = 0.0f; // Minimum bound  
            float max = 1.0f; // Maximum bound  

            // Act  
            float result = PerlinNoise.Clamp(value, min, max); // Perform clamping  

            // Assert  
            Assert.That(max, Is.EqualTo(result)); // Check if result equals the maximum value  
        }
    }
}