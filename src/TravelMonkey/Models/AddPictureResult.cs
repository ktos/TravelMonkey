using TravelMonkey.Resources;
using Xamarin.Forms;

namespace TravelMonkey.Models
{
    public class AddPictureResult
    {
        public string Description { get; }
        public string LandmarkDescription { get; }
        public Color AccentColor { get; }
        public bool Succeeded => !string.IsNullOrEmpty(Description) && AccentColor != Color.Default;

        public AddPictureResult()
        {
        }

        public AddPictureResult(string description, Color accentColor, string landmarkDescription = "")
        {
            Description = string.Format(LanguageResources.AddPictureResultDescription, description);
            AccentColor = accentColor;
            LandmarkDescription = string.IsNullOrWhiteSpace(landmarkDescription) ? "" : string.Format(LanguageResources.AddPictureResultLandmarkDescription, landmarkDescription);
        }
    }
}