# Clipboard Translator

A Windows application that monitors your clipboard for text changes and automatically translates the copied text to your selected language.

## Features

- **Automatic Clipboard Monitoring**: Detects when text is copied to the clipboard and translates it immediately
- **Multiple Language Support**: Translate to 10 different languages including Spanish, French, German, Chinese, Portuguese, Dutch, Russian, Korean, Italian, and English
- **Translation History**: Keeps track of previously translated text for easy reference
- **Offline Translation**: Uses local AI models via Ollama for translation, allowing operation without internet connection
- **Manual Translation**: Translate button to manually trigger translation of clipboard content
- **Language Selection**: Easily switch between target languages using the dropdown menu
- **Caching**: Stores translations to avoid re-translating the same text

## Requirements

- Windows operating system
- [Ollama](https://ollama.ai/) installed and running locally
- A compatible AI model (e.g., DeepSeek, Mistral) pulled via Ollama

## Installation

1. Clone this repository or download the release
2. Build the solution using Visual Studio 2022 or later
3. Ensure Ollama is installed and running
4. Pull a compatible translation model:
   ```
   ollama pull deepseek-r1:1.5b
   ```
   or
   ```
   ollama pull mistral
   ```

## Usage

1. Start the application
2. Select your desired target language from the dropdown
3. Copy any text you want to translate
4. The application will automatically display both the original and translated text
5. View your translation history in the list on the left side
6. Click on any previous translation to view it again
7. Use the Translate button to manually translate clipboard content
8. Clear all translations using the Clear button

## Technical Details

- Built with C# and .NET Framework
- Uses Windows Forms for the user interface
- Integrates with Ollama API for local AI-powered translations
- Implements Windows clipboard monitoring via native API calls

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgements

- Ollama for providing the local AI model infrastructure
- DeepSeek and Mistral for the translation models


#### Links that helped:

[1] https://github.com/ha1fdan/clipboard-translator
[2] https://github.com/sudoker0/Windows-Forms-App-2.0-Unofficial
[3] https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/5426/trados-t-window-for-clipboard
[4] https://www.ccjk.com/google-translate-mobile-features/
[5] https://github.com/dotnet/samples/blob/main/windowsforms/README.md
[6] https://appstore.rws.com/Plugin/51
[7] https://docs.uipath.com/clipboard-ai/standalone/latest/user-guide/introduction
[8] https://www.reddit.com/r/tasker/comments/1byyj2c/translation_of_copied_text_using_google/
[9] https://dev.to/xastrix/lswap-clipboard-translator-4l5i
[10] https://github.com/dotnet/samples/blob/main/README.md
[11] https://play.google.com/store/apps/details?id=quality.multi.copy.managers.apps.labs
[12] https://www.youtube.com/watch?v=XSSD9U67nEE
[13] https://github.com/dotnet/winforms/blob/main/README.md
[14] https://sourceforge.net/projects/copytranslator.mirror/
[15] https://github.com/JalenBrown11/Dashboard
[16] https://bohemicus-software.cz/2020/11/03/how-to-translate-and-write-faster-a-clipboard-manager/
[17] https://github.com/topics/windows-forms
[18] https://support.google.com/translate/answer/6350658?co=GENIE.Platform%3DAndroid
[19] https://www.reddit.com/r/software/comments/pfm05i/windows_application_that_translates_text_copied/
[20] https://github.com/NikolaGrujic91/WinForms-Examples
