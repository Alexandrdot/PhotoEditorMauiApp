namespace Lab4New;

public partial class MainPage : ContentPage
{
	private EditViewModel _editViewModel;
	public MainPage(EditViewModel editViewModel)
	{
		InitializeComponent();
		_editViewModel = editViewModel;
		BindingContext = _editViewModel;
	}
    async void OpenAddTextToPhoto(object sender, EventArgs e)
    {
        App.EditViewModel.ClearFields();
        await Navigation.PushAsync(new AddTextPage(_editViewModel));
    }
    async void OpenSetPixelToPhoto(object sender, EventArgs e)
    {
        App.EditViewModel.ClearFields();
        await Navigation.PushAsync(new SetPixelPage(_editViewModel));
    }
    async void OpenCropToPhoto(object sender, EventArgs e)
    {
        App.EditViewModel.ClearFields();
        await Navigation.PushAsync(new CropPage(_editViewModel));
    }
    async void OpenCollageToPhoto(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CollagePage(_editViewModel));
    }

}


