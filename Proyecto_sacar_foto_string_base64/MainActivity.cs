namespace CameraAppDemo
{
    using System;
    using System.Collections.Generic;
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.Graphics;
    using Android.OS;
    using Android.Provider;
    using Android.Widget;
    using Java.IO;
    using Environment = Android.OS.Environment;
    using Uri = Android.Net.Uri;


    public static class App {
        public static File _file;
        public static File _dir;     
        public static Bitmap bitmap;
    }

    [Activity(Label = "Camera App Demo", MainLauncher = true)]
    public class MainActivity : Activity
    {
       
        private ImageView myView;
        string _base64String;

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            ImageView myView = (ImageView)FindViewById(Resource.Id.imageView1);
            myView.Visibility = Android.Views.ViewStates.Visible;

            // Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);


            // Display in ImageView. We will resize the bitmap to fit the display
            // Loading the full sized image will consume to much memory 
            // and cause the application to crash.

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = this.myView.Height ;
            App.bitmap = App._file.Path.LoadAndResizeBitmap (width, height);

            string imagePath = App._file.Path.ToString();

            //Probando obtener base64
            // provide read access to the file
            System.IO.FileStream fs = new System.IO.FileStream(imagePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            // Create a byte array of file stream length
            byte[] ImageData = new byte[fs.Length];
            //Read block of bytes from stream into the byte array
            fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));
            //Close the File Stream
            fs.Close();
            //string _base64String = Convert.ToBase64String(ImageData);
            _base64String = Convert.ToBase64String(ImageData);
            Toast.MakeText(this, "String en base 64: " + _base64String, ToastLength.Short).Show();            
            //Fin codigo de prueba


            if (App.bitmap != null) {
                this.myView.SetImageBitmap (App.bitmap);
                App.bitmap = null;
            }

            // Dispose of the Java side bitmap.
            GC.Collect();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Button takePhoto = FindViewById<Button>(Resource.Id.myButton);
                myView = FindViewById<ImageView>(Resource.Id.imageView1);
                takePhoto.Click += TakeAPicture;

                Button sendToServer = FindViewById<Button>(Resource.Id.sendButton);
                sendToServer.Click += SendPicture;
                //sendToServer.Click += (sender, args) => MessageBox.Show("Ta-dah!");
            }

        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new File(
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), "CameraAppDemo");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = 
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            App._file = new File(App._dir, String.Format("Encuesta{0}.png", Guid.NewGuid()));

            //string photoName = App._file.ToString();
            //string pattern = ".*/";
            //string replacement = " ";
            //System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(pattern);
            //string FileName = rgx.Replace(photoName, replacement);

            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }

        private void SendPicture(object sender, EventArgs eventArgs)
        {
            //Uri address = new Uri("http://example.com/insert.php");
            var address = new UriBuilder("http://example.com/insert.php").Uri;
            System.Collections.Specialized.NameValueCollection nameValueCollection = new System.Collections.Specialized.NameValueCollection();
            nameValueCollection["Name"] = "string-input";

            var webClient = new System.Net.WebClient();
            webClient.UploadValuesAsync(address, "POST", nameValueCollection);
            Toast.MakeText(this, "Enviando al servidor: " + _base64String, ToastLength.Short).Show();
        }
    }
}
