using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.CodeAnalysis;
using System.Reflection.PortableExecutable;

namespace AvaloniaTestApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ResetValues();
        }

        public bool IsFirstNumber { get; set; }
        public int FirstNumber { get; set; }
        public int SecondNumber { get; set; }
        public int CurrentNumber => IsFirstNumber ? FirstNumber : SecondNumber;
        public char Sign { get; set; }
        public string Result { get; set; }

        private void ResetValues()
        {
            FirstNumber = 0;
            SecondNumber = 0;
            IsFirstNumber = true;
            Sign = (char)0;
        }

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

        public void OnClickEquals(object sender, RoutedEventArgs e)
        {
            if (Sign == 0)
                return;
            Result = Sign switch
            {
                '+' => (FirstNumber + SecondNumber).ToString(),
                '-' => (FirstNumber - SecondNumber).ToString(),
                '*' => (1.0f * FirstNumber * SecondNumber).ToString(),
                '/' => SecondNumber == 0
                    ? "Division by zero error"
                    : (1.0f * FirstNumber / SecondNumber).ToString(),
                _ => "No sign",
            };

            ResetValues();
            UpdateDataContext();
        }


        private void UpdateDataContext()
        {
            string numberStr = "";
            if (string.IsNullOrWhiteSpace(Result)) // show input values
            {
                if (FirstNumber != 0)
                    numberStr += FirstNumber;
                if (Sign != 0)
                    numberStr += Sign;
                if (Sign != 0 && SecondNumber != 0)
                    numberStr += SecondNumber;
            }
            else // show result
            {
                numberStr = Result;
            }
            (DataContext as MainViewModel).MainTextBox = numberStr;
        }
    }
}
