using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using PointF = SixLabors.ImageSharp.PointF;
using Rectangle = SixLabors.ImageSharp.Rectangle;

namespace Lab4New

{
    public class PhotoEditor
    {
        public PhotoEditor(){}
        public static Image<Rgba32> LoadPhoto(string path)
        {
            var photo = Image.Load(path);
            return (Image<Rgba32>)photo;
        }

        public static void SavePhoto(string path, Image myimage)
        {
            myimage.Save(path);
        }

        public static void RotatePhoto(Image image, float degrees)
        {
            image.Mutate(x => x.Rotate(degrees));
        }

        public static void CropPhoto(Image image, int x, int y, int width, int height)
        {
            image.Mutate(ctx => ctx.Crop(new Rectangle(x, y, width, height)));
        }

        public static Image ApplyFilter(Image image, IFilter filter, float result)
        {
            filter.Apply(image, result);
            return image;
        }
        public static void AddTextToPhoto(Image<Rgba32> image, string text, int x, int y, float fontSize, Rgba32 color)
        {
            FontFamily fontFamily = SystemFonts.Families.FirstOrDefault();
            //FontFamily fontFamily = new FontFamily("Arial");
            var font = fontFamily.CreateFont(fontSize);
            image.Mutate(image => image.DrawText(text, font, color, new PointF(x, y)));
        }
        public static void ChangePixelColor(Image<Rgba32> image, int x, int y, Rgba32 color)
        {
            image[x, y] = color;
        }
    }
}