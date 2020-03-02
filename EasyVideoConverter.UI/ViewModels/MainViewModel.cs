using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using FFmpegWrapper;
using FFmpegWrapper.Extensions;
using MVVMbasics;
using MVVMbasics.Attributes;
using MVVMbasics.Commands;
using MVVMbasics.Viewmodels;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

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

            if (openFileDialog.FileNames.Any() && string.IsNullOrEmpty(OutputDir))
                OutputDir = Path.GetDirectoryName(openFileDialog.FileNames.FirstOrDefault());
        });

        public string Prefix { get; set; }

        public string PostFix { get; set; }

        public AudioFormat SelectedAudioFormat { get; set; } = AudioFormat.Mp2;

        public VideoFormat SelectedVideoFormat { get; set; } = VideoFormat.Same;

        public string OutputDir { get; set; }

        public AudioFormat[] AudioFormatDictionary { get; set; } = new []
        {
            AudioFormat.Mp2 ,
            AudioFormat.Mp3, 
            AudioFormat.Same 
        };

        public VideoFormat[] VideoFormats { get; set; } = new[]
        {
            VideoFormat.Same,
            VideoFormat.Avi,
            VideoFormat.Mp4
        };

        public ICommand ClearFilesCommand => new BaseCommand(() => { FilesCollection.Clear(); });

        public ICommand ConvertCommand => new BaseCommand(() =>
        {
            if(!FilesCollection.Any())
                return;

            if (!Directory.Exists(OutputDir))
                Directory.CreateDirectory(OutputDir);
            
            FfmpegConvertHelper.Convert(FilesCollection.ToArray(),SelectedVideoFormat,SelectedAudioFormat,null,OutputDir);
        });

        public ICommand SelectFolderCommand => new BaseCommand(() =>
        {
            var folderBrowser = new FolderBrowserDialog();

            if (!string.IsNullOrEmpty(OutputDir))
                folderBrowser.SelectedPath = OutputDir;

            var choice = folderBrowser.ShowDialog();

            if(choice != DialogResult.OK)
                return;

            OutputDir = folderBrowser.SelectedPath;
        });
    }
}
