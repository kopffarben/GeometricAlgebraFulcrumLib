# GitHub Pages Setup for GA-FuL Documentation

This documentation is configured for GitHub Pages with Jekyll.

## Quick Start

1. **Enable GitHub Pages:**
   - Go to your repository settings
   - Navigate to "Pages" section
   - Under "Source", select the `main` branch and `/docs` folder
   - Click "Save"

2. **Wait for deployment:**
   - GitHub will automatically build and deploy your site
   - Visit: `https://ga-explorer.github.io/GeometricAlgebraFulcrumLib/`

## Features

### ✨ Implemented Features

- **Bilingual Documentation**: Full English and German versions
- **Responsive Navigation**: Menu bar that works on all devices
- **Language Switcher**: Easy switching between EN/DE
- **Modern Design**: Clean, professional styling with gradient accents
- **Mobile-Friendly**: Fully responsive design
- **Code Highlighting**: Syntax highlighting for code examples
- **Copy Buttons**: One-click copy for code blocks
- **Smooth Navigation**: Smooth scrolling and active section highlighting
- **Print Support**: Optimized styles for printing

### 📁 File Structure

```
docs/
├── index.html                          # Landing page with language selector
├── _config.yml                         # Jekyll configuration
├── _layouts/
│   └── default.html                    # Main layout template
├── assets/
│   ├── css/
│   │   └── documentation.css           # Custom styles
│   └── js/
│       └── navigation.js               # Navigation & language switcher
├── README.en.md                        # English documentation
├── README.de.md                        # German documentation
├── getting-started.en.md / .de.md
├── examples.en.md / .de.md
├── architecture.en.md / .de.md
├── design-principles.en.md / .de.md
├── api-reference.en.md / .de.md
└── project-structure.en.md / .de.md
```

### 🎨 Customization

#### Colors

Edit `assets/css/documentation.css` to change the color scheme:

```css
:root {
    --primary-color: #667eea;      /* Main purple */
    --primary-dark: #764ba2;       /* Dark purple */
    --secondary-color: #f093fb;    /* Pink accent */
}
```

#### Navigation Menu

Edit `_config.yml` to modify menu items:

```yaml
navigation:
  en:
    - title: "Home"
      url: "/README.en.html"
    # Add more items...
  de:
    - title: "Startseite"
      url: "/README.de.html"
    # Add more items...
```

## Local Testing

To test locally before deploying:

```bash
# Install Jekyll
gem install bundler jekyll

# Create Gemfile in docs folder
cd docs
cat > Gemfile << EOF
source 'https://rubygems.org'
gem 'github-pages', group: :jekyll_plugins
EOF

# Install dependencies
bundle install

# Serve locally
bundle exec jekyll serve

# Open browser to http://localhost:4000/GeometricAlgebraFulcrumLib/
```

## Troubleshooting

### Pages not updating?

1. Check GitHub Actions tab for build errors
2. Ensure `_config.yml` has correct `baseurl` and `url`
3. Clear browser cache
4. Wait a few minutes for propagation

### Styling not working?

1. Check that CSS file path is correct in `_layouts/default.html`
2. Verify `baseurl` in `_config.yml` matches your repository name
3. Check browser console for 404 errors

### Language switcher not working?

1. Verify JavaScript is loading (check browser console)
2. Ensure file naming is consistent (.en.md / .de.md)
3. Check that front matter includes `lang:` field

## Adding New Pages

1. Create both language versions:
   - `new-page.en.md`
   - `new-page.de.md`

2. Add front matter:
   ```yaml
   ---
   layout: default
   title: "Page Title"
   lang: en  # or de
   ---
   ```

3. Add to navigation in `_config.yml`

4. Link from other pages

## Browser Support

- ✅ Chrome/Edge (latest)
- ✅ Firefox (latest)
- ✅ Safari (latest)
- ✅ Mobile browsers (iOS/Android)
- ⚠️ IE11 (basic support, no advanced features)

## Performance

- Optimized CSS (no heavy frameworks)
- Minimal JavaScript
- No external dependencies (except Jekyll/GitHub Pages)
- Fast load times

## Accessibility

- Semantic HTML5
- ARIA labels where needed
- Keyboard navigation support
- High contrast text
- Responsive design

## License

Documentation is part of the GA-FuL project. See main LICENSE file.

## Support

For issues related to:
- **Content**: Contact Ahmad H. Eid (ga.computing.eg@gmail.com)
- **GitHub Pages**: Check GitHub Pages documentation
- **Jekyll**: See Jekyll documentation

---

**Last Updated:** 2025-10-02
**Jekyll Version:** 3.9.x (GitHub Pages compatible)
**Theme:** Custom (based on Jekyll Cayman theme)
