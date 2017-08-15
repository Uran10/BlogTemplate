using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogTemplate.Models;
using Microsoft.AspNetCore.Authorization;
using BlogTemplate.Services;
using Microsoft.AspNetCore.Http;

namespace BlogTemplate.Pages
{
    [Authorize]
    public class NewModel : PageModel
    {
        const string StorageFolder = "BlogFiles";
        private BlogDataStore _dataStore;
        private readonly SlugGenerator _slugGenerator;
        private readonly ExcerptGenerator _excerptGenerator;

        public NewModel(BlogDataStore dataStore, SlugGenerator slugGenerator, ExcerptGenerator excerptGenerator)
        {
            _dataStore = dataStore;
            _slugGenerator = slugGenerator;
            _excerptGenerator = excerptGenerator;
        }
        public void OnGet()
        {
        }

        [BindProperty]
        public Post Post { get; set; }
        [BindProperty]
        public List<IFormFile> files { get; set; }

        [ValidateAntiForgeryToken]
        public IActionResult OnPostPublish()
        {
            if (ModelState.IsValid)
            {
                Post.IsPublic = true;
                SavePost(Post);
                return Redirect("/Index");
            }

            return Page();
        }

        [ValidateAntiForgeryToken]
        public IActionResult OnPostSaveDraft()
        {
            Post.IsPublic = false;
            SavePost(Post);
            return Redirect("/Index");
        }

        private void SavePost(Post post)
        {
            Post.Slug = _slugGenerator.CreateSlug(Post.Title);

            if (string.IsNullOrEmpty(Post.Excerpt))
            {
                Post.Excerpt = _excerptGenerator.CreateExcerpt(Post.Body, 140);
            }

            _dataStore.SaveFiles(files);
            _dataStore.SavePost(Post);
        }
    }
}
