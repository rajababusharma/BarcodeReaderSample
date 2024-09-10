using BarcodeReaderSample.DB;
using BarcodeReaderSample.Models;
using BarcodeReaderSample.ViewModels;
using Honeywell.AIDC.CrossPlatform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeReaderSample.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class Scan : ContentPage
    {
        ScanViewModel viewModel;
        DatabaseController database;
        //..........................
        private const string DEFAULT_READER_KEY = "default";
        private Dictionary<string, BarcodeReader> mBarcodeReaders;
        private bool mContinuousScan = false, mOpenReader = false;
        private BarcodeReader mSelectedReader = null;
        private SynchronizationContext mUIContext = SynchronizationContext.Current;
        private int mTotalContinuousScanCount = 0;
        private bool mSoftContinuousScanStarted = false;
        private bool mSoftOneShotScanStarted = false;
        private string deviceModel = null;

        //.............................

        public Scan()
        {
            InitializeComponent();
            viewModel = new ScanViewModel();
            BindingContext = viewModel;
            database = new DatabaseController();
            mBarcodeReaders = new Dictionary<string, BarcodeReader>();
            viewModel.USER = Preferences.Get(Constants.username, "");
            this.Dispatcher.Dispatch(async() =>
            {
                OpenBarcodeReader();
                viewModel.ObjDocketList = await database.GetAll_Articles();
                viewModel.TOTALCOUNT = viewModel.ObjDocketList.Count.ToString();
                if (mSelectedReader != null && mSelectedReader.IsReaderOpened)
                {
                    BarcodeReader.Result result = await mSelectedReader.EnableAsync(true); // Enables or disables barcode reader
                    if (result.Code != BarcodeReader.Result.Codes.SUCCESS)
                    {
                        await DisplayAlert("Error", "EnableAsync failed, Code:" + result.Code +
                                            " Message:" + result.Message, "OK");
                    }
                }
            });
           
           

        }

        private string GetSelectedReaderName()
        {

            return DEFAULT_READER_KEY;

        }
        private async void UpdateBarcodeInfo(string data, string symbology, DateTime timestamp)
        {
            entrysku.Text = data;
            await ValidateDocket();
        }

        private async Task ValidateDocket()
        {
            try
            {
               
                if (!string.IsNullOrEmpty(entrysku.Text))
                {
                    /* if (entrydocket.Text.Length == 10)
                     {*/
                    viewModel.SKU = entrysku.Text;
                    //DisplayAlert("Alert", "Entry text changed", "Ok");
                    var isduplicate = Preferences.Get(Constants.ISDUPLICATE, false);
                    if (isduplicate) 
                    {
                        int result = await database.CheckArticleDuplicate(viewModel.SKU);
                        if (result > 0)
                        {
                            SpeakNow("Already Scanned");
                            entrysku.Text = "";
                            viewModel.SKU = "";
                          
                        }
                        else
                        {
                            Articles articles = new Articles();
                            articles.ARTICLES = viewModel.SKU;
                            articles.DATETIME = DateTime.Now.ToString();
                            articles.USER = viewModel.USER;
                            // mylist.Add(articles);
                            int p = await database.Insert_IntoARTICLES(articles);
                            if (p > 0)
                            {
                                entrysku.Text = "";
                                viewModel.SKU = "";
                                viewModel.ObjDocketList = await database.GetAll_Articles();

                            }
                        }
                    }
                    else
                    {
                        Articles articles = new Articles();
                        articles.ARTICLES = viewModel.SKU;
                        articles.DATETIME = DateTime.Now.ToString();
                        articles.USER = viewModel.USER;
                        // mylist.Add(articles);
                        int p = await database.Insert_IntoARTICLES(articles);
                        if (p > 0)
                        {
                            entrysku.Text = "";
                            viewModel.SKU = "";
                            viewModel.ObjDocketList = await database.GetAll_Articles();

                        }
                    }
                   
                }
                else
                {
                    this.Dispatcher.Dispatch(async () =>
                    {
                       
                        entrysku.Text = "";
                        viewModel.SKU = "";
                        // viewModel.ObjDocketList = database.GetAll_Articles();
                       
                    });
                    
                }
                viewModel.TOTALCOUNT = viewModel.ObjDocketList.Count.ToString();
               
            }
            catch (Exception excp)
            {
                this.Dispatcher.Dispatch(async () =>
                {
                  
                    entrysku.Text = "";
                    viewModel.SKU = "";
                  
                    await DisplayAlert("Alert", excp.Message, "OK");
                });
               

            }
        }


        public static void SpeakNow(string textTospeak)
        {
            var settings = new SpeechOptions()
            {
                Volume = 1.0f,
                Pitch = 1.0f
            };

            TextToSpeech.SpeakAsync(textTospeak, settings);
        }

        public BarcodeReader GetBarcodeReader(string readerName)
        {
            BarcodeReader reader = null;

            if (readerName == DEFAULT_READER_KEY)
            { // This name was added to the Open Reader picker list if the
              // query for connected barcode readers failed. It is not a
              // valid reader name. Set the readerName to null to default
              // to internal scanner.
                readerName = null;
            }

            if (null == readerName)
            {
                if (mBarcodeReaders.ContainsKey(DEFAULT_READER_KEY))
                {
                    reader = mBarcodeReaders[DEFAULT_READER_KEY];
                }
            }
            else
            {
                if (mBarcodeReaders.ContainsKey(readerName))
                {
                    reader = mBarcodeReaders[readerName];
                }
            }

            if (null == reader)
            {
                // Create a new instance of BarcodeReader object.
                reader = new BarcodeReader(readerName);

                // Add an event handler to receive barcode data.
                // Even though we may have multiple reader sessions, we only
                // have one event handler. In this app, no matter which reader
                // the data come frome it will update the same UI controls.
                reader.BarcodeDataReady += MBarcodeReader_BarcodeDataReady;
                reader.MenuCommandResponseReceived += Reader_MenuCommandResponseReceived;

                // Add the BarcodeReader object to mBarcodeReaders collection.
                if (null == readerName)
                {
                    mBarcodeReaders.Add(DEFAULT_READER_KEY, reader);
                }
                else
                {
                    mBarcodeReaders.Add(readerName, reader);
                }
            }

            return reader;
        }

        private void Reader_MenuCommandResponseReceived(object sender, MenuCommandDataArgs e)
        {
            // Update the menu command response in the UI thread.
            if (e.MenuCommandResponse != null && e.MenuCommandResponse.StartsWith("REVINF"))
            {
                var revInfString = e.MenuCommandResponse.Replace("REVINF", "");
                mUIContext.Post(_ => {
                    UpdateBarcodeInfo(revInfString, "", DateTime.Now);
                }
                    , null);
            }
        }

        // Event handler for the BarcodeDataReady event.
        private async void MBarcodeReader_BarcodeDataReady(object sender, BarcodeDataArgs e)
        {
            // Update the barcode information in the UI thread.
            mUIContext.Post(_ => {
                UpdateBarcodeInfo(e.Data, e.SymbologyName, e.TimeStamp);
            }
                , null);


            // Turn off the software trigger.
            await mSelectedReader.SoftwareTriggerAsync(false);
            mSoftOneShotScanStarted = false;

        }

        /// <summary>
        /// Opens the barcode reader. This method should be called from the
        /// main UI thread because it also updates the button states.
        /// </summary>
        public async void OpenBarcodeReader()
        {

            mSelectedReader = GetBarcodeReader(GetSelectedReaderName());
            if (!mSelectedReader.IsReaderOpened)
            {
                BarcodeReader.Result result = await mSelectedReader.OpenAsync();

                if (result.Code == BarcodeReader.Result.Codes.SUCCESS ||
                    result.Code == BarcodeReader.Result.Codes.READER_ALREADY_OPENED)
                {
                    SetScannerAndSymbologySettings();
                }
                else
                {
                    await DisplayAlert("Error", "OpenAsync failed, Code:" + result.Code +
                        " Message:" + result.Message, "OK");
                }
            }

        }

        /// <summary>
        /// Closes the barcode reader. This method should be called from the
        /// main UI thread because it also updates the button states.
        /// </summary>
        public async void CloseBarcodeReader()
        {
            if (mSelectedReader != null && mSelectedReader.IsReaderOpened)
            {
                if (mSoftOneShotScanStarted || mSoftContinuousScanStarted)
                {
                    // Turn off the software trigger.
                    await mSelectedReader.SoftwareTriggerAsync(false);
                    mSoftOneShotScanStarted = false;
                    mSoftContinuousScanStarted = false;
                }

                BarcodeReader.Result result = await mSelectedReader.CloseAsync();
                if (result.Code == BarcodeReader.Result.Codes.SUCCESS)
                {
                    mScanButton.IsEnabled = false;
                }
                else
                {
                    await DisplayAlert("Error", "CloseAsync failed, Code:" + result.Code +
                        " Message:" + result.Message, "OK");
                }
            }
        }

        private async void SetScannerAndSymbologySettings()
        {
            try
            {
                if (mSelectedReader.IsReaderOpened)
                {

                    // Specify settings for the scanner.
                    Dictionary<string, object> settings = new Dictionary<string, object>()
                                        {
                                            {mSelectedReader.SettingKeys.TriggerScanMode, mSelectedReader.SettingValues.TriggerScanMode_OneShot },
                                            {mSelectedReader.SettingKeys.Code128Enabled, true },
                                            {mSelectedReader.SettingKeys.Code39Enabled, true },
                                            {mSelectedReader.SettingKeys.Ean8Enabled, true },
                                            {mSelectedReader.SettingKeys.Ean8CheckDigitTransmitEnabled, true },
                                            {mSelectedReader.SettingKeys.Ean13Enabled, true },
                                            {mSelectedReader.SettingKeys.Ean13CheckDigitTransmitEnabled, true },
                                            {mSelectedReader.SettingKeys.Interleaved25Enabled, true },
                                            {mSelectedReader.SettingKeys.Interleaved25MaximumLength, 100 },
                                            {mSelectedReader.SettingKeys.Postal2DMode, mSelectedReader.SettingValues.Postal2DMode_Usps }
                                        };

                    BarcodeReader.Result result = await mSelectedReader.SetAsync(settings);
                    if (result.Code != BarcodeReader.Result.Codes.SUCCESS)
                    {
                        await DisplayAlert("Error", "Symbology settings failed, Code:" + result.Code +
                                            " Message:" + result.Message, "OK");
                    }

                }
            }
            catch (Exception exp)
            {
                await DisplayAlert("Error", "Symbology settings failed. Message: " + exp.Message, "OK");
            }
        }
       
        public async void OnScanButtonClicked(object sender, EventArgs args)
        {
            // await ValidateDocket();
            if (mSelectedReader != null && mSelectedReader.IsReaderOpened)
            {
                if (mContinuousScan)
                {
                    // The Continuous switch on the UI was turned on and the trigger
                    // scan mode was set to continuous scan. Pressing the SCAN button on
                    // the UI will begin the continuous scan.
                    mSoftContinuousScanStarted = true;
                }
                BarcodeReader.Result result = await mSelectedReader.SoftwareTriggerAsync(true);
                if (result.Code == BarcodeReader.Result.Codes.SUCCESS)
                {
                    // Set mSoftOneShotScanStarted to true if not in continuous scan mode.
                    // The mSoftOneShotScanStarted flag is used to turn off the software
                    // trigger after a barcode is read successfully.
                    mSoftOneShotScanStarted = !mSoftContinuousScanStarted;
                }
                else
                {
                    await DisplayAlert("Error", "Failed to turn on software trigger, Code:" + result.Code +
                        " Message:" + result.Message, "OK");
                }
            } //endif (mReaderOpened)
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            CloseBarcodeReader();
        }
    }
}
