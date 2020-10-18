using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VisionClientApi;

namespace ImageTagger.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        const int FOUR_MEGABYTES = 4_194_304;

        readonly VisionClient _visionClient;

        AnalysisResult        _analysisResult;
        RelayCommand          _analyzeFromClipboard;
        RelayCommand          _analyzeImageCommand;
        RelayCommand          _exitCommand;
        RelayCommand          _openFilesCommand;
        
        bool                  _canAnalyzeForFree;
        long                  _fileLength;
        string                _fileName;
        string                _selectedImagePath;
        string                _statusText;
        ImageSource           _imageSource;

        public MainWindowViewModel()
        {
            var myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var json        = File.ReadAllText(System.IO.Path.Combine(myDocumentsPath, "ImageTagger.Settings.Json"));
            var settings = JsonConvert.DeserializeObject<JObject>(json);

            _visionClient = new VisionClient(settings["apiKey"].Value<string>(), settings["endpoint"].Value<string>());
        }

        public AnalysisResult Analysis
        {
            get { return _analysisResult; }
            set { AnnounceIt(() => _analysisResult = value); }
        }

        public ICommand AnalyzeFromClipboard => _analyzeFromClipboard ??= new RelayCommand(async () => await DoClipboardImageAnalysis());
        public ICommand AnalyzeImage => _analyzeImageCommand ??= new RelayCommand(async () => await DoImageAnalysis());
        public ICommand ExitCommand => _exitCommand ??= new RelayCommand(() => Quit());
        public ICommand OpenFiles => _openFilesCommand ??= new RelayCommand(() => OpenLocalFiles());
       
        public bool CanAnalyzeForFree
        {
            get { return _canAnalyzeForFree; }
            set { AnnounceIt(() => _canAnalyzeForFree = value); }
        }

        public long FileLength
        {
            get { return _fileLength; }
            set { AnnounceIt(() => _fileLength = value); }
        }

        public string FileName
        {
            get { return _fileName; }
            set { AnnounceIt(() => _fileName = value); }
        }

        public ImageSource FileSource
        {
            get { return _imageSource; }
            set 
            { 
                AnnounceIt(() => _imageSource = value);                
            }
        }

        public string SelectedImagePath
        {
            get { return _selectedImagePath; }
            set { AnnounceIt(() => _selectedImagePath = value); }
        }

        public string StatusText
        {
            get { return _statusText; }
            set { AnnounceIt(() => _statusText = value); }
        }

        public void OpenLocalFiles()
        {
            Analysis = null; // Remove any previous analysis
            FileSource = null;
            SelectImageFileUsingDialog();
            UpdateUiWithtNewFileDetails();
        }

        public void Quit()
        {
            App.Current.Shutdown();
        }

        async Task DoClipboardImageAnalysis()
        {
            ClearPreviousResults();

            var clipBoardData = Clipboard.GetDataObject();
            if (clipBoardData.GetDataPresent(DataFormats.Bitmap))
            {
                var bitmapSource = Clipboard.GetImage();
                var streamBytes = ConvertBitmapSourceToByteArray(bitmapSource);
                FileSource = bitmapSource;

                StatusText = "Clipboard Image";
                FileName = "Paste from Clipboard";
                FileLength = streamBytes.Length;
                SelectedImagePath = "Clipboard";
                CanAnalyzeForFree = FileLength < FOUR_MEGABYTES;

                Analysis = await _visionClient.AnalyzeImageBytes(streamBytes);                
                StatusText = Analysis.StatusText;
            }
            else
            {
                StatusText = "No image found in clipboard";
            }
        }

        private void ClearPreviousResults()
        {
            FileSource        = null;
            Analysis          = null;
            StatusText        = string.Empty;
            FileName          = string.Empty;
            FileLength        = 0;
            SelectedImagePath = string.Empty;
            CanAnalyzeForFree = false;
        }

        async Task DoImageAnalysis()
        {
            // ClearPreviousResults();

            if(SelectedImagePath?.Any() ?? false)
            {
                if (FileLength < FOUR_MEGABYTES)
                {
                    StatusText = "Analyzing...";
                    Analysis   = await _visionClient.AnalyzeImage(SelectedImagePath);
                    StatusText = Analysis.StatusText;                    
                }
            }
        }

        static byte[] ConvertBitmapSourceToByteArray(BitmapSource bitmapSource)
        {
            var encoder = new PngBitmapEncoder();
            var frame   = BitmapFrame.Create(bitmapSource);

            encoder.Frames.Add(frame);
            byte[] streamBytes;
            using (var memoryStream = new MemoryStream())
            {
                encoder.Save(memoryStream);
                streamBytes = memoryStream.ToArray();
            }
            return streamBytes;
        }

        private void SelectImageFileUsingDialog()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.bmp,*.jpg,*.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*",
                CheckFileExists = true
            };
            openFileDialog.ShowDialog();
            SelectedImagePath = openFileDialog.FileName;
        }

        private void SetBasicProperties()
        {
            var info = new FileInfo(_selectedImagePath);
            FileName = info.Name;
            FileLength = info.Length;
            FileSource = new BitmapImage(new Uri(SelectedImagePath));
            CanAnalyzeForFree = FileLength < FOUR_MEGABYTES;  // Files under 4MB are free
        }

        private void SuggestStatusTextBasedOnPathAndSize()
        {
            if (CanAnalyzeForFree)
            {
                StatusText = "Go ahead and analyze this!";
            }
            else if (FileName.Any())
            {
                StatusText = "Too big. Only 4MB or smaller images";
            }
            else
            {
                StatusText = "";
            }
        }

        private void UpdateUiWithtNewFileDetails()
        {
            if (!SelectedImagePath.Any())
            {
                FileName = string.Empty;
                FileLength = 0;
                return;
            }
            SetBasicProperties();
            SuggestStatusTextBasedOnPathAndSize();
        }
    }
}