using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net.Http;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace TextTranslatorApp
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        private const int WM_CLIPBOARDUPDATE = 0x031D;

        private Dictionary<string, string> _translations = new Dictionary<string, string>();
        private string _previousClipboardText = string.Empty;
        private HttpClient _httpClient;
        private string _logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "translator_log.txt");
        private string _currentLanguage = "Spanish";
        private string _currentLanguageCode = "es";

        public Form1()
        {
            InitializeComponent();

            // Initialize language dropdown
            comboBoxLanguage.Items.AddRange(new string[] {
    "English (en)",
    "Spanish (es)",
    "French (fr)",
    "German (de)",
    "Chinese (zh)",
    "Portuguese (pt)",
    "Dutch (nl)",
    "Russian (ru)",
    "Korean (ko)",
    "Italian (it)"
});
            comboBoxLanguage.SelectedIndex = 1; // Default to Spanish
            _currentLanguage = "Spanish";
            _currentLanguageCode = "es";

            _httpClient = new HttpClient();
            AddClipboardFormatListener(this.Handle);
            LogMessage("Application started");
        }

        private void LogMessage(string message)
        {
            try
            {
                string logEntry = $"[{DateTime.Now}] {message}";
                File.AppendAllText(_logPath, logEntry + Environment.NewLine);
                Debug.WriteLine(logEntry);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing to log: {ex.Message}");
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                try
                {
                    string clipboardText = Clipboard.GetText();
                    LogMessage($"Clipboard update detected: '{clipboardText}'");

                    if (!string.IsNullOrWhiteSpace(clipboardText) && clipboardText != _previousClipboardText)
                    {
                        _previousClipboardText = clipboardText;

                        // First add to the list
                        if (!listBoxCapturedText.Items.Contains(clipboardText))
                        {
                            listBoxCapturedText.Items.Add(clipboardText);
                            LogMessage($"Added text to list: '{clipboardText}'");
                        }

                        // Then set the text in the original textbox
                        textBoxOriginal.Text = clipboardText;

                        // Then translate
                        TranslateTextAsync(clipboardText);
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"Error processing clipboard: {ex.Message}");
                }
            }
            base.WndProc(ref m);
        }

        private async void TranslateTextAsync(string text)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                this.Invoke((MethodInvoker)delegate {
                    textBoxTranslated.Text = "Translating...";
                });

                LogMessage($"Starting translation for: '{text}' to {_currentLanguage}");

                // Create a unique key for caching that includes the language
                string cacheKey = $"{text}_{_currentLanguageCode}";

                if (_translations.ContainsKey(cacheKey))
                {
                    LogMessage($"Found cached translation for: '{text}' to {_currentLanguageCode}");

                    this.Invoke((MethodInvoker)delegate {
                        textBoxTranslated.Text = _translations[cacheKey];
                    });

                    return;
                }

                // Update prompt to include the selected language
                var requestData = new
                {
                    model = "deepseek-r1:1.5b",
                    prompt = $"Translate the following English text to {_currentLanguage}. Provide ONLY the translation with no additional text or explanation: {text}",
                    stream = false
                };

                var jsonContent = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");


                LogMessage("Sending request to Ollama API...");
                var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", content);

                // Rest of your method remains the same
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    LogMessage($"API Response: {responseContent}");

                    var translatedText = ParseOllamaResponse(responseContent);
                    LogMessage($"Parsed translation: '{translatedText}'");

                    var errorContent = await response.Content.ReadAsStringAsync();
                    LogMessage($"API request failed: {response.StatusCode}, Details: {errorContent}");

                    this.Invoke((MethodInvoker)delegate {
                        textBoxTranslated.Text = $"Translation API request failed: {response.StatusCode}. Details: {errorContent}";
                    });

                    this.Invoke((MethodInvoker)delegate {
                        textBoxTranslated.Text = translatedText;
                        LogMessage($"Updated textBoxTranslated.Text to: '{translatedText}'");
                    });

                    // Cache with language-specific key
                    _translations[cacheKey] = translatedText;
                }
                else
                {
                    LogMessage($"API request failed: {response.StatusCode}");

                    this.Invoke((MethodInvoker)delegate {
                        textBoxTranslated.Text = $"Translation API request failed: {response.StatusCode}";
                    });
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Translation error: {ex.Message}\nStack trace: {ex.StackTrace}");

                this.Invoke((MethodInvoker)delegate {
                    textBoxTranslated.Text = $"Translation error: {ex.Message}";
                });
            }
            finally
            {
                this.Invoke((MethodInvoker)delegate {
                    this.Cursor = Cursors.Default;
                });
            }
        }



        private string ParseOllamaResponse(string jsonResponse)
        {
            try
            {
                LogMessage("Parsing JSON response");
                var responseObj = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                string fullResponse = responseObj.response.ToString();

                // Extract only the final translation (after the </think> tag)
                string result = fullResponse;

                // Check if there's a thinking section to remove
                if (fullResponse.Contains("</think>"))
                {
                    int endOfThinkingIndex = fullResponse.LastIndexOf("</think>") + 9; // 9 is the length of "</think>"
                    if (endOfThinkingIndex < fullResponse.Length)
                    {
                        result = fullResponse.Substring(endOfThinkingIndex).Trim();
                    }
                }

                LogMessage($"Extracted translation: '{result}'");
                return result;
            }
            catch (Exception ex)
            {
                LogMessage($"Error parsing response: {ex.Message}\nJSON: {jsonResponse}");
                return "Error parsing translation response";
            }
        }


        private void buttonTranslate_Click(object sender, EventArgs e)
        {
            LogMessage("Translate button clicked");
            string clipboardText = Clipboard.GetText();
            if (!string.IsNullOrEmpty(clipboardText))
            {
                LogMessage($"Got clipboard text: '{clipboardText}'");
                textBoxOriginal.Text = clipboardText;

                // Add to list if not already present
                if (!listBoxCapturedText.Items.Contains(clipboardText))
                {
                    listBoxCapturedText.Items.Add(clipboardText);
                    LogMessage($"Added text to list: '{clipboardText}'");
                }

                // Translate using current language settings
                TranslateTextAsync(clipboardText);
            }
            else
            {
                LogMessage("Clipboard is empty");
                MessageBox.Show("The clipboard is empty. Please copy some text first.", "No Text to Translate", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



        private void buttonClearAll_Click(object sender, EventArgs e)
        {
            LogMessage("Clear button clicked");
            listBoxCapturedText.Items.Clear();
            textBoxOriginal.Text = "";
            textBoxTranslated.Text = "";
            _translations.Clear();
        }

        private void listBoxCapturedText_SelectedIndexChanged(object sender, EventArgs e)
        {
            LogMessage($"SelectedIndexChanged fired, SelectedIndex: {listBoxCapturedText.SelectedIndex}");
            if (listBoxCapturedText.SelectedIndex != -1)
            {
                string selectedText = listBoxCapturedText.SelectedItem.ToString();
                LogMessage($"List item selected: '{selectedText}'");

                // Update the original text box
                textBoxOriginal.Text = selectedText;

                // Check if we have a cached translation
                if (_translations.ContainsKey(selectedText))
                {
                    LogMessage("Using cached translation");

                    // Ensure this update happens on the UI thread
                    this.Invoke((MethodInvoker)delegate {
                        textBoxTranslated.Text = _translations[selectedText];
                        LogMessage($"Updated textBoxTranslated.Text to: '{_translations[selectedText]}'");
                    });
                }
                else
                {
                    LogMessage("No cached translation, translating now");
                    TranslateTextAsync(selectedText);
                }
            }
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            LogMessage("Application closing");
            RemoveClipboardFormatListener(this.Handle);
            _httpClient?.Dispose();
            base.OnFormClosing(e);
        }

        private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedLanguage = comboBoxLanguage.SelectedItem.ToString();
            _currentLanguageCode = selectedLanguage.Substring(selectedLanguage.Length - 3, 2);
            _currentLanguage = selectedLanguage.Split(' ')[0];

            LogMessage($"Language changed to: {_currentLanguage} ({_currentLanguageCode})");

            // Optionally retranslate current text with new language
            if (!string.IsNullOrEmpty(textBoxOriginal.Text))
            {
                TranslateTextAsync(textBoxOriginal.Text);
            }
        }

        private void listBoxCapturedText_MouseClick(object sender, MouseEventArgs e)
        {
            int index = listBoxCapturedText.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                string selectedText = listBoxCapturedText.Items[index].ToString();
                LogMessage($"List item clicked: '{selectedText}'");

                textBoxOriginal.Text = selectedText;

                // Check if we have a cached translation for this language
                string cacheKey = $"{selectedText}_{_currentLanguageCode}";

                if (_translations.ContainsKey(cacheKey))
                {
                    LogMessage($"Using cached translation for {_currentLanguageCode}");
                    this.Invoke((MethodInvoker)delegate {
                        textBoxTranslated.Text = _translations[cacheKey];
                    });
                }
                else
                {
                    LogMessage($"No cached translation for {_currentLanguageCode}, translating now");
                    TranslateTextAsync(selectedText);
                }
            }
        }




        private void textBoxTranslated_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxTranslated_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBoxOriginal_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxTranslated_TextChanged_2(object sender, EventArgs e)
        {

        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
