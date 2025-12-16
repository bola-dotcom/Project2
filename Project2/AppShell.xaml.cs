namespace Project2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //This to register the rout to MovieDetailPage to the appshell
            Routing.RegisterRoute(nameof(movieDetailPage), typeof(movieDetailPage));
        }
    }
}
