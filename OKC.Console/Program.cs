using OKC.Infrastructure.Services;

var parser = new HtmlQuestionParser();
var repository = new JsonQuestionRepository(Path.GetFullPath("..\\..\\..\\..\\outputs\\IV-до-и-выше-1000В.json"));
var processor = new QuestionProcessor(parser, repository);

var htmlFiles = Directory.GetFiles(Path.GetFullPath("..\\..\\..\\..\\inputs\\4 до и выше"), "*.html");
processor.ProcessFiles(htmlFiles);