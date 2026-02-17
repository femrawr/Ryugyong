using System.Text;

namespace Main.Source.Utilities
{
    public class Files
    {
        public static void CopyFiles(string? srcDir, string outDir)
        {
            if (string.IsNullOrEmpty(srcDir) || string.IsNullOrEmpty(outDir))
            {
                return;
            }

            foreach (var file in Directory.GetFiles(srcDir))
            {
                string newFile = Path.Join(outDir, Path.GetFileName(file));
                File.Copy(file, newFile, true);
            }

            foreach (var dir in Directory.GetDirectories(srcDir))
            {
                string newDir = Path.Join(outDir, Path.GetFileName(dir));
                Directory.CreateDirectory(newDir);

                CopyFiles(dir, newDir);
            }
        }

        public static int CountFiles(string? dirPath)
        {
            if (string.IsNullOrEmpty(dirPath))
            {
                return 0;
            }

            if (!Directory.Exists(dirPath))
            {
                return 0;
            }

            int count = Directory.GetFiles(dirPath).Length;

            foreach (var dir in Directory.GetDirectories(dirPath))
            {
                count += CountFiles(dir);
            }

            return count;
        }

        public static void SecureDelete(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            try
            {
                byte[] buffer = new byte[1 * 1024 * 1024];
                var rng = new Random();

                long fileSize = new FileInfo(filePath).Length;

                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Write))
                {
                    Array.Clear(buffer, 0, buffer.Length);
                    WriteChunk(stream, buffer, fileSize);

                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = 0xFF;
                    }

                    stream.Position = 0;
                    WriteChunk(stream, buffer, fileSize);

                    rng.NextBytes(buffer);

                    stream.Position = 0;
                    WriteChunk(stream, buffer, fileSize);
                }

                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                Logger.Error($"SecureDelete: [{ex.GetType()}] {ex.Message} - {ex.StackTrace}");
            }
        }

        public static void FileTree(
            string dirPath,
            string prefix,
            int depth,
            int maxDepth,
            StringBuilder builder
        )
        {
            if (!Directory.Exists(dirPath))
            {
                return;
            }

            if (depth >= maxDepth)
            {
                return;
            }

            var dirInfo = new DirectoryInfo(dirPath);

            var items = dirInfo.EnumerateFileSystemInfos().ToList();
            int count = items.Count;

            for (int i = 0; i < count; i++)
            {
                var item = items[i];

                bool last = i == count - 1;

                string next = last ? "└── " : "├── ";
                string name = item.Name;

                if ((item.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    builder.AppendLine($"{prefix}{next}📁 {name}");

                    string newPrefix = prefix + (last ? "    " : "│   ");
                    FileTree(item.FullName, newPrefix, depth + 1, maxDepth, builder);
                }
                else
                {
                    builder.AppendLine($"{prefix}{next}📄 {name}");
                }
            }
        }

        private static void WriteChunk(
            FileStream stream,
            byte[] buffer,
            long fileSize
        )
        {
            long written = 0;

            while (written < fileSize)
            {
                long toWrite = Math.Min(buffer.Length, fileSize - written);
                stream.Write(buffer, 0, (int)toWrite);
                stream.Flush();

                written += toWrite;
            }
        }
    }
}
