using System.Text.RegularExpressions;
using HtmlAgilityPack;
using OKC.Core.Interfaces;
using OKC.Core.Models;

namespace OKC.Infrastructure.Services;

public class HtmlQuestionParser : IQuestionParser
{
    public IEnumerable<Question> Parse(string htmlContent)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        var questions = new List<Question>();

        // Ищем все контейнеры вопросов
        var questionNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'show-question')]");

        foreach (var node in questionNodes)
        {
            try
            {
                var question = new Question
                {
                    QuestionText = CleanText(node.SelectSingleNode(".//div[contains(@class, 'show-question-content')]")?.InnerText),
                    Options = ExtractOptions(node),
                    CorrectAnswer = ExtractCorrectAnswer(node)
                };

                questions.Add(question);
            }
            catch (Exception ex)
            {
                // Логируем ошибки парсинга
                Console.WriteLine($"Ошибка парсинга вопроса: {ex.Message}");
            }
        }

        return questions;
    }

    private List<AnswerOption> ExtractOptions(HtmlNode questionNode)
    {
        var options = new List<AnswerOption>();
        var optionsContainer = questionNode.SelectSingleNode(".//div[contains(@class, 'show-question-choices')]");

        if (optionsContainer == null) return options;

        var optionNodes = optionsContainer.SelectNodes(".//li[contains(@class, 'answer')]");

        foreach (var optionNode in optionNodes)
        {
            var optionText = CleanText(optionNode.SelectSingleNode(".//span[@class='answer']")?.InnerText);
            var isCorrect = optionNode.Attributes["class"]?.Value.Contains("correct-answer") ?? false;

            options.Add(new AnswerOption
            {
                Text = optionText,
                IsCorrect = isCorrect
            });
        }

        return options;
    }

    private string ExtractCorrectAnswer(HtmlNode questionNode)
    {
        var correctNode = questionNode.SelectSingleNode(".//li[contains(@class, 'correct-answer')]");
        return CleanText(correctNode?.SelectSingleNode(".//span[@class='answer']")?.InnerText);
    }

    private string CleanText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        // Удаляем нумерацию вопросов (1., 2. и т.д.)
        text = Regex.Replace(text, @"^\d+\.\s*", "");

        // Удаляем HTML комментатарии
        text = Regex.Replace(text, @"<!--.*?-->", "", RegexOptions.Singleline);

        // Удаляем лишние пробелы
        text = Regex.Replace(text, @"\s+", " ");

        return text.Trim();
    }
}