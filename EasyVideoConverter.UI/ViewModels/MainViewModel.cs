using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FFmpegWrapper;
using FFmpegWrapper.Extensions;
using Microsoft.Win32;
using MVVMbasics;
using MVVMbasics.Attributes;
using MVVMbasics.Commands;
using MVVMbasics.Viewmodels;

namespace EasyVideoConverter.UI.ViewModels
{
    [MvvmBindableProperties]
    public class MainViewModel : BaseViewmodel
    {

        public ObservableCollection<string> FilesCollection { get; set; } = new ObservableCollection<string>();

        public ICommand AddFilesCommand => new BaseCommand(() =>
        {
            var openFileDialog = new OpenFileDialog(){Multiselect = true};

            var result = openFileDialog.ShowDialog();

            if (result != true)
                return;

            foreach (var file in openFileDialog.FileNames)
                FilesCollection.Add(file);

            if (openFileDialog.FileNames.Any())
                OutputDir = Path.GetDirectoryName(openFileDialog.FileNames.FirstOrDefault());
        });

        public string Prefix { get; set; }

        public string PostFix { get; set; }

        public AudioFormat SelectedAudioFormat { get; set; }

        public string OutputDir { get; set; }

        public Dictionary<string,AudioFormat> AudioFormatDictionary { get; set; } = new Dictionary<string, AudioFormat>()
        {
            { AudioFormat.Mp2.GetDescription(),AudioFormat.Mp2 },
            { AudioFormat.Mp3.GetDescription(),AudioFormat.Mp3 },
            { AudioFormat.Same.GetDescription(),AudioFormat.Same }
        };

        public ICommand ClearFilesCommand => new BaseCommand(() => { FilesCollection.Clear(); });

        public ICommand ConvertCommand => new BaseCommand(() =>
        {
            if(!FilesCollection.Any())
                return;

            if (!Directory.Exists(OutputDir))
                Directory.CreateDirectory(OutputDir);
            
            FfmpegConvertHelper.Convert(FilesCollection.ToArray(),VideoFormat.Same,AudioFormat.Mp2,null,OutputDir);
        });
    }
}
