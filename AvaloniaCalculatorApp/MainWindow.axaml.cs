using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Microsoft.CodeAnalysis;
using System.Reflection.PortableExecutable;
using System.Reactive.Disposables;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;

namespace AvaloniaCalculatorApp
{
    public partial class MainWindow : Window
    {
        public bool IsFirstNumber { get; set; }
        public string FirstNumber { get; set; }
        public string SecondNumber { get; set; }
        public string CurrentNumber => IsFirstNumber ? FirstNumber : SecondNumber;
        public char Sign { get; set; }
        public string Result { get; set; }

        public MainWindow()
        {
            this.Activated += (object sender, EventArgs wat) =>
            {
                var els = this.GetVisualDescendants().OfType<Button>();
                foreach (var el in this.GetVisualDescendants().OfType<Button>())
                {
                    //el.Width = 30;
                }
            };
            InitializeComponent();
            ResetValues();
        }

        private void ResetValues()
        {
            FirstNumber = "";
            SecondNumber = "";
            IsFirstNumber = true;
            Sign = (char)0;
            Result = null;
        }

        public void OnClickDigit(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Result))
                Result = null; // start new calculation
            int digit = int.Parse((sender as Button).Content as string);
            if (CurrentNumber == "" && digit == 0)
                return; // no trailing zeroes

            if (IsFirstNumber)
                FirstNumber += digit;
            else
                SecondNumber += digit;
            UpdateDataContext();
        }

        public void OnClickSign(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Result))
                MoveResultToFirstNumber();

            if (string.IsNullOrWhiteSpace(FirstNumber))
                return; // don't set sign if first number is not set
            Sign = ((sender as Button).Content as string).Trim()[0];
            IsFirstNumber = false;
            UpdateDataContext();
        }

        public void OnClickSignRoot(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Result))
                MoveResultToFirstNumber();

            if (Sign != 0)
            {
                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow("Error",
                    "Finish current calculation first", ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Warning);
                messageBoxStandardWindow.Show();
                return;
            }
            Sign = '√';
            UpdateDataContext();
        }

        public void OnClickDecimalPoint(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Result))
                Result = null; // start new calculation

            string currentNumber = CurrentNumber;
            if (currentNumber.Contains('.'))
                return;

            if (currentNumber == "")
                currentNumber = "0.";
            else
                currentNumber += ".";

            if (IsFirstNumber)
                FirstNumber = currentNumber;
            else
                SecondNumber = currentNumber;
            UpdateDataContext();
        }

        public void OnClickNumberPi(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Result))
                MoveResultToFirstNumber();

            if (IsFirstNumber)
                FirstNumber = Math.PI.ToString();
            else
                SecondNumber = Math.PI.ToString();
            UpdateDataContext();
        }

        public void OnClickDeleteChar(object sender, RoutedEventArgs e)
        {
            static string removeLastChar(string s) => s[0..(s.Length - 1)];

            if (!string.IsNullOrWhiteSpace(Result))
                MoveResultToFirstNumber();

            if (CurrentNumber == "")
                return;

            if (IsFirstNumber)
                FirstNumber = removeLastChar(FirstNumber);
            else
                SecondNumber = removeLastChar(SecondNumber);
            UpdateDataContext();
        }

        public void OnClickClear(object sender, RoutedEventArgs e)
        {
            ResetValues();
            UpdateDataContext();
        }

        public void OnClickEquals(object sender, RoutedEventArgs e)
        {
            if (Sign == 0)
                return;
            decimal firstNumber = 0, secondNumber = 0;
            try
            {
                firstNumber = decimal.Parse(FirstNumber);
                if (Sign != '√')
                    secondNumber = decimal.Parse(SecondNumber);
            }
            catch
            {
                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow("Error",
                    "Could not parse one of the numbers", ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Error);
                messageBoxStandardWindow.Show();
                return;
            }

            string result;
            try
            {
                result = Sign switch
                {
                    '+' => (firstNumber + secondNumber).Normalize().ToString(),
                    '-' => (firstNumber - secondNumber).Normalize().ToString(),
                    '*' => (firstNumber * secondNumber).Normalize().ToString(),
                    '/' => secondNumber == 0
                        ? "Division by zero error"
                        : (firstNumber / secondNumber).Normalize().ToString(),
                    '^' => Math.Pow((double)firstNumber, (double)secondNumber).ToString(),
                    '√' => Math.Sqrt((double)firstNumber).ToString(),
                    _ => "No sign",
                };
            }
            catch
            {
                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow("Error",
                    "Could not calculate", ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Error);
                messageBoxStandardWindow.Show();
                return;
            }

            ResetValues();
            Result = result;
            UpdateDataContext();
        }

        private void MoveResultToFirstNumber()
        {
            if (!string.IsNullOrWhiteSpace(Result))
            {
                bool isDecimal = decimal.TryParse(Result, out _);
                if (isDecimal) // if result is a valid number
                    FirstNumber = Result; // use result as first number in the new calculation
                Result = null; // clear result field
            }
        }

        private void UpdateDataContext()
        {
            string numberStr = "";
            if (string.IsNullOrWhiteSpace(Result)) // show input values
            {
                if (Sign == '√')
                {
                    numberStr = $"√{FirstNumber}";
                }
                else
                {
                    numberStr += FirstNumber;
                    if (Sign != 0)
                        numberStr += Sign;
                    if (Sign != 0 && SecondNumber != "")
                        numberStr += SecondNumber;
                }
            }
            else // show result
            {
                numberStr = Result;
            }
            (DataContext as MainViewModel).MainTextBox = numberStr;
        }
    }
}
