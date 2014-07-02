using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace File_Wizard
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Members
        private string _folderPath = string.Empty;
        private string _filterText = string.Empty;
        private File _selectedFile = null;
        private BitmapImage _iconBitmap = null;
        private bool _showPopup = false;
        private bool _nextClicked = false;
        private bool _progressBarVisible = false;
        private bool _includeSubdirectories = false;
        private bool _restartBW = false; 
        private BackgroundWorker bw = new BackgroundWorker();
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor of wizard
        /// </summary>
        public MainWindowViewModel()
        {
            FileList = new ObservableCollection<File>();

            this.FileListView = (CollectionView)CollectionViewSource.GetDefaultView(_fileList);
            this.FileListView.Filter += new Predicate<object>(FileFilter);
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        
        }
        #endregion


        #region Properties       

        /// <summary>
        /// gets or sets whether File list panel is shown on ui or not
        /// </summary>
        public bool FileListShow
        {
            get { return _nextClicked; }
            set 
            {
                _nextClicked = value;
                RaisePropertyChanged("FileListShow");
            }        
        }

        /// <summary>
        /// gets or sets whether file properties popup should show or not
        /// </summary>
        public bool ShowPopup
        {
            get { return _showPopup; }
            set
            {
                _showPopup = value;
                RaisePropertyChanged("ShowPopup");
            }
        }

        /// <summary>
        /// gets or sets the icon bitmap for file properties popup
        /// </summary>
        public BitmapImage IconBitmap
        {
            get { return _iconBitmap; }
            set
            {
                _iconBitmap = value;
                RaisePropertyChanged("IconBitmap");
            }
        }
        /// <summary>
        /// gets or sets whether progress bar is visible or not
        /// </summary>
        public bool ProgressBarVisible
        {
            get { return _progressBarVisible; }
            set
            {
                _progressBarVisible = value;
                RaisePropertyChanged("ProgressBarVisible");
            }

        }

        /// <summary>
        /// gets or sets the folder path for which files are shown
        /// </summary>
        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                if (_folderPath != value)
                {
                    _folderPath = value;
                    RaisePropertyChanged("FolderPath");
                }
            }
        }

        /// <summary>
        /// selected File object in data grid
        /// </summary>
        public File SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                RaisePropertyChanged("SelectedFile");
            }        
        }

        /// <summary>
        /// Multiple selcted files 
        /// </summary>
        public IList SelectedFiles
        {
            get; set;
        }

        /// <summary>
        /// gets or sets search filter text for file list observable collection
        /// </summary>
        public string FilterText
        {
            get { return _filterText; }
            set 
            {
                if (value != _filterText)
                {
                    _filterText = value;
                    RaisePropertyChanged("FilterText");
                    this.FileListView.Refresh();
                    // refresh the file list view
                }
            }       
        }

        /// <summary>
        /// gets or sets whether checkbox is checked for recursive directories
        /// </summary>
        public bool IncludeSubdirectories
        {
            get { return _includeSubdirectories; }
            set
            {
                _includeSubdirectories = value;
                if (this.FileListShow)
                {
                    RestartBW();
                }
                RaisePropertyChanged("IncludeSubdirectories");
            }
        }

        /// <summary>
        /// Cancel button action binding
        /// </summary>
        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (null == _cancelCommand)
                {
                    _cancelCommand = new RelayCommand(() => this.CancelAction());
                }
                return _cancelCommand;
            }
        }

        /// <summary>
        /// Next button action binding 
        /// </summary>
        private ICommand _nextCommand;
        public ICommand NextCommand
        {
            get
            {
                if (null == _nextCommand)
                {
                    _nextCommand = new RelayCommand(() => this.NextAction(),()=>this.CanNextAction());
                }
                return _nextCommand;
            }
        }

        /// <summary>
        /// Open button action binding
        /// </summary>
        private ICommand _openInExplorer;
        public ICommand OpenInExplorer
        {
            get
            {
                if (null == _openInExplorer)
                {
                    _openInExplorer = new RelayCommand(() => this.OpenFileInExplorer());
                }
                return _openInExplorer;
            }
        }

        /// <summary>
        /// Copy path action binding
        /// </summary>
        private ICommand _copyPath;
        public ICommand CopyPath
        {
            get 
            {
                if (null == _copyPath)
                {
                    _copyPath = new RelayCommand(() => this.CopyFilePath());
                }
                return _copyPath;
            }
        }

        /// <summary>
        /// Show details action binding
        /// </summary>
        private ICommand _showDetails;
        public ICommand ShowDetails
        {
            get 
            {
                if (null == _showDetails)
                {
                    _showDetails = new RelayCommand(() => this.ShowFileDetails());
                }
                return _showDetails;
            }
        }

        /// <summary>
        /// Close button action binding
        /// </summary>
        private ICommand _cLosePopup;
        public ICommand CLosePopup
         {
             get
             {
                 if (null == _cLosePopup)
                 {
                     _cLosePopup = new RelayCommand(() => this.ClosePopup());
                 }
                 return _cLosePopup;
             }
         }
        
        /// <summary>
        /// File list observable collection
        /// </summary>
        private ObservableCollection<File> _fileList;
        public ObservableCollection<File> FileList
        {
            get
            {
                return _fileList; 
            }
            set
            {
                _fileList = value;
            }
        }

        /// <summary>
        /// File list collection view for search filter
        /// </summary>
        private ICollectionView _FileListView;
        public ICollectionView FileListView
        {
            get { return _FileListView; }
            set 
            { 
                _FileListView = value; 
                RaisePropertyChanged("FileListView"); 
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion




        #region Methods

        /// <summary>
        /// Should Next action button be enabled or not
        /// </summary>
        /// <returns></returns>
        private bool CanNextAction()
        {
            if (this.FileListShow)
            {

                if (this.SelectedFiles != null && this.SelectedFiles.Count> 0)
                    return true;
                else
                    return false;
            }
            else
            {
                if (!string.IsNullOrEmpty(this.FolderPath))
                {
                    if (Directory.Exists(this.FolderPath))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }        
        }

        /// <summary>
        /// Cancel function for back navigation or close application
        /// </summary>
        private void CancelAction()
        {
            this.bw.CancelAsync();
            if (this.FileListShow)
            {
                this.IncludeSubdirectories = false;
                this.FileListShow = false;
                Application.Current.MainWindow.Height = 200;
                Application.Current.MainWindow.Width = 525;                
            }
            else
            {
                Application.Current.MainWindow.Close();
            }
        }       

        /// <summary>
        /// Next function for forward navigation or open file
        /// </summary>
        private void NextAction()
        {
            if (!this.FileListShow)
            {
                Application.Current.MainWindow.Height = 600;
                Application.Current.MainWindow.Width = 500;
                this.FileListShow = true;
                RestartBW();
            }
            else
            {
                foreach (File file in this.SelectedFiles)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(file.FileName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// Open in file explorer contex menu
        /// </summary>
        private void OpenFileInExplorer()
        {
            try
            {
                if (this.SelectedFile != null)
                {
                    string argument = @"/select, " + this.SelectedFile.FileName;
                    Process.Start("explorer.exe", argument);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Copy file path context menu
        /// </summary>
        private void CopyFilePath()
        {
            try
            {
                if (this.SelectedFile != null)
                {
                    System.Windows.Clipboard.SetText(this.SelectedFile.FileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// close popup function
        /// </summary>
        private void ClosePopup()
        {
            this.ShowPopup = false;
        }

        /// <summary>
        /// Show file details context menu
        /// </summary>
        private void ShowFileDetails()
        {
            try
            {
                if (this.SelectedFile != null)
                {
                    System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(this.SelectedFile.FileName);
                    BitmapImage bitmapImage = null;
                    using (MemoryStream memory = new MemoryStream())
                    {
                        icon.ToBitmap().Save(memory, ImageFormat.Png);
                        memory.Position = 0;
                        bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                    }

                    this.IconBitmap = bitmapImage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Icon binding failed"+ ex.StackTrace);
            }

            this.ShowPopup = true;
        }

        /// <summary>
        /// Do work background thread to traverse directory and push files to UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string root = this.FolderPath + @"\";

            // structure to hold names of subfolders
            Stack<string> dirs = new Stack<string>();
            int _repProg = 0;

            if (!System.IO.Directory.Exists(root))
            {
                throw new ArgumentException();
            }
            dirs.Push(root);

            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();
                IEnumerable<string> subDirs;
                try
                {
                    subDirs = System.IO.Directory.GetDirectories(currentDir);
                }
                catch
                {

                    Console.WriteLine("not accesible");
                    continue;
                }

                string[] files = null;
                try
                {
                    files = System.IO.Directory.GetFiles(currentDir);
                }
                catch
                {
                    Console.WriteLine("not accesible");
                    continue;
                }

                // Perform the required action on each file here. 
                foreach (string file in files)
                {
                    try
                    {
                        if (bw.CancellationPending)
                        {
                            e.Cancel = true;
                            break;

                        }
                        else
                        {
                            FileInfo finfo = new FileInfo(file);

                            // Report progress was too slow for huge no of files.
                            //bw.ReportProgress(0, new File
                            //{
                            //    FileName = finfo.FullName,
                            //    FileSize = finfo.Length,
                            //    FileType = finfo.Extension,
                            //    Name = finfo.Name,
                            //    DirectoryName = finfo.DirectoryName,
                            //    IsReadOnly = finfo.IsReadOnly,
                            //    AccessedTime = finfo.LastAccessTime,
                            //    CreationTime = finfo.CreationTime,
                            //    ModifiedTime = finfo.LastWriteTime
                            //});


                            File tmp = new File
                            {
                                FileName = finfo.FullName,
                                FileSize = finfo.Length,
                                FileType = finfo.Extension,
                                Name = finfo.Name,
                                DirectoryName = finfo.DirectoryName,
                                IsReadOnly = finfo.IsReadOnly,
                                AccessedTime = finfo.LastAccessTime,
                                CreationTime = finfo.CreationTime,
                                ModifiedTime = finfo.LastWriteTime
                            };

                            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                this.FileList.Add(tmp);

                            }), DispatcherPriority.DataBind);

                            // Need to sleep, otherwise Ui becomes too busy
                            _repProg++;
                            if (_repProg == 20)
                            {
                                Thread.Sleep(200);
                                _repProg = 0;
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("not accesible");
                        continue;
                    }
                }

                if (this.IncludeSubdirectories)
                {
                    // Push the subdirectories onto the stack for traversal. 
                    foreach (string str in subDirs)
                    {
                        if (bw.CancellationPending)
                        {
                            e.Cancel = true;
                            break;

                        }
                        else
                        {
                            dirs.Push(str);
                        }
                    }
                }
            }
        }       

        /// <summary>
        /// background worker complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_restartBW)
            {
                bw.RunWorkerAsync();
                _restartBW = false;
            }
            else
            {
                this.ProgressBarVisible = false;
            }
        }

        /// <summary>
        /// Restart background worker
        /// </summary>
        private void RestartBW()
        {
            this.ProgressBarVisible = true;
            this.FileList.Clear();

            if (bw.IsBusy)
            {
                bw.CancelAsync();
                _restartBW = true;
            }
            else
            {
                bw.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Filter object for search function
        /// </summary>
        /// <param name="item">search text</param>
        /// <returns></returns>
        private bool FileFilter(object item)
        {
            File file = item as File;
            if (file.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Raise property event handler
        /// </summary>
        /// <param name="propertyName"></param>
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
