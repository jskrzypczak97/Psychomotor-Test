using System;
using System.Collections.Generic;
using System.Windows;

namespace Psychomotor_Test
{
    public partial class MainWindow : Window
    {
        private List<Window> TestsList;
        private int ListIterator = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Session_Action(object sender, RoutedEventArgs e)
        {
            Complex_Test Complex_Test = new Complex_Test();
            Simple_Test Simple_Test = new Simple_Test();
            Sound_Test SoundTest = new Sound_Test();

            TestsList = new List<Window>
            {
                Complex_Test,
                Simple_Test,
                SoundTest
            };

            Session(null, null);
        }

        public void Session(object sender, EventArgs e)
        {
            if (ListIterator < TestsList.Count)
            {
                TestsList[ListIterator].IsEnabled = true;
                TestsList[ListIterator].Closed += Session;
                TestsList[ListIterator].Show();
                ListIterator++;
            }
            else
            {
                ListIterator = 0;
                TestsList.Clear();
            }
        }

        private void Complex_Reaction_Test_Action(object sender, RoutedEventArgs e)
        {
            Complex_Test Complex_Test = new Complex_Test();
            Complex_Test.IsEnabled = true;
            Complex_Test.Show();
        }

        private void Simple_Reaction_Test_Action(object sender, RoutedEventArgs e)
        {
            Simple_Test Simple_Test = new Simple_Test();
            Simple_Test.IsEnabled = true;
            Simple_Test.Show();
        }

        private void Sound_Reaction_Test_Action(object sender, RoutedEventArgs e)
        {
            Sound_Test SoundTest = new Sound_Test();
            SoundTest.IsEnabled = true;
            SoundTest.Show();
        }

        private void Finish_Action(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
