using WebParking.Domain;

namespace WebParking.ViewModels
{
    public class CarMarksViewModel
    {
        public string CarMarkName { get; set; }

        public IEnumerable<CarMark> CarMarks { get; set; }

        public PageViewModel PageViewModel { get; set; }

        public SortViewModel SortViewModel { get; set; }
    }
}
