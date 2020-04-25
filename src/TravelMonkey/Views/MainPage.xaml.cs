using System;
using System.ComponentModel;
using TravelMonkey.Resources;
using TravelMonkey.ViewModels;
using Xamarin.Forms;

namespace TravelMonkey.Views
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private readonly MainPageViewModel _mainPageViewModel = new MainPageViewModel();

        public MainPage()
        {
            InitializeComponent();

            BindingContext = _mainPageViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _mainPageViewModel.StartSlideShow();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _mainPageViewModel.StopSlideShow();
        }

        private async void AddNewPicture_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddPicturePage());
        }

        private async void Entry_Completed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TranslateTextEntry.Text))
            {
                await DisplayAlert(LanguageResources.NoTextEnteredAlertTitle, LanguageResources.NoTextEnteredAlertContent, LanguageResources.NoTextEnteredAlertCancel);
                return;
            }

            await Navigation.PushModalAsync(new TranslationResultPage(TranslateTextEntry.Text));
            TranslateTextEntry.Text = "";
        }
    }
}