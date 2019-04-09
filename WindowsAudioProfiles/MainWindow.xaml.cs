using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using NAudio.CoreAudioApi;
using LiteDB;
using WindowsAudioProfiles.Entity;
using System.IO;

namespace WindowsAudioProfiles
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MMDevice DefaultDevice { get; set; }

        public static readonly string ConnectionString = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WindowsAudioProfiles", @"WindowsAudioProfiles.db");

        //public System.Collections.ObjectModel.ObservableCollection<Profile> MyProperty { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            InitList();
            InitCurrentBalance();
        }

        private void InitList()
        {
            
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(ConnectionString)))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(ConnectionString));
            }
            if (!File.Exists(ConnectionString))
            {
                return;
            }
            using (var db = new LiteDatabase(ConnectionString))
            {
                var collection = db.GetCollection<Profile>("profiles");
                var results = collection.FindAll();
                foreach (var profile in results)
                {
                    listBox.Items.Add(profile);
                }
            }
        }

        private void InitCurrentBalance()
        {
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
            DefaultDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications);

            LeftSlider.Value = DefaultDevice.AudioEndpointVolume.Channels[0].VolumeLevelScalar;
            RightSlider.Value = DefaultDevice.AudioEndpointVolume.Channels[1].VolumeLevelScalar;
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Profile p = listBox.SelectedItem as Profile;
            if (p == null)
            {
                ProfileName.Text = string.Empty;
            }
            else
            {
                ProfileName.Text = p.Name;
                LeftSlider.Value = p.Left;
                RightSlider.Value = p.Right;

                ApplyProfile(p);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedIndex == -1)
            {
                AddProfile();
            }
            else
            {
                UpdateProfile();
            }
        }

        private void AddProfile()
        {
            var profile = new Profile
            {
                Name = ProfileName.Text,
                Left = (float)LeftSlider.Value,
                Right = (float)RightSlider.Value
            };

            // add to list
            listBox.Items.Add(profile);

            // save to db
            using (var db = new LiteDatabase(ConnectionString))
            {
                var collection = db.GetCollection<Profile>("profiles");
                collection.Insert(profile);
            }

            // empty fields
            ProfileName.Text = string.Empty;

            // deselect item
            listBox.SelectedIndex = -1;
        }

        private void UpdateProfile()
        {
            Profile profile = listBox.SelectedItem as Profile;
            profile.Name = ProfileName.Text;
            profile.Left = (float)LeftSlider.Value;
            profile.Right = (float)RightSlider.Value;

            // empty fields
            ProfileName.Text = string.Empty;

            // save to db
            using (var db = new LiteDatabase(ConnectionString))
            {
                var collection = db.GetCollection<Profile>("profiles");
                collection.Update(profile);
            }

            // update list
            int index = listBox.SelectedIndex;
            listBox.Items.RemoveAt(index);
            listBox.Items.Insert(index, profile);

            // deselect item
            listBox.SelectedIndex = -1;
        }

        private void LeftSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var val = (int)(e.NewValue * 100);
            LeftValue.Text = val.ToString();

            DefaultDevice.AudioEndpointVolume.Channels[0].VolumeLevelScalar = (float)e.NewValue;
        }

        private void RightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var val = (int)(e.NewValue * 100);
            RightValue.Text = val.ToString();

            DefaultDevice.AudioEndpointVolume.Channels[1].VolumeLevelScalar = (float)e.NewValue;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            listBox.SelectedIndex = -1;
        }


        public static void ApplyProfile(Profile p)
        {
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();

            MMDevice defaultDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications);

            //var v1 = defaultDevice.AudioMeterInformation.PeakValues[0]; //i.e. left channel
            //var v2 = defaultDevice.AudioMeterInformation.PeakValues[1]; //i.e. left channel

            //defaultDevice.AudioEndpointVolume.Channels[0].VolumeLevelScalar = 0.8F;
            //defaultDevice.AudioEndpointVolume.Channels[1].VolumeLevelScalar = 0.8F;

            defaultDevice.AudioEndpointVolume.Channels[0].VolumeLevelScalar = p.Left;
            defaultDevice.AudioEndpointVolume.Channels[1].VolumeLevelScalar = p.Right;
        }
    }
}
