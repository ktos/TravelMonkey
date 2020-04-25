using System;
using TravelMonkey.Resources;
using TravelMonkey.ViewModels;
using Xamarin.Forms;

namespace TravelMonkey.Views
{
    public partial class TranslationResultPage : ContentPage
    {
        private readonly TranslateResultPageViewModel _translateResultPageViewModel =
            new TranslateResultPageViewModel();

        public TranslationResultPage(string inputText)
        {
            InitializeComponent();

            MessagingCenter.Subscribe<TranslateResultPageViewModel>(this,
                Constants.TranslationFailedMessage,
                async (s) =>
                {
                    await DisplayAlert(LanguageResources.TranslationFailedAlertTitle, LanguageResources.TranslationFailedAlertContent, LanguageResources.TranslationFailedAlertCancel);
                });

            _translateResultPageViewModel.InputText = inputText;

            BindingContext = _translateResultPageViewModel;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}