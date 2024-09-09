using MigraDocCore.DocumentObjectModel;
using MigraDocCore.Rendering;
using BarcodeReaderSample.DB;
using BarcodeReaderSample.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using static System.Collections.Specialized.BitVector32;
using Section = MigraDocCore.DocumentObjectModel.Section;
using AndroidX.Lifecycle;
using static Android.App.Assist.AssistStructure;
using BarcodeReaderSample.Views;
using Android.Provider;
using BarcodeReaderSample.View;
using Settings = BarcodeReaderSample.View.Settings;

namespace BarcodeReaderSample.ViewModels
{
    public class ScanViewModel : BaseViewModel
    {
        DatabaseController database;

        public Command OpenSettings { get; set; }
        public Command SUBMIT { get; set; }
        public ScanViewModel()
        {
            database = new DatabaseController();
           
            PdfSharpCore.Fonts.GlobalFontSettings.FontResolver = new FileFontResolver();

            OpenSettings = new Command(async() => 
            {
                await App.Current.MainPage.Navigation.PushAsync(new Settings());
            });

            SUBMIT = new Command(() =>
            {
                var fileformat = Preferences.Get(Constants.FILEFORMAT, ".csv");
                if (fileformat.Equals(".csv"))
                {
                    SubmitDetailsCSV();
                }
                else if(fileformat.Equals(".pdf"))
                {
                    SubmitDetailsPdf1();
                }
                else if (fileformat.Equals(".txt"))
                {
                    SubmitDetailsText();
                }
                else
                {
                    SubmitDetailsCSV();
                }
            });
        }

        private string count;
        public string COUNT
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                NotifyPropertyChanged("COUNT");
            }
        }

        private string totalcount;
        public string TOTALCOUNT
        {
            get
            {
                return totalcount;
            }
            set
            {
                totalcount = value;
                NotifyPropertyChanged("TOTALCOUNT");
            }
        }

        private string _user;
        public string USER
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                NotifyPropertyChanged("USER");
            }
        }

        private string article;
        public string SKU
        {
            get
            {
                return article;
            }
            set
            {
                article = value;
                NotifyPropertyChanged("SKU");
            }
        }

        List<Articles> _objdocketList;

        public List<Articles> ObjDocketList
        {
            get { return _objdocketList; }

            set
            {
                if (_objdocketList != value)
                {
                    _objdocketList = value;
                    // OnPropertyChanged("ObjPaymentList");
                    NotifyPropertyChanged("ObjDocketList");
                }
            }
        }

        public async void SubmitDetailsPdf1()
        {
           
     
            try
            {
               /* var tm = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute;
                 string directory = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);

                var pdfPath = Path.Combine(directory, "stocktake" + tm + ".pdf");*/
                var pdfPath = DBConstant.PdfPath();
                // string[] lines = ARTICLES.Split('\n');
                Document doc = new Document();
                Section section = doc.AddSection();

                var font = new MigraDocCore.DocumentObjectModel.Font("verdana_regular", 12);
                var HeaderFont = new MigraDocCore.DocumentObjectModel.Font("verdana_regular", 16);
               // MigraDocCore.DocumentObjectModel.Style style = doc.Styles["Normal"];
               // style.Font.Name = "OpenSansRegular";


                //just font arrangements as you wish
                // MigraDoc.DocumentObjectModel.Font font = new Font("Times New Roman", 15);
               // font.Bold = false;
               // HeaderFont.Bold = true;

                Paragraph paragraph = section.AddParagraph();
                paragraph.AddFormattedText("GRN Report", HeaderFont);
                paragraph.AddLineBreak();

                //add each line to pdf 
                

                int i = 1;
                foreach (var line in ObjDocketList)
                {
                    Paragraph para = section.AddParagraph();
                    para.AddFormattedText(i.ToString() + ":", HeaderFont);

                    Paragraph para1 = section.AddParagraph();
                    para1.AddFormattedText(line.ARTICLES, font);

                    Paragraph para2 = section.AddParagraph();
                    para2.AddFormattedText("User: " + line.USER, font);

                    Paragraph para3 = section.AddParagraph();
                    para3.AddFormattedText("Date: " + line.DATETIME, font);

                   // para3.AddLineBreak();
                   // para3.AddLineBreak();
                    i++;
                }

                    //save pdf document
                    PdfDocumentRenderer renderer = new()
                    {
                        Document = doc
                    };
                    renderer.RenderDocument();
                renderer.Save(pdfPath);


                database.Delete_AllArticles();
                ObjDocketList = await database.GetAll_Articles();


                TOTALCOUNT = ObjDocketList.Count.ToString();

                await App.Current.MainPage.DisplayAlert("Alert", "Data saved in " + pdfPath + " location", "Ok");
                SKU = "";
                // Process.Start(pdfPath);
                if (pdfPath != null)
                {
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(pdfPath)
                    });
                }
            }
            catch (Exception excp)
            {
                await App.Current.MainPage.DisplayAlert("Exception", excp.Message, "Ok");
            }
            
        }

        public async void SubmitDetailsCSV()
        {
            try
            {

              
                var csvPath = DBConstant.CSVPath();
                // File.Create(csvPath);

               
                //// build the data in memory
                //StringBuilder s = new StringBuilder();
                //foreach (var listing in articles)
                //{
                //    s = s.AppendLine(listing.BATCH_NO + "," + listing.ARTICLES + "," + listing.DATETIME);
                //}

                //// write the data all at once
                // File.WriteAllText(csvPath, s.ToString());
                //s.Clear();
                using (var logStream = new FileStream(csvPath, FileMode.Append, FileAccess.Write, FileShare.Write))
                using (var streamWriter = new StreamWriter(logStream))
                {
                    foreach (var listing in ObjDocketList)
                    {
                        streamWriter.WriteLine(listing.ARTICLES + "," + listing.DATETIME + "," + listing.USER);
                    }

                    streamWriter.Flush();
                    streamWriter.Close();
                    logStream.Close();
                }

                database.Delete_AllArticles();
                ObjDocketList =await database.GetAll_Articles();
                TOTALCOUNT = ObjDocketList.Count.ToString();
                await App.Current.MainPage.DisplayAlert("Alert", "Data saved in " + csvPath + " location", "Ok");

              
                if (csvPath != null)
                {
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(csvPath)
                    });
                }


            }
            catch (Exception excp)
            {
                await App.Current.MainPage.DisplayAlert("Exception", excp.Message, "Ok");
            }

        }

        public async void SubmitDetailsText()
        {
            try
            {


                var txtPath = DBConstant.TextPath();
               
                using (var logStream = new FileStream(txtPath, FileMode.Append, FileAccess.Write, FileShare.Write))
                using (var streamWriter = new StreamWriter(logStream))
                {
                    foreach (var listing in ObjDocketList)
                    {
                        streamWriter.WriteLine(listing.ARTICLES + "," + listing.DATETIME + "," + listing.USER);
                    }

                    streamWriter.Flush();
                    streamWriter.Close();
                    logStream.Close();
                }

                database.Delete_AllArticles();
                ObjDocketList = await database.GetAll_Articles();
                TOTALCOUNT = ObjDocketList.Count.ToString();
                await App.Current.MainPage.DisplayAlert("Alert", "Data saved in " + txtPath + " location", "Ok");


                if (txtPath != null)
                {
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(txtPath)
                    });
                }


            }
            catch (Exception excp)
            {
                await App.Current.MainPage.DisplayAlert("Exception", excp.Message, "Ok");
            }

        }
    }
}
