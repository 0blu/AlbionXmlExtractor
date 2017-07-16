using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace AlbionXmlExtractor
{
    class AlbionXmlExtractor
    {
        private static DES des = new DESCryptoServiceProvider();
        private static readonly ICryptoTransform CryptoTransform = des.CreateDecryptor(Keys.rgbKey, Keys.rgbIV);

        static void Main(string[] args)
        {
            try
            {
                Console.WindowWidth = 200;
            }
            catch (Exception)
            {
            }

            string gameDataFolder;
            string outputDataFolder;
            if (args.Length == 2)
            {
                gameDataFolder = args[0];
                outputDataFolder = args[1];
            }
            else if (args.Length == 0)
            {
                gameDataFolder = @"C:\Program Files (x86)\AlbionOnline\game\Albion-Online_Data\StreamingAssets\GameData";
                outputDataFolder = Path.Combine(Environment.CurrentDirectory, "output");
            }
            else
            {
                Console.WriteLine("Usage: {0} <gameDataFolder> <outputDataFolder>", Path.GetFileName(Environment.GetCommandLineArgs()[0]));
                return;
            }

            DirectoryInfo gameDataFolderInfo = new DirectoryInfo(gameDataFolder);
            DirectoryInfo outputDataFolderInfo = new DirectoryInfo(outputDataFolder);


            var gameBinFiles = Directory.GetFiles(gameDataFolderInfo.FullName, "*.bin", SearchOption.AllDirectories);

            for(int i = 0; i < gameBinFiles.Length; i++)
            {
                string gameBinFile = gameBinFiles[i];

                FileInfo outputFileInfo = new FileInfo(gameBinFile.Replace(gameDataFolderInfo.FullName, outputDataFolderInfo.FullName));
                outputFileInfo.Directory?.Create();

                Console.WriteLine("{0:F1}%: {1}", i*(1f/gameBinFiles.Length)*100f, gameBinFile);
                using (Stream stream = GetStream(gameBinFile))
                {
                    using (Stream newFileStream = File.OpenWrite(Path.ChangeExtension(outputFileInfo.FullName, "xml")))
                    {
                        stream.CopyTo(newFileStream);
                    }
                }
            }
            Console.WriteLine("Done. Press enter to exit");
            Console.ReadLine();
        }

        public static Stream GetStream(string filePath)
        {
            Stream stream = File.OpenRead(filePath);
            var compressedStream = new CryptoStream(stream, CryptoTransform, CryptoStreamMode.Read);
            return new GZipStream(compressedStream, CompressionMode.Decompress);
        }
    }
}
