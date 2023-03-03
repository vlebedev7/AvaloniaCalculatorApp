using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.CodeAnalysis;
using System.Reflection.PortableExecutable;
using System.Data.SqlTypes;

namespace AvaloniaTestApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public bool IsFirstNumber { get; set; } = true;
        public int FirstNumber { get; set; }
        public int SecondNumber { get; set; }
        public char Sign { get; set; }

        private int ApplyDigit(int number, int digit) => 
            number * 10 + digit;

        public void OnClickDigit(object sender, RoutedEventArgs e)
        {
            int digit = int.Parse((sender as Button).Content as string);

            if (IsFirstNumber)
                FirstNumber = ApplyDigit(FirstNumber, digit);
            else
                SecondNumber = ApplyDigit(SecondNumber, digit);
            UpdateDataContext();
        }

        public void OnClickSign(object sender, RoutedEventArgs e)
        {
            Sign = ((sender as Button).Content as string).Trim()[0];
            IsFirstNumber = false;
            UpdateDataContext();
        }

        private void UpdateDataContext()
        {
            (DataContext as MainViewModel).MainTextBox = FirstNumber.ToString();
        }
    }
}
