using System;
using System.Text.Json;
using System.Xml.Serialization;

class Word
{
    public string? Text { get; set; }
    public int Difficulty { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        int mistakes = 0;
        bool hasGuessed = false;
        
        List<Word>? words = new List<Word>();
        int difficulty;
        string fileName = "wordlist.json";
        if (File.Exists(fileName))
        {
            string jsonString = File.ReadAllText(fileName);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            words = JsonSerializer.Deserialize<List<Word>>(jsonString, options);
        }
        else 
        {
            Console.WriteLine("Database not found!");
            return;
        }

        Console.WriteLine("-- Hangman! --");

        do
        {
            difficulty = DifficultyText();
            List<Word> filteredWords = new List<Word>();
            foreach(var word in words)
            {
                if (word.Difficulty == difficulty) filteredWords.Add(word);
            }
            Random rnd = new Random();
            int rndWord = rnd.Next(filteredWords.Count);
            string? chosenWord = filteredWords[rndWord].Text;

            char letter;
            List<char> guessedLetters = new List<char>();
            List<char> wrongLetters = new List<char>();

            while (!hasGuessed & mistakes < 5)
            {
                Console.WriteLine("*************");
                Console.WriteLine($"Mistakes: {mistakes}");
                foreach(char l in chosenWord)
                {
                    if (guessedLetters.Contains(l)) Console.Write(l);
                    else Console.Write("_");
                }
                letter = LetterInput(guessedLetters, wrongLetters);
                
                if (chosenWord.Contains(letter))
                {
                    guessedLetters.Add(letter);
                }
                else
                {
                    Console.WriteLine($"The secret word does not contain {letter}");
                    wrongLetters.Add(letter);
                    mistakes++;
                }
                hasGuessed = chosenWord.All(l => guessedLetters.Contains(l));
            }
            if (mistakes >= 5 & !hasGuessed) Console.WriteLine($"You lost! The secret word was {chosenWord}!");
            else Console.WriteLine($"You won! You only made {mistakes}!");

            chosenWord = null;
            guessedLetters = null;
            wrongLetters = null;
            mistakes = 0;
            Console.WriteLine("-------------");
        } while (true);
    }

    public static int DifficultyText()
    {
        int choice = 0;
        while(choice < 1 || choice > 3)
        {
            Console.Write("Choose the difficulty [1-Easy, 2-Medium, 3-Hard]: ");
            string? input = Console.ReadLine();
            bool isNumber = int.TryParse(input, out choice);
            if(!isNumber || choice < 1 || choice > 3)
            {
                Console.WriteLine("Error! Insert a valid number from 1 to 3!");
                choice = 0;
            }
        }
        return choice;
    }
    public static char LetterInput(List<char> guessedLetters, List<char> wrongLetters)
    {
        string? input;
        char letter;
        Console.WriteLine();
        Console.Write("Choose a letter: ");
        input = Console.ReadLine().ToLower();
        while(!char.TryParse(input, out letter) || string.IsNullOrWhiteSpace(input) || int.TryParse(input, out int i) || guessedLetters.Contains(letter) || wrongLetters.Contains(letter))
        {
            if(guessedLetters.Contains(letter) || wrongLetters.Contains(letter))
            {
                Console.WriteLine($"The letter {letter} has already been guessed.");
            }
            Console.Write("Choose a valid letter: ");
            input = Console.ReadLine().ToLower();
            letter = '\0';
        }
        return letter;
    }
}