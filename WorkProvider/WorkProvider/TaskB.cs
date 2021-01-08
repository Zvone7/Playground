using WorkProvider.Infrastructrue;
using WorkProvider.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WorkProvider
{
    public class WorkTaskB<T> : IWork where T : IFileWriter
    {
        private readonly T _work_;

        public WorkTaskB(T work)
        {
            _work_ = work;
        }

        public override String ToString()
        {
            return _work_.ToString();
        }

        public async Task Execute()
        {
            await _work_.WriteAsync();
        }
    }

    public class WorkDequeuerTaskB : IWorkDequeuer
    {
        private readonly DateTime _started_;
        private Int32 _worksExecutedCounter_ = 0;
        private readonly WorkTaskB<IFileWriter> fileAfter2Seconds = new WorkTaskB<IFileWriter>(new FileWriter("file-1"));
        private readonly WorkTaskB<IFileWriter> fileAfter4Seconds = new WorkTaskB<IFileWriter>(new FileWriter("file-2"));
        private readonly WorkTaskB<IFileWriter> fileAfter6Seconds = new WorkTaskB<IFileWriter>(new FileWriter("file-3"));
        private readonly WorkTaskB<IFileWriter> fileAfter8Seconds = new WorkTaskB<IFileWriter>(new FileWriter("file-4"));

        public WorkDequeuerTaskB()
        {
            _started_ = DateTime.UtcNow;
        }

        public async Task<IWork> DequeueAsync()
        {

            if (DateTime.UtcNow - _started_ >= TimeSpan.FromSeconds(2) && _worksExecutedCounter_ == 0)
            {
                _worksExecutedCounter_++;
                return fileAfter2Seconds;
            }
            else if (DateTime.UtcNow - _started_ >= TimeSpan.FromSeconds(4) && _worksExecutedCounter_ == 1)
            {
                _worksExecutedCounter_++;
                return fileAfter4Seconds;
            }
            else if (DateTime.UtcNow - _started_ >= TimeSpan.FromSeconds(6) && _worksExecutedCounter_ == 2)
            {
                _worksExecutedCounter_++;
                return fileAfter6Seconds;
            }
            else if (DateTime.UtcNow - _started_ >= TimeSpan.FromSeconds(8) && _worksExecutedCounter_ == 3)
            {
                _worksExecutedCounter_++;
                return fileAfter8Seconds;
            }
            else
            {
                return null;
            }
        }
    }

    public class FileWriter : IFileWriter
    {
        private readonly String _fileName_;
        public FileWriter(String fileName)
        {
            _fileName_ = fileName;
        }
        public override String ToString()
        {
            return $"File {_fileName_}";
        }

        public async Task WriteAsync()
        {
            var lines = new List<String>() { $"1:{_fileName_}", $"2:{_fileName_}", $"3:{_fileName_}" };
            var outputfileName = $"{_fileName_}-out.txt";
            try
            {

                if (File.Exists(outputfileName))
                {
                    CustomLogger.Log(new String[] { $"File with name {outputfileName} exists. Deleting..." });
                    File.Delete(outputfileName);
                }

                CustomLogger.Log(new String[] { $"Started writing file {outputfileName}" });

                using (var writer = new StreamWriter(outputfileName))
                {
                    foreach (var line in lines)
                    {
                        if (!String.IsNullOrWhiteSpace(line))
                            writer.WriteLine(line);
                    }
                }
                CustomLogger.Log(new String[] { $"Finished writing file {outputfileName}" });
            }
            catch (Exception e)
            {
                CustomLogger.Log(new String[] { $"Exception writing file {outputfileName}.", e.ToString() });
            }
        }

    }
}
