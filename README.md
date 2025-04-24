#  <img width="70" alt="image" src="https://github.com/user-attachments/assets/73f21ff3-c1b8-4db6-be14-929219e0518d" width="100"/> EzTranslate - Windows Native ML Language Translator

A Windows application that monitors your clipboard for text changes and automatically translates the copied text to your selected language.

## Screenshots
<img width="450" alt="image" src="https://github.com/user-attachments/assets/5abf948c-7661-471d-8c17-86643ece2a29" width="400" />

Just "copy" any text from anywhere in the Windows Operating System.
<br><br>
<img width="700" alt="image" src="https://github.com/user-attachments/assets/c3baa3db-c760-4c47-8fbd-1b8fb9ecdf34" />

The app will translate the selected text in the background and keep it saved for you check it out later.

## Features

- **Automatic Clipboard Monitoring**: Detects when text is copied to the clipboard and translates it immediately
- **Multiple Language Support**: Translate to 10 different languages including Spanish, French, German, Chinese, Portuguese, Dutch, Russian, Korean, Italian, and English
- **Translation History**: Keeps track of previously translated text for easy reference
- **Offline Translation**: Uses local AI models via Ollama for translation, allowing operation without internet connection
- **Manual Translation**: Translate button to manually trigger translation of clipboard content
- **Language Selection**: Easily switch between target languages using the dropdown menu
- **Caching**: Stores translations to avoid re-translating the same text.

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
   ollama pull zongwei/gemma3-translator:1b
   ```
   or
   ```
   ollama pull deepseek-r1:1.5b
   ```
5. Start the ollama server
   ```
   ollama serve
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

<img src="https://github.com/user-attachments/assets/49d5d4f8-99e1-4ccd-90c3-d4b72dd93c70" alt="drawing" width="400"/>

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgements

- Ollama for providing the local AI model infrastructure
- DeepSeek and Mistral for the translation models
