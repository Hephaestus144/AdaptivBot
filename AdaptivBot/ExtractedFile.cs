using System.ComponentModel;


namespace AdaptivBot
{
    public class ExtractedFile : INotifyPropertyChanged
    {
        private string _filePath;

        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    this.OnPropertyChanged("FilePath");
                }
            }
        }

        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    this.OnPropertyChanged("FileName");
                }
            }
        }

        private string _fileSize;
        public string FileSize
        {
            get => _fileSize;
            set
            {
                if (_fileSize != value)
                {
                    _fileSize = value;
                    this.OnPropertyChanged("FileSize");
                }
            }
        }

        private string _fileType;
        public string FileType
        {
            get { return _fileType; }
            set
            {
                if (_fileType != value)
                {
                    _fileType = value;
                    this.OnPropertyChanged("FileType");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
