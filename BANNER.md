# Banner Image Download

## Overview

The banner image for the Zwift TTT Race Simulator website needs to be downloaded from GitHub's asset storage and committed to this repository.

## Why Local Storage?

The banner image was originally linked directly from GitHub's user attachment CDN:
- These URLs can become inaccessible if issues are made private
- The links may expire or change over time
- Local storage ensures the image is always available

## How to Download

### Option 1: Using the Script (Recommended)

Run the provided download script:
```bash
chmod +x download-banner.sh
./download-banner.sh
```

The script will:
1. Download the banner image from GitHub
2. Save it to `docs/banner.png`
3. Verify the download was successful

### Option 2: Manual Download

If the script fails, download manually:

```bash
curl -L "https://github.com/user-attachments/assets/f849a4fb-2561-43bc-bc7b-d8ef61f56f54" -o docs/banner.png
```

Or visit the URL in your browser and save the image to `docs/banner.png`.

### Option 3: Use wget

```bash
wget -O docs/banner.png "https://github.com/user-attachments/assets/f849a4fb-2561-43bc-bc7b-d8ef61f56f54"
```

## After Download

Once the banner image is downloaded:

1. Verify the image looks correct:
   ```bash
   file docs/banner.png  # Should show "PNG image data"
   ls -lh docs/banner.png  # Should be around 100-500KB
   ```

2. Add and commit the image:
   ```bash
   git add docs/banner.png
   git commit -m "Add banner image to repository"
   git push
   ```

3. The website and README will now display the banner from the local file instead of the external URL.

## References Updated

The following files have been updated to reference `docs/banner.png`:
- `README.md` - Banner at the top of the README
- `docs/index.html` - Banner header on the website

## Image Details

- **Source**: GitHub user attachments
- **Format**: PNG
- **Usage**: Top banner for website and README
- **Design**: Features cyclists in a paceline with power zone color bars
- **Color Scheme**: Dark blue background with orange/red/yellow/green gradient
