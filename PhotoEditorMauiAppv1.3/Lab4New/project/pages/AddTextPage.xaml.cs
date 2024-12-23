namespace Lab4New;

public partial class AddTextPage : ContentPage
{
    private EditViewModel _editViewModel;
    public AddTextPage(EditViewModel editViewModel)
    {
        InitializeComponent();
        _editViewModel = editViewModel;
        BindingContext = _editViewModel;
    }
}
