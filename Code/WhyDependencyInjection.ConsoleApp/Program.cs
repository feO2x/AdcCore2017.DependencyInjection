using System;
using System.IO;
using LightInject;

namespace WhyDependencyInjection.ConsoleApp
{
    public static class Program
    {
        public static void Main()
        {
            // Composition Root
            // Register
            var container = new ServiceContainer();
            container.Register<IReader, ConsoleReader>()
                     //.Register<IWriter, ConsoleWriter>()
                     .Register<IWriter>(f => new FileWriter("text.txt"), new PerRequestLifeTime())
                     .Register<CopyProcess>();

            // Resolve
            using (container.BeginScope())
            {
                var copyProcess = container.GetInstance<CopyProcess>();
                copyProcess.Copy();

                // Release
            }

        }
    }

    public sealed class CopyProcess
    {
        private readonly IReader _reader;
        private readonly IWriter _writer;

        public CopyProcess(IReader reader, IWriter writer)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public void Copy()
        {
            while (true)
            {
                var readResult = _reader.Read();
                if (readResult.ShouldQuit)
                    return;

                _writer.Write(readResult.Character);
            }
        }
    }

    public interface IReader
    {
        ReadResult Read();
    }

    public struct ReadResult
    {
        public readonly char Character;
        public readonly bool ShouldQuit;

        public ReadResult(char character, bool shouldQuit)
        {
            Character = character;
            ShouldQuit = shouldQuit;
        }
    }

    public sealed class ConsoleReader : IReader
    {
        public ReadResult Read()
        {
            var consoleKeyInfo = Console.ReadKey(true);
            return new ReadResult(consoleKeyInfo.KeyChar,
                                  consoleKeyInfo.Key == ConsoleKey.Escape);
        }
    }

    public interface IWriter
    {
        void Write(char character);
    }

    public sealed class ConsoleWriter : IWriter
    {
        public void Write(char character)
        {
            Console.Write(character);
        }
    }


    public sealed class FileWriter : IWriter, IDisposable
    {
        private readonly StreamWriter _streamWriter;

        public FileWriter(string filePath)
        {
            _streamWriter = new StreamWriter(filePath);
        }

        public void Write(char character)
        {
            _streamWriter.Write(character);
        }

        public void Dispose()
        {
            _streamWriter.Dispose();
        }
    }

    public static class File
    {
        private static StreamWriter _streamWriter;

        public static void Initialize(string path)
        {
            _streamWriter = new StreamWriter(path);
        }

        public static void Write(char character)
        {
            _streamWriter.Write(character);
        }

        public static void Dispose()
        {
            _streamWriter?.Dispose();
            _streamWriter = null;
        }
    }
}