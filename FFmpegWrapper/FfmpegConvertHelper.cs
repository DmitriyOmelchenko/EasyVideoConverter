using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFmpegWrapper.Extensions;

namespace FFmpegWrapper
{
    public enum VideoFormat
    {
        [Description("copy")]
        Same = 0,
        [Description("avi")]
        Avi,
        [Description("mpeg4")]
        Mp4
    }

    public enum AudioFormat
    {
        [Description("copy")]
        Same = 0,
        [Description("libmp3lame")]
        Mp3,
        [Description("mp2")]
        Mp2,
    }

    public static class FfmpegConvertHelper
    {
        private static string AddPrefix(string outputPrefix) => string.IsNullOrEmpty(outputPrefix)
            ? outputPrefix
            : outputPrefix + "_";

        private static string AddPostfix(string outputPostfix) => string.IsNullOrEmpty(outputPostfix)
            ? outputPostfix
            : "_" + outputPostfix;

        private static string AddVideoExt(string file, VideoFormat videoFormat)
        {
            switch (videoFormat)
            {
                case VideoFormat.Same:
                    return Path.GetExtension(file);
                case VideoFormat.Avi:
                    return ".avi";
                case VideoFormat.Mp4:
                    return ".mp4";
            }

            return null;
        }



        public static bool Convert(IEnumerable<string> inputFiles, VideoFormat videoFormat, 
            AudioFormat audioFormat, Action<string> writelog,
            string destinationFolder = null, string outputPrefix = null, string outPutPostFix = null)
        {
            try
            {
                var filesArray = inputFiles as string[] ?? inputFiles.ToArray();

                var arguments = new StringBuilder();

                var anotherFolderPlacment = !string.IsNullOrEmpty(destinationFolder);

                foreach (var file in filesArray)
                {
                    arguments.Append($"-i \"{file}\" ");
                }

                for (int i = 0; i < filesArray.Count(); i++)
                {
                    var currentFile = filesArray[i];

                    var destinationFileName =
                        $"{AddPrefix(outputPrefix)}{Path.GetFileNameWithoutExtension(currentFile)}{AddPostfix(outPutPostFix)}{AddVideoExt(currentFile, videoFormat)}";

                    var outPutFile = string.Empty;

                    if (anotherFolderPlacment)
                        outPutFile = Path.Combine(destinationFolder, destinationFileName);
                    else
                        outPutFile = Path.Combine(Path.GetDirectoryName(currentFile), destinationFileName);

                    arguments.Append(
                        $"-map {i} -vcodec {videoFormat.GetDescription()} -acodec {audioFormat.GetDescription()} \"{outPutFile}\" ");
                }

                Console.WriteLine(arguments);

                try
                {
                   var ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FFmpeg",
                            "ffmpeg.exe");
                    Console.WriteLine(ffmpegPath);
                    var process = new Process()
                    {
                        StartInfo = new ProcessStartInfo()
                        {
                            FileName = ffmpegPath,
                            Arguments = arguments.ToString(),
                            //UseShellExecute = false,
                            //RedirectStandardOutput = true,
                            //RedirectStandardError = true
                        }

                    };

                    //    process.OutputDataReceived += (s, arg) =>
                    //    {
                    //        writelog.Invoke(arg.Data);
                    //    };

                    //    process.ErrorDataReceived += (sender, args) => { writelog?.Invoke(args.Data); };

                    //process.BeginErrorReadLine();
                    //process.BeginOutputReadLine();
                    process.Start();
                    process.WaitForExit();
                    try
                    {
                        //while (!process.HasExited)
                        //{
                        //    cancellationToken.ThrowIfCancellationRequested();
                        //    Thread.Sleep(100);
                        //}

                        if (process.ExitCode != 0)
                            return false;
                    }
                    catch (OperationCanceledException e)
                    {
                        process?.Close();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    writelog?.Invoke(ex.ToString());
                }
            }
            catch (Exception e)
            {
                writelog?.Invoke($"Failed with exception {e}");
                return false;
            }

            return true;
        }
    }
}
