namespace Lab4New;

public partial class CollagePage : ContentPage
{
    private EditViewModel _editViewModel;
    public CollagePage(EditViewModel editViewModel)
    {
        InitializeComponent();
        _editViewModel = editViewModel;
        BindingContext = _editViewModel;
    }
}
