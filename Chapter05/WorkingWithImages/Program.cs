using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

string imagesFolder = Path.Combine(Environment.CurrentDirectory, "images");

Console.WriteLine($"I will look for any images in the following folder:\n{imagesFolder}");
Console.WriteLine();

if(!Directory.Exists(imagesFolder))
{
    Console.WriteLine();
    Console.WriteLine("Folder does not exist");
    return;
}

IEnumerable<string> images = Directory.EnumerateFiles(imagesFolder);

foreach(string imagePath in images)
{
    if(Path.GetFileNameWithoutExtension(imagePath).EndsWith("-thumbnail"))
    {
        Console.WriteLine($"Skipping:\n {imagePath}");
        Console.WriteLine();

        continue; //file has already been converted.
    }

    string thumbnailPath = Path.Combine(
        Environment.CurrentDirectory, "images",
        Path.GetFileNameWithoutExtension(imagePath)
        + "-thumbnail" + Path.GetExtension(imagePath));
    
    using(Image image = Image.Load(imagePath))
    {
        Console.WriteLine($"Converting:\n  {imagePath}");
        Console.WriteLine($"To:\n {thumbnailPath}");
        image.Mutate(x=>x.Resize(image.Width / 10, image.Height / 10));
        image.Mutate(x=>x.Grayscale());
        image.Save(thumbnailPath);
    }
}

Console.WriteLine("Image processing complete. View the images folder.");

