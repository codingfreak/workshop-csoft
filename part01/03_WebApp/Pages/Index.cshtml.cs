namespace commasoft.Workshop.WebApp.Pages
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Configuration;

    public class IndexModel : PageModel
    {
        #region constructors and destructors

        public IndexModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        #region methods

        public void OnGet()
        {
            ViewData["ApiBaseUrl"] = Configuration["App:ApiBaseUrl"];
        }

        #endregion

        #region properties

        private IConfiguration Configuration { get; }

        #endregion
    }
}