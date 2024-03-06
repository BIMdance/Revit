using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace BIMdance.Revit.Utils;

public static class BitmapUtils
{
    private static readonly object LockObject = new();
    public static byte[] ToBytes(this Image image)
    {
        lock (LockObject)
        {
            var converter = new ImageConverter();
            return (byte[])converter.ConvertTo(image, typeof(byte[]));
        }
    }

    public static Bitmap ToBitmap(this byte[] bytes)
    {
        if (bytes == null || !bytes.Any())
            return null;

        var converter = new ImageConverter();
        return (Bitmap)converter.ConvertFrom(bytes);
    }

    public static Bitmap ToBitmap(this BitmapImage bitmapImage)
    {
        using var outStream = new MemoryStream();
        var bitmapEncoder = new BmpBitmapEncoder();
        bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapImage));
        bitmapEncoder.Save(outStream);
        var bitmap = new Bitmap(outStream);
        return new Bitmap(bitmap);
    }

    public static Bitmap ToBitmap(this BitmapSource source)
    {
        if (source == null)
            return null;
            
        var bmp = new Bitmap
        (
            source.PixelWidth,
            source.PixelHeight,
            PixelFormat.Format32bppPArgb
        );

        var data = bmp.LockBits
        (
            new Rectangle(Point.Empty, bmp.Size),
            ImageLockMode.WriteOnly,
            PixelFormat.Format32bppPArgb
        );

        source.CopyPixels
        (
            Int32Rect.Empty,
            data.Scan0,
            data.Height * data.Stride,
            data.Stride
        );

        bmp.UnlockBits(data);

        return bmp;
    }

    public static Bitmap ToBitmap(this string base64String)
    {
        if (string.IsNullOrWhiteSpace(base64String))
            return null;

        var bytes = Convert.FromBase64String(base64String);
        using var memoryStream = new MemoryStream(bytes);
        return new Bitmap(Image.FromStream(memoryStream));
    }

    public static string ToBase64String(this Bitmap bitmap)
    {
        if (bitmap == null)
            return null;
        
        var bytes = bitmap.ToBytes();
        return Convert.ToBase64String(bytes);
    }
    
    public static BitmapImage ToBitmapImage(this Bitmap bitmap)
    {
        using var memory = new MemoryStream();
        var bitmapImage = new BitmapImage();
            
        bitmap.Save(memory, ImageFormat.Png);
        memory.Position = 0;
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = memory;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
        bitmapImage.Freeze();

        return bitmapImage;
    }

    public static BitmapSource ToBitmapSource(this Bitmap bitmap)
    {
        if (bitmap == null)
            return null;

        try
        {
            lock (LockObject)
            {
                return Imaging.CreateBitmapSourceFromHBitmap(
                    bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            return null;
        }
    }

    public static void SetDefaultResolution(this Bitmap bitmap) => bitmap.SetResolution(96, 96);
}