namespace OCRWorker;

internal class Program
{
    private static void Main(string[] args)
    {
        var worker = new OcrWorker();
        worker.Start();

        Console.WriteLine("OCR Worker is running. Press Ctrl+C to exit.");

        while (true) Thread.Sleep(1000);
    }
}