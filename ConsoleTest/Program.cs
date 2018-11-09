using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFmpegWrapper;

namespace ConsoleTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            //var FfmpegConvertHelper = new FfmpegConvertHelper();

            //var canc = new CancellationTokenSource();

          var res = FfmpegConvertHelper.Convert(
                new []
                {
                    "D:\\Downloads\\Elementary (Season 05) LostFilm\\Elementary.[S05E01].XviD.LostFilm.[qqss44].avi",
                    "D:\\Downloads\\Elementary (Season 05) LostFilm\\Elementary.[S05E02].XviD.LostFilm.[qqss44].avi"
                },
                VideoFormat.Same, AudioFormat.Mp2, s => Console.WriteLine(s), null, "Conv");
            Console.WriteLine(res);
            var x =  Console.ReadLine();

        }
    }
}
