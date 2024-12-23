using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace Lab4New
{
    public interface IFilter
    {
        void Apply(Image image, float result);
    }

    public class Filter1 : IFilter
    {
        public void Apply(Image image, float result)
        {
            image.Mutate(x => x.Brightness(result)); // Осветление
        }
    }

    public class Filter2 : IFilter
    {
        public void Apply(Image image, float result)
        {
            image.Mutate(x => x.Hue(result)); // Изменение оттенка
        }
    }
}