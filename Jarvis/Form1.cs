using System;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using AIMLbot;

namespace Jarvis
{
    public partial class Form1 : Form
    {
        private SpeechRecognitionEngine Engine;
        private SpeechSynthesizer Synthesizer;
        private Bot Bot;
        private User User;

        public Form1()
        {
            InitializeComponent();
        }

        public void LoadSpeech()
        {
            try
            {
                Engine = new SpeechRecognitionEngine();
                Bot = new Bot();

                Engine.SetInputToDefaultAudioDevice();
                Engine.LoadGrammar(new DictationGrammar());
                Engine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Rec);
                Engine.RecognizeAsync(RecognizeMode.Multiple);

                Bot.loadSettings();
                User = new User("Joaquin", Bot);
                Bot.isAcceptingUserInput = false;
                Bot.loadAIMLFromFiles();
                Bot.isAcceptingUserInput = true;

                Synthesizer = new SpeechSynthesizer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSpeech();
        }

        private void Rec(object s, SpeechRecognizedEventArgs e)
        {
            string Speech = e.Result.Text;
            string Answer = GetResponse(Speech);
            
            if(e.Result.Confidence > 0.4f)
            {
                label1.Text = "You: " + Speech;
                label2.Text = "Jarvis: " + Answer;

                Synthesizer.SpeakAsync(Answer);
            }
        }

        private string GetResponse(string Query)
        {
            Request Request = new Request(Query, User, Bot);
            Result Result = Bot.Chat(Request);

            return Result.Output;
        }
    }
}
