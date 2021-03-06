﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Aggravation
{
    public enum PlayerType
    {
        Human = 1,
        Computer = 2
    }

    public enum ComputerDificulty
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }

	public partial class NewPlayerPrompt : UserControl
	{
        public String PlayerNameLabel { get; set; }
        public PlayerType PlayerType { get; set; }
        public Boolean IsPlayer1 { get; set; }
        public Int32 PlayerNumber { get; set; }
        public ComputerDificulty ComputerDificulty { get; set; }

        public Boolean IsActivated 
        { 
            get { return (Boolean)this.cbActive.IsChecked; }
            set { this.cbActive.IsChecked = value; } 
        }

        public String PlayerName 
        { 
            get { return this.txtName.Text; } 
            set { this.txtName.Text = value; } 
        }

        public NewPlayerPrompt()
		{
            this.ComputerDificulty = Aggravation.ComputerDificulty.Medium;
			InitializeComponent();
            this.Loaded += new RoutedEventHandler(NewPlayerPrompt_Loaded);
        }

        private void cbActive_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsPlayer1)
            {
                ((CheckBox)sender).IsChecked = true;
                return;
            }

            if ((Boolean)((CheckBox)sender).IsChecked)
                this.playerCanvas.Opacity = 1.0;
            else
                this.playerCanvas.Opacity = 0.5;
        }

        private void NewPlayerPrompt_Loaded(object sender, RoutedEventArgs e)
        {
            this.canvasDifficulty.Visibility = System.Windows.Visibility.Collapsed;
            this.cbActive.Click += new RoutedEventHandler(cbActive_Click);
            this.cbxType.SelectionChanged += new SelectionChangedEventHandler(cbxType_SelectionChanged);
            this.cbxDifficulty.SelectionChanged += new SelectionChangedEventHandler(cbxDifficulty_SelectionChanged); 

            this.txtName.Text = this.PlayerName;
            this.txtPlayerLabel.Text = this.PlayerNameLabel;
            if (this.PlayerType == Aggravation.PlayerType.Computer)
                this.cbxType.SelectedIndex = 1;
            else
                this.cbxType.SelectedIndex = 0;

            if (this.IsPlayer1)
            {
                this.cbActive.IsEnabled = false;
                this.cbActive.IsChecked = true;
                this.cbActive.Opacity = 0.3;
            }
            else
                this.cbActive.IsChecked = false;

            this.cbActive_Click(this.cbActive, new RoutedEventArgs());


            //junk
            this.canvasDifficulty.Visibility = System.Windows.Visibility.Visible;
            this.PlayerType = Aggravation.PlayerType.Computer;
            this.cbActive.IsChecked = true;
            this.cbxType.SelectedIndex = 1;
        }

        void cbxDifficulty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxDifficulty.SelectedIndex == 0) this.ComputerDificulty = Aggravation.ComputerDificulty.Easy;
            if (cbxDifficulty.SelectedIndex == 1) this.ComputerDificulty = Aggravation.ComputerDificulty.Medium;
            if (cbxDifficulty.SelectedIndex == 2) this.ComputerDificulty = Aggravation.ComputerDificulty.Hard;
        }

        private void cbxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex == 1)
            {
                this.canvasDifficulty.Visibility = System.Windows.Visibility.Visible;
                this.PlayerType = Aggravation.PlayerType.Computer;
            }
            else
            {
                this.canvasDifficulty.Visibility = System.Windows.Visibility.Collapsed;
                this.PlayerType = Aggravation.PlayerType.Human;
            }
        }
	}
}