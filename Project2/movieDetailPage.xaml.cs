namespace Project2;
[QueryProperty(nameof(MovieProperty), "Movie")]
public partial class movieDetailPage : ContentPage
{

    private Movie movieProperty;
    public Movie MovieProperty
    {
        get => movieProperty;
        set
        {
            movieProperty = value;
            BindingContext = movieProperty;
        }
    }
    public movieDetailPage()
	{
		InitializeComponent();
	}
    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

}