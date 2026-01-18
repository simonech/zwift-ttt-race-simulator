# GitHub Pages Setup Instructions

This document outlines the manual steps required to complete the GitHub Pages deployment for the Zwift TTT Race Simulator project.

## What Has Been Done

✅ Created the `docs/` directory with:
- `index.html` - Main website page with content from README.md
- `styles.css` - Styling based on the banner image color scheme (dark blue background with orange/red accents)
- `.nojekyll` - File to ensure GitHub Pages serves CSS files correctly

✅ Created GitHub Actions workflow (`.github/workflows/pages.yml`) to automatically deploy to GitHub Pages

✅ Updated `README.md` to include the banner image at the top

## Manual Steps Required

### 1. Enable GitHub Pages in Repository Settings

Since the GitHub Actions workflow requires GitHub Pages to be enabled and configured, you need to complete these steps in the GitHub repository:

1. Go to the repository settings: `https://github.com/simonech/zwift-ttt-race-simulator/settings/pages`
2. Under **Source**, select **GitHub Actions** from the dropdown (not "Deploy from a branch")
3. Click **Save**

### 2. Merge the Pull Request

Once the PR is merged to the `main` branch:
- The GitHub Actions workflow will automatically run
- The site will be deployed to GitHub Pages
- The site will be available at: `https://simonech.github.io/zwift-ttt-race-simulator/`

### 3. Verify Deployment

After merging:
1. Go to the **Actions** tab in the repository
2. Look for the "Deploy to GitHub Pages" workflow run
3. Wait for it to complete successfully (green checkmark)
4. Visit `https://simonech.github.io/zwift-ttt-race-simulator/` to see the live site

## Design Details

The website design is based on the banner image colors:
- **Background**: Dark navy blue (#1a2332)
- **Accents**: Orange/red gradient (#ff6b35, #e84a3f, #ffa630)
- **Text**: White with gray for secondary text
- **Clean, modern layout** with responsive design for mobile devices

## Website Features

- ✅ Banner image at the top (referenced from GitHub assets)
- ✅ Full README content converted to styled HTML
- ✅ Color-coded sections with emoji icons
- ✅ Responsive design for mobile/tablet/desktop
- ✅ Code blocks with syntax styling
- ✅ "View on GitHub" call-to-action button
- ✅ Professional footer

## Troubleshooting

If the site doesn't load after deployment:

1. **Check workflow status**: Ensure the "Deploy to GitHub Pages" workflow completed successfully
2. **Verify Pages settings**: Make sure GitHub Pages is enabled and set to use GitHub Actions
3. **Check permissions**: The workflow requires `pages: write` and `id-token: write` permissions (already configured)
4. **Wait a few minutes**: GitHub Pages can take 1-2 minutes to propagate changes

If the banner image doesn't load:
- The image is hosted on GitHub's CDN and should load automatically
- If it fails, you may need to download and commit the image to the `docs/` folder

## Future Enhancements

Potential improvements for the website:
- Add a navigation menu for easier section jumping
- Create additional pages (e.g., tutorials, examples, API documentation)
- Add search functionality
- Include interactive examples or demos
- Add a blog section for updates
