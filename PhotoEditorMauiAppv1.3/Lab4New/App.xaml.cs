namespace Lab4New;

public partial class App : Application
{
    public static EditViewModel EditViewModel { get; set; } = new();
    public App()
	{
		InitializeComponent();
        MainPage = new NavigationPage(new MainPage(EditViewModel));
    }
}

