using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using xingyi.job;
using xingyi.job.Tests;

namespace gui.Pages
{
    public class QuestionPageModel : PageModel
    {
        public Section SectionData { get; set; }

        // OnGet method to initialize the data when the page is loaded
        public void OnGet()
        {
            SectionData = JobDeserialisationFixture.sec1;
        }

        // OnPost method to handle the form submission
        public void OnPost(List<Answer> answers, string Comments, string action)
        {

            if (action == "draft")
            {
                // Handle draft save logic here
            }
            else if (action == "submit")
            {
                // Handle submit logic here
            }
        }
    }
}
