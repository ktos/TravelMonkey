using System.Threading.Tasks;
using Acr.UserDialogs;
using Plugin.Media;
using Plugin.Media.Abstractions;
using TravelMonkey.Data;
using TravelMonkey.Models;
using TravelMonkey.Resources;
using TravelMonkey.Services;
using Xamarin.Forms;

namespace TravelMonkey.ViewModels
{
    public class AddPicturePageViewModel : BaseViewModel
    {
        private readonly ComputerVisionService _computerVisionService = new ComputerVisionService();

        public bool ShowImagePlaceholder => !ShowPhoto;
        public bool ShowPhoto => _photoSource != null;

        private MediaFile _photo;
        private StreamImageSource _photoSource;

        public StreamImageSource PhotoSource
        {
            get => _photoSource;
            set
            {
                if (Set(ref _photoSource, value))
                {
                    RaisePropertyChanged(nameof(ShowPhoto));
                    RaisePropertyChanged(nameof(ShowImagePlaceholder));
                }
            }
        }

        private bool _isPosting;

        public bool IsPosting
        {
            get => _isPosting;
            set => Set(ref _isPosting, value);
        }

        private Color _pictureAccentColor = Color.SteelBlue;

        public Color PictureAccentColor
        {
            get => _pictureAccentColor;
            set => Set(ref _pictureAccentColor, value);
        }

        private string _pictureDescription;

        public string PictureDescription
        {
            get => _pictureDescription;
            set => Set(ref _pictureDescription, value);
        }

        public Command TakePhotoCommand { get; }
        public Command AddPictureCommand { get; }

        public AddPicturePageViewModel()
        {
            TakePhotoCommand = new Command(async () => await TakePhoto());
            AddPictureCommand = new Command(() =>
             {
                 MockDataStore.Pictures.Add(new PictureEntry { Description = _pictureDescription, Image = _photoSource });
                 MessagingCenter.Send(this, Constants.PictureAddedMessage);
             });
        }

        private async Task TakePhoto()
        {
            var result = await UserDialogs.Instance.ActionSheetAsync(LanguageResources.TakePhotoActionSheetTitle,
                LanguageResources.TakePhotoActionSheetCancel, null, null, LanguageResources.TakePhotoActionSheetTake, LanguageResources.TakePhotoActionSheetChoose);

            if (result.Equals(LanguageResources.TakePhotoActionSheetTake))
            {
                _photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions { PhotoSize = PhotoSize.Small });

                PhotoSource = (StreamImageSource)ImageSource.FromStream(() => _photo.GetStream());
            }
            else if (result.Equals(LanguageResources.TakePhotoActionSheetChoose))
            {
                _photo = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions { PhotoSize = PhotoSize.Small });

                PhotoSource = (StreamImageSource)ImageSource.FromStream(() => _photo.GetStream());
            }
            else
            {
                return;
            }

            if (_photo != null)
                await Post();
        }

        private async Task Post()
        {
            if (_photo == null)
            {
                await UserDialogs.Instance.AlertAsync(LanguageResources.NoImageSelectedAlertContent, LanguageResources.NoImageSelectedAlertTitle);
                return;
            }

            IsPosting = true;

            try
            {
                var pictureStream = _photo.GetStreamWithImageRotatedForExternalStorage();
                var result = await _computerVisionService.AddPicture(pictureStream);

                if (!result.Succeeded)
                {
                    MessagingCenter.Send(this, Constants.PictureFailedMessage);
                    return;
                }

                PictureAccentColor = result.AccentColor;

                PictureDescription = result.Description;

                if (!string.IsNullOrWhiteSpace(result.LandmarkDescription))
                    PictureDescription += $". {result.LandmarkDescription}";
            }
            finally
            {
                IsPosting = false;
            }
        }
    }
}