namespace ExampleDockManagerViews.ViewModel
{
    public class DocumentViewModel : DummyViewModel
    {
        public override string LongTitle
        {
            get
            {
                return Title + " : " + URL;
            }
        }
    }
}
