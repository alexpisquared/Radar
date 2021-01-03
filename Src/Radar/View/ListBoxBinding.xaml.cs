using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;


namespace Radar
{
  public partial class ListBoxBinding : Window
	{
		public ListBoxBinding()
		{
			InitializeComponent();

			//One last thing, just to make it easier for us later we'll add a line to BitmapForWpfHelperWayWindow contractor:
			DataContext = this;
		}

		private void WindowLoaded(object s, EventArgs e)
		{
			imageListBox.DataContext = loadImageListFromFS();
			imageListBox.SelectedIndex = imageListBox.Items.Count - 1;
			imageListBox.Focus();
		}

		private void showListboxSelectedImage(object s, SelectionChangedEventArgs args)
		{
			var list = ((ListBox)s);
			if (list != null)
			{
				var index = list.SelectedIndex;	//Save the selected index 
				if (index >= 0)
				{
					var selection = list.SelectedItem?.ToString();

					if ((selection != null) && (selection.Length != 0))
					{
						//Set currentImage to selected Image
						var selLoc = new Uri(selection);
						var id = new BitmapImage(selLoc);
						var currFileInfo = new FileInfo(selection);
						currentImage.Source = id;

						Title = string.Format("{0}x{1} {2}", id.PixelWidth, id.PixelHeight, id.Format);
					}
				}
			}
		}



		private ArrayList loadImageListFromFS()
		{
			var imageFiles = new ArrayList();

			//string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\..\\..\\myData", "*.jpg");
			//  _d____________________________ataroot = Path.Combine((Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\SkyDrive", "UserFolder", System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "OneDrive")) ?? System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "OneDrive")).ToString(), "web.cache", "iWindsurf.jpg"); //tu: XP-proofed!!!
			foreach (var image in Directory.GetFiles(GetOneDriveFile("---www.weatheroffice.gc.ca-data-radar-temp_image*.Gif"))) //tu: XP-proofed!!!
				imageFiles.Add(new FileInfo(image));


			//imageFiles.Sort();

			return imageFiles;
		}

        static string GetOneDriveFile(string fff) => "";//is it used at all? Dec 2017: Path.Combine((OneDrive.Folder_Alex("web.cache")), fff);


		//Now let's write a property that will scan the "My Pictures" folder and load all the images it finds.
		public List<MyImage> AllImages
		{
			get
			{
				var result = new List<MyImage>();
				foreach (var filename in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)))
				{
					try
					{
						result.Add(
						 new MyImage(
						 new BitmapImage(
						 new Uri(filename)),
						 System.IO.Path.GetFileNameWithoutExtension(filename)));
					}
					catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex.Message, System.Reflection.MethodInfo.GetCurrentMethod()?.Name); if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break(); throw; }
				}
				return result;
			}
		}
	}



	public class UriToBitmapConverter3 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var bi = new BitmapImage();
			bi.BeginInit();
			bi.DecodePixelWidth = 100;
			bi.CacheOption = BitmapCacheOption.OnLoad;
			bi.UriSource = new Uri(value.ToString() ?? "");
			bi.EndInit();
			return bi;
		}
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new Exception("The method or operation is not implemented.");
  }

















}