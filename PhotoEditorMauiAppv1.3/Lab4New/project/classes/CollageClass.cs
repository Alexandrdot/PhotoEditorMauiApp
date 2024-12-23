using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using Point = SixLabors.ImageSharp.Point;
namespace Lab4New
{
    public class Collage
    {
        public static Image CreateCollage(List<Image> photos)
        {
            // Простейший коллаж: изображения размещаются рядом друг с другом
            int totalWidth = photos.Sum(photo => photo.Width);
            int maxHeight = photos.Max(photo => photo.Height);

            var collage = new Image<Rgba32>(totalWidth, maxHeight);

            int currentX = 0;
            foreach (var photo in photos)
            {
                collage.Mutate(ctx => ctx.DrawImage(photo, new Point(currentX, 0), 1f));
                currentX += photo.Width;
            }
            return collage;
        }
    }
}

